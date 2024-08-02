using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEditor;
using Unity.Burst.Intrinsics;
using UnityEngine.SceneManagement;
using Cinemachine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class PlayerController : MonoSingleton<PlayerController>
{
    protected override void SetInstanceToThis()
    {
        instance = this;
    }
    protected override void SceneChanged(Scene replacedScene, Scene newScene)
    {
        line.HideAimingLine();
        
        col.enabled = true;
        LeanTween.cancel(damageTweenID);

        sprite.color = Color.white;

        rigid2D.simulated = true;
        rigid2D.gravityScale = 3f;
        rigid2D.velocity = Vector2.zero;

        damageInsteract = true;
    }

    [SerializeField, Range(0f, 90f), Tooltip("정수리 기준 최소 점프 각도")]
    private float jumpAngle = 60f;
    [SerializeField, Range(0f, 100f)]
    private float jumpPower = 20f;
    [SerializeField, Range(0f, 1f), Tooltip("벽에 부딫혔을 때 X 속도의 보존률")]
    private float speedRetention = 0.75f;

    public float JumpPower => jumpPower;
    public float JumpAngle => jumpAngle;

    private Vector2 angleVec; // angleLock의 각도를 벡터값으로 변환해 저장할 변수
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x - angleVec.x, transform.position.y + angleVec.y));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + angleVec.x, transform.position.y + angleVec.y));

        Gizmos.color = Color.cyan;
        if (col)
        {
            Gizmos.DrawCube(new Vector2(col.bounds.center.x, col.bounds.min.y), new Vector2((col.bounds.extents.x * 2) - 0.02f, 0.02f));
        }
    }
    [Space(5)]
    [SerializeField]
    private bool aiming = false;
    [SerializeField]
    private bool controllable = true;
    [SerializeField]
    private bool alive = true;

    private Collider2D col;
    private Rigidbody2D rigid2D;
    private SpriteRenderer sprite;
    
    private AimingLine line;
    private Collider2D hitBox;

    public Collider2D Col => col;
    public  Rigidbody2D Rigid2D => rigid2D;
    public AimingLine Line => line;
    protected override void Awake()
    {
        base.Awake();

        col = GetComponent<Collider2D>();
        rigid2D = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        line = GetComponentInChildren<AimingLine>();
        hitBox = transform.Find("Hit box").GetComponent<Collider2D>();

        GameManager.Inst.GameOverEvent += Dead;
        GameManager.Inst.GameClearEvent += Clear;

        DragManager.MouseDragEvent += Aiming;
        DragManager.MouseUpEvent += Jump;

        AngleRefresh();
        tempVelo.Enqueue(Vector2.zero); // 처음에 빈 값을 넣어 속도 저장이 한 칸씩 밀리게 함.
    }
    #region _About Velocity Load_
    // 바로 전 속도를 구하기 위한 파트. collision.contact.relativeVelocity는 가끔가다 0, 0으로 나올 때가 있어 이렇게 함.
    private Queue<Vector2> tempVelo = new Queue<Vector2>(2);
    private void PushTempVelo()
    {
        tempVelo.Enqueue(rigid2D.velocity);
        tempVelo.Dequeue();
    }
    private Vector2 GetTempVelo()
    {
        return tempVelo.Peek();
    }
    #endregion

    private int damageTweenID = 0;
    private bool damageInsteract = true;
    ParticleObject particle;
    public void DamageInteract(Attack obstacle)
    {
        if (!damageInsteract) // 대미지 상호작용이 꺼졌을 경우 메소드 종료
        {
            return;
        }

        line.HideAimingLine(); // 녹아 사라질 때 뒤에 점이 보이길래

        Vector2 knockback;
        knockback.x = obstacle.transform.position.x < transform.position.x ? obstacle.HitKnockback.x : -obstacle.HitKnockback.x; // 내가 장애물보다 오른쪽에 있었으면 오른쪽으로, 반대면 왼쪽으로 튐
        knockback.y = obstacle.HitKnockback.y;//obstacle.transform.position.y < transform.position.y ? obstacle.HitKnockback.y : -obstacle.HitKnockback.y; // 내가 장애물보다 위쪽에 있었으면 위쪽으로, 반대면 아래쪽으로 튐
        if (!obstacle.VelocityImpulse) // 이동속도를 완전히 바꿔버리지 않을 경우
        {
            knockback.x += rigid2D.velocity.x; // 기존 이동 속도 더해주기
            knockback.y += rigid2D.velocity.y;
        }
        rigid2D.velocity = knockback;

        rigid2D.gravityScale = obstacle.GravityScale;

        particle = ParticleManager.Inst.PlayParticle(obstacle.Obstacletype, transform);

        switch (obstacle.Obstacletype) // 장애물 타입 비교
        {
            case ParticleType.None: // 아무 것도 안 튈 경우. 보통 잡아먹히거나 떨어졌을 때 사용됨.
                hitBox.enabled = false;
                break;

            case ParticleType.Dust: // 먼지가 튈 경우. 보통 둔기 피해에 사용됨.

                break;

            case ParticleType.Fire: // 불에 탈 경우. 불에 타거나 용암에 빠졌을 때 사용됨.
                LeanTween.cancel(damageTweenID);
                damageTweenID = LeanTween.color(gameObject, CustomColor.fireDamageColor, 0.5f).setOnComplete(() => damageTweenID = 0).id;
                StopCoroutine("FireStop");
                StartCoroutine("FireStop");
                break;

            case ParticleType.Blood: // 피가 튈 경우. 찔리거나 베일 때 사용됨.
                LeanTween.cancel(damageTweenID);
                //damageTweenID = LeanTween.color(gameObject, CustomColor.bloodDamageColor, 0.5f).setOnComplete(() => damageTweenID = 0).id;
                break;

            case ParticleType.Steam: // 증기가 필 경우. 염산에 빠질 때 사용됨.
                LeanTween.cancel(damageTweenID);

                rigid2D.simulated = false;
                col.enabled = false;
                damageTweenID = LeanTween.color(gameObject, CustomColor.zero, 0.5f).setOnComplete(() => damageTweenID = 0).id;
                damageInsteract = false;
                break;

            default:
                Debug.LogError("알려지지 않은 피해를 입음. 알맞은 대미지 이벤트를 제작해주세요.");
                break;
        }
    }
    private IEnumerator FireStop()
    {
        yield return YieldReturn.WaitForSeconds(2f);

        particle.FollowOFF();
    }

    public void DoorInteract(Transform doorPosition)
    {
        LeanTween.moveX(gameObject, doorPosition.position.x, 0.5f);
        controllable = false;
    }
    private void Dead(Attack obstacle)
    {
        controllable = false;
        alive = false;
    }
    private void Clear(bool jewelyClear, bool timeClear, bool jumpClear)
    {

    }
    public void Revive()
    {
        controllable = true;
        alive = true;
    }

    [SerializeField]
    private bool isGround = false;
    public bool IsGround => isGround;
    private void FixedUpdate()
    {
        // 콜라이더 밑부분에서 착지 체크
        isGround = Physics2D.OverlapBox(new Vector2(col.bounds.center.x, col.bounds.min.y), new Vector2((col.bounds.extents.x * 2) - 0.02f, 0.02f), 0f, 1 << (int)LAYER.Ground);
        //isGround = Physics2D.OverlapArea(new Vector2(col.bounds.min.x + 0.05f, col.bounds.min.y - 0.05f), new Vector2(col.bounds.max.x - 0.05f, col.bounds.min.y - 0.05f), 1 << (int)LAYER.Ground);
        //Debug.DrawLine(new Vector2(col.bounds.min.x + 0.05f, col.bounds.min.y - 0.05f), new Vector2(col.bounds.max.x - 0.05f, col.bounds.min.y - 0.05f));

        PushTempVelo(); // 계속해서 이동속도 저장
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == (int)LAYER.Ground) // 공중에서 지형에 충돌했을 경우
        {
            Vector2 lastVelocity = GetTempVelo();
            if (Mathf.Abs(collision.contacts[0].normal.x) > 0.72f) // 경사가 약 45도 초과, 135도 미만인 땅에 부딫혔을 경우
            {
                if (!Mathf.Approximately(Mathf.Sign(lastVelocity.x), Mathf.Sign(collision.contacts[0].normal.x))) // 뒤집은 속도가 벽 충돌 방향의 반대일 경우
                {
                    rigid2D.velocity = new Vector2(-lastVelocity.x * speedRetention, lastVelocity.y); // 충돌 당시의 x 방향의 반작용 속도를 대입해줌
                    Debug.Log("공중에서 벽에 충돌함. 반작용 : " + rigid2D.velocity);
                }
            }
            else if (collision.contacts[0].normal.y < -0.71f) // 천장에 박았을 경우
            {
                rigid2D.velocity = new Vector2(lastVelocity.x, rigid2D.velocity.y); // 충돌 당시의 x 방향의 반작용 속도를 대입해줌
                Debug.Log("머리 박음. 반작용 : " + rigid2D.velocity);
            }

            /*
            Vector2 relativeVelocity = collision.contacts[0].relativeVelocity;

            if (Mathf.Abs(collision.contacts[0].normal.x) > 0.72f) // 경사가 약 45도 초과, 135도 미만인 땅에 부딫혔을 경우
            {
                Debug.Log("공중에서 벽에 충돌함. 반작용 : " + relativeVelocity);
                rigid2D.velocity = new Vector2(relativeVelocity.x * speedRetention, -relativeVelocity.y); // 충돌 당시의 x 방향의 반작용 속도를 대입해줌
            }
            else if (collision.contacts[0].normal.y < -0.71f) // 천장에 박았을 경우
            {
                Debug.Log("머리 박음. 반작용 : " + relativeVelocity);
                rigid2D.velocity = new Vector2(-relativeVelocity.x, rigid2D.velocity.y); // 충돌 당시의 x 방향의 반작용 속도를 대입해줌
            }
            */
        }
    }
    private void OnCollisionStay2D(Collision2D collision) // 속도가 빠르거나 벽에 붙어 점프하는 등 특정 상황에서 벽에 튕기지 않는 상황을 방지하기 위함.
    {
        if (collision.gameObject.layer == (int)LAYER.Ground) // 지형 충돌일 경우
        {
            Vector2 lastVelocity = GetTempVelo();
            if (Mathf.Abs(collision.contacts[0].normal.x) > 0.72f) // 경사가 약 45도 초과, 135도 미만인 땅이 되었을 경우
            {
                if (!Mathf.Approximately(Mathf.Sign(lastVelocity.x), Mathf.Sign(collision.contacts[0].normal.x))) // 뒤집은 속도가 벽 충돌 방향의 반대일 경우
                {
                    rigid2D.velocity = new Vector2(-lastVelocity.x * speedRetention, lastVelocity.y); // 충돌 당시의 x 방향의 반작용 속도를 대입해줌
                    Debug.Log("벽에 빠르게 충돌함. 반작용 : " + rigid2D.velocity);
                }
            }
            else if (collision.contacts[0].normal.y < -0.71f) // 천장에 박았을 경우
            {
                rigid2D.velocity = new Vector2(lastVelocity.x, rigid2D.velocity.y); // 충돌 당시의 x 방향의 반작용 속도를 대입해줌
                Debug.Log("빠르게 머리 박음. 반작용 : " + rigid2D.velocity);
            }
        }
    }

    public void Aiming(Vector2 aim, float powerPercent)
    {
        if ((controllable && Time.timeScale > 0.5f) && isGround) // 조종 가능한 상태(+ 일시정지가 아닐 떄)고 착지해 있을 경우
        {
            float g = Physics2D.gravity.y * rigid2D.gravityScale; // 중력 = 공통 중력 세기 * 대상의 중력 세기
            Vector2 startSpeed = (AngleLock(aim) * powerPercent * jumpPower) + rigid2D.velocity; // 초기 속도 = (조준 각도(AngleLock 처리) * 세기 퍼센트 * 최대 점프 파워) + 현재 오브젝트 속도
            Color dotColor = new Color(1f, 1f - powerPercent, 1f - powerPercent, 0.75f); // 점의 컬러 저장. 셀수록 점들이 빨개진다.
            line.SetAuxiliaryLine(g, startSpeed, dotColor); // 보조선 세팅

            sprite.flipX = (aim.x < 0f); // 왼쪽 조준했을 경우 스프라이트 뒤집기
        }
        else // 공중에 있을 경우
        {
            if (line.Visible) // 보조선이 보일 경우
            {
                line.HideAimingLine(); // 보조선 숨김
            }
            return;
        }
    }
    public Vector2 AngleLock(Vector2 aim) // 중력 방향에 맞춰 조준 각도에 락을 거는 메소드 (무중력은 없음)
    {
        if (rigid2D.gravityScale > 0f) // 아래 방향 중력(기본)일 경우
        {
            if (aim.y < angleVec.y) // 조준의 y 성분이 점프각을 벗어낫을 경우 x 조준 방향에 맞는 최소 벡터로 바꿔준다.
            {
                aim.x = aim.x  < 0 ? -angleVec.x : angleVec.x;
                aim.y = angleVec.y;
            }
        }
        else // 위 방향 중력(반중력)일 경우
        {
            if (aim.y > angleVec.y) // 조준의 y 성분이 점프각을 벗어낫을 경우
            {
                aim.x = aim.x < 0 ? -angleVec.x : angleVec.x;
                aim.y = -angleVec.y;
            }
        }

        return aim.normalized;
    }
    public void Jump(Vector2 aim, float powerPercent)
    {
        if (!isGround || !(controllable && Time.timeScale > 0.5f)) // 착지해 있지 않거나 컨트롤이 안 될 땐 점프를 무효화함
        {
            Debug.Log("점프할 수 없는 상태");
            return;
        }

        if (SceneManager.GetActiveScene().buildIndex > (int)SCENE.StageSelect)
        {
            GameManager.Inst.JumpCount++;
        }

        line.HideAimingLine(); // 보조선 숨김        

        aim = AngleLock(aim); // 점프 각도 조정
        rigid2D.velocity += aim * (powerPercent * jumpPower); // 조정된 점프를 현재 이동속도에 더해준다. (아예 바꿔버리면 미끄러지고 있던 속도 등의 디테일이 사라짐)
        PushTempVelo(); // 바뀐 속도 저장. 2번 해야 이 값이 나오므로 2번 반복
        PushTempVelo();
    }
    public void AngleRefresh()
    {
        float angle = 90f - jumpAngle; // 지면과 최소 벡터와의 각도

        angleVec = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerController))]
public class E_PlayerController : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlayerController script = (PlayerController)target;

        if (GUILayout.Button("Angle Lock Changed"))
        {
            script.AngleRefresh();
        }
    }
}
#endif

public static class CustomColor
{
    public static readonly Color zero = new Color(0f, 0f, 0f, 0f);

    public static readonly Color bloodDamageColor = new Color(0.75f, 0f, 0f, 1f);
    public static readonly Color fireDamageColor = new Color(0.25f, 0.25f, 0.25f, 1f);

}