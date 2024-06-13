using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using Unity.Burst.Intrinsics;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class PlayerController : MonoSingleton<PlayerController>
{
    protected override void SetInstanceToThis()
    {
        instance = this;
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
    }
    [Space(5)]
    [SerializeField]
    private bool aiming = false;

    private Collider2D col;
    private Rigidbody2D rigid2D;
    private SpriteRenderer sprite;
    private AimingLine line;

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

        DragManager drag = GetComponentInChildren<DragManager>();

        drag.MouseDragEvent += Aiming;
        drag.MouseUpEvent += Jump;

        AngleRefresh();
    }

    [SerializeField]
    private bool isGround = false;
    public bool IsGround => isGround;
    private void FixedUpdate()
    {
        // 콜라이더 밑부분에서 착지 체크
        isGround = Physics2D.OverlapArea(new Vector2(col.bounds.min.x + 0.05f, col.bounds.min.y - 0.05f), new Vector2(col.bounds.max.x - 0.05f, col.bounds.min.y - 0.05f), 1 << (int)LAYER.Ground);
        Debug.DrawLine(new Vector2(col.bounds.min.x + 0.05f, col.bounds.min.y - 0.05f), new Vector2(col.bounds.max.x - 0.05f, col.bounds.min.y - 0.05f));

        /*
        if (!isGround) // 공중에 있을 경우
        {
            Collider2D wallCollision;
            if (rigid2D.velocity.x < 0) // 왼쪽으로 이동하고 있을 경우
            {
                // 콜라이더 왼쪽 부분에서 벽 충돌 체크
                wallCollision = Physics2D.OverlapArea(new Vector2(col.bounds.min.x - 0.1f, col.bounds.min.y + 0.01f), new Vector2(col.bounds.min.x - 0.1f, col.bounds.max.y - 0.01f), 1 << (int)LAYER.Ground);
            }
            else // 오른쪽으로 이동하고 있을 경우
            {
                // 콜라이더 오른쪽 부분에서 벽 충돌이 감지되었을 경우
                wallCollision = Physics2D.OverlapArea(new Vector2(col.bounds.max.x + 0.1f, col.bounds.min.y + 0.01f), new Vector2(col.bounds.max.x + 0.1f, col.bounds.max.y - 0.01f), 1 << (int)LAYER.Ground);
            }
            if (wallCollision) // 벽 충돌 감지에 성공했을 경우
            {
                rigid2D.velocity = new Vector2(-rigid2D.velocity.x, rigid2D.velocity.y); // x 속도 뒤집기
                Debug.Log("벽 충돌 : " + rigid2D.velocity);
            }
        }
        */
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isGround && collision.gameObject.layer == (int)LAYER.Ground) // 공중에서 벽에 충돌했을 경우
        {
            Vector2 relativeVelocity = collision.contacts[0].relativeVelocity;

            if (Mathf.Abs(collision.contacts[0].normal.x) > 0.72f) // 경사가 약 45도 초과, 135도 미만인 땅에 부딫혔을 경우
            {
                Debug.Log("공중에서 벽에 충돌함. 반작용 : " + relativeVelocity);
                rigid2D.velocity = new Vector2(relativeVelocity.x * speedRetention, -relativeVelocity.y); // 충돌 당시의 x 방향의 반작용 속도를 대입해줌
            }
            else if (collision.contacts[0].normal.y < -0.71f) // 천장에 박았을 경우
            {
                Debug.Log("머리 박음. 반작용 : " + relativeVelocity);
                rigid2D.velocity = new Vector2(-relativeVelocity.x, /*relativeVelocity.y * speedRetention*/rigid2D.velocity.y); // 충돌 당시의 x 방향의 반작용 속도를 대입해줌
            }
        }
    }

    public void Aiming(Vector2 aim, float powerPercent)
    {
        if (isGround) // 착지해 있을 경우
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
                line.HideAuxiliaryLine(); // 보조선 숨김
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
        if (!isGround) // 착지해 있지 않을 시 점프를 무효화함
        {
            Debug.Log("공중 점프는 허용되지 않음");
            return;
        }

        line.HideAuxiliaryLine(); // 보조선 숨김        

        aim = AngleLock(aim); // 점프 각도 조정
        rigid2D.velocity += aim * (powerPercent * jumpPower); // 조정된 점프를 현재 이동속도에 더해준다. (아예 바꿔버리면 미끄러지고 있던 속도 등의 디테일이 사라짐)
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