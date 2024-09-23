using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEditor;
using Unity.Burst.Intrinsics;
using UnityEngine.SceneManagement;
using Cinemachine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : Singleton<PlayerController>
{
    protected override void SceneChanged(Scene replacedScene, Scene newScene)
    {
        line.HideAimingLine();
        anim.SetTrigger(Hash_Alive);

        col.enabled = true;
        StopCoroutine("ColorChange");
        sprite.color = CustomColor.zero;

        rigid2D.simulated = true;
        rigid2D.gravityScale = 3f;
        rigid2D.velocity = Vector2.zero;

        hitBox.enabled = true;
        damageInsteract = true;
    }

    [SerializeField, Range(0f, 90f), Tooltip("������ ���� �ּ� ���� ����")]
    private float jumpAngle = 60f;
    [SerializeField, Range(0f, 100f)]
    private float jumpPower = 20f;
    [SerializeField, Range(0f, 1f), Tooltip("���� �΋H���� �� X �ӵ��� ������")]
    private float speedRetention = 0.75f;

    public float JumpPower => jumpPower;
    public float JumpAngle => jumpAngle;

    private Vector2 angleVec; // angleLock�� ������ ���Ͱ����� ��ȯ�� ������ ����
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
    public bool Aiming => aiming;
    [SerializeField]
    private bool controllable = true;

    private SpriteRenderer sprite;
    private Collider2D col;
    private Rigidbody2D rigid2D;
    
    private AimingLine line;
    private Collider2D hitBox;
    private Animator anim;

    public Collider2D Col => col;
    public  Rigidbody2D Rigid2D => rigid2D;
    public AimingLine Line => line;
    protected override void Awake()
    {
        base.Awake();

        sprite = GetComponentInChildren<SpriteRenderer>();
        col = GetComponentInChildren<Collider2D>();
        rigid2D = GetComponent<Rigidbody2D>();

        line = GetComponentInChildren<AimingLine>();
        hitBox = transform.Find("Hit box").GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();

        GameManager.GameOverEvent += Dead;
        GameManager.GameClearEvent += Clear;

        DragManager.MouseDragEvent += Aim;
        DragManager.MouseUpEvent += Jump;

        AngleRefresh();
        tempVelo.Enqueue(Vector2.zero); // ó���� �� ���� �־� �ӵ� ������ �� ĭ�� �и��� ��.
    }
    #region _About Velocity Load_
    // �ٷ� �� �ӵ��� ���ϱ� ���� ��Ʈ. collision.contact.relativeVelocity�� �������� 0, 0���� ���� ���� �־� �̷��� ��.
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

    private bool damageInsteract = true;
    public void DamageInteract(Attack obstacle)
    {
        if (!damageInsteract) // ����� ��ȣ�ۿ��� ������ ��� �޼ҵ� ����
        {
            return;
        }

        line.HideAimingLine(); // ��� ����� �� �ڿ� ���� ���̱淡

        Vector2 knockback;
        knockback.x = obstacle.transform.position.x < transform.position.x ? obstacle.HitKnockback.x : -obstacle.HitKnockback.x; // ���� ��ֹ����� �����ʿ� �־����� ����������, �ݴ�� �������� Ʀ
        knockback.y = obstacle.HitKnockback.y; // ���� ��ֹ����� ���ʿ� �־����� ��������, �ݴ�� �Ʒ������� Ʀ
        if (!obstacle.VelocityImpulse) // �̵��ӵ��� ������ �ٲ������ ���� ���
        {
            knockback.x += rigid2D.velocity.x; // ���� �̵� �ӵ� �����ֱ�
            knockback.y += rigid2D.velocity.y;
        }
        rigid2D.velocity = knockback;
        SpriteFlip(rigid2D.velocity.x);

        rigid2D.gravityScale = obstacle.GravityScale;

        EffectObject particle = EffectManager.Inst.PlayParticle(obstacle.Obstacletype, transform);
        anim.SetTrigger(Hash_Die);
        EffectManager.Inst.PlaySFX(ResourceLoader<AudioClip>.ResourceLoad(FolderName.Death, obstacle.Obstacletype.ToString()));

        switch (obstacle.Obstacletype) // ��ֹ� Ÿ�� ��
        {
            case ParticleType.None: // �ƹ� �͵� �� ƥ ���. ���� ��Ƹ����ų� �������� �� ����.
                hitBox.enabled = false;
                break;

            case ParticleType.Dust: // ������ ƥ ���. ���� �б� ���ؿ� ����.

                break;

            case ParticleType.Fire: // �ҿ� Ż ���. �ҿ� Ÿ�ų� ��Ͽ� ������ �� ����.
                StopCoroutine("ColorChange");
                StartCoroutine(ColorChange(CustomColor.fireDamageColor));
                break;

            case ParticleType.Blood: // �ǰ� ƥ ���. �񸮰ų� ���� �� ����.
                break;

            case ParticleType.Steam: // ���Ⱑ �� ���. ���꿡 ���� �� ����.
                StopCoroutine("ColorChange");
                hitBox.enabled = false;
                StartCoroutine(ColorChange(CustomColor.acidDamageColor));
                break;

            default:
                Debug.LogError("�˷����� ���� ���ظ� ����. �˸��� ����� �̺�Ʈ�� �������ּ���.");
                break;
        }
    }
    private IEnumerator ColorChange(Color newColor)
    {
        Color colorDiff = (newColor - sprite.color) * Time.deltaTime / 0.5f;
        float f = 0f;
        while (f <= 0.5f)
        {
            f += Time.deltaTime;
            sprite.color += colorDiff;

            yield return YieldReturn.waitForFixedUpdate;
        }
    }
    private IEnumerator FireStop(EffectObject fireParticle)
    {
        yield return YieldReturn.WaitForSeconds(2f);

        fireParticle.ReturnToPool();
    }

    public void DoorInteract(Transform doorPosition)
    {
        //LeanTween.moveX(gameObject, doorPosition.position.x, 0.5f);
        controllable = false;
        hitBox.enabled = false;
        rigid2D.velocity = Vector2.zero;
    }
    private void Dead(Attack obstacle)
    {
        controllable = false;
    }
    private void Clear(bool jewelyClear, bool timeClear, bool jumpClear)
    {

    }
    public void Revive()
    {
        controllable = true;
    }

    [SerializeField]
    private bool isGround = false;
    public bool IsGround => isGround;
    private void FixedUpdate()
    {
        // �ݶ��̴� �غκп��� ���� üũ
        isGround = Physics2D.OverlapBox(new Vector2(col.bounds.center.x, col.bounds.min.y), new Vector2((col.bounds.extents.x * 2) - 0.02f, 0.02f), 0f, 1 << (int)LAYER.Ground);
        anim.SetBool(Hash_Landing, isGround);

        PushTempVelo(); // ����ؼ� �̵��ӵ� ����
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GroundCollision(collision);
    }
    private void OnCollisionStay2D(Collision2D collision) // �ӵ��� �����ų� ���� �پ� �����ϴ� �� Ư�� ��Ȳ���� ���� ƨ���� �ʴ� ��Ȳ�� �����ϱ� ����.
    {
        GroundCollision(collision);
    }
    private void GroundCollision(Collision2D collision)
    {
        if (collision.gameObject.layer == (int)LAYER.Ground) // ���� �浹�� ���
        {
            Vector2 lastVelocity = GetTempVelo();
            if (Mathf.Abs(collision.contacts[0].normal.x) > 0.72f) // ��簡 �� 45�� �ʰ�, 135�� �̸��� ���� �Ǿ��� ���
            {
                if (!Mathf.Approximately(Mathf.Sign(lastVelocity.x), Mathf.Sign(collision.contacts[0].normal.x))) // ������ �ӵ��� �� �浹 ������ �ݴ��� ���
                {
                    EffectManager.Inst.PlaySFX(ResourceLoader<AudioClip>.ResourceLoad(FolderName.Player, "HitWall"));

                    rigid2D.velocity = new Vector2(-lastVelocity.x * speedRetention, lastVelocity.y); // �浹 ����� x ������ ���ۿ� �ӵ��� ��������
                    anim.SetTrigger(Hash_Knockback);
                    Invoke("ResetKnockback", 0.1f);
                    SpriteFlip(rigid2D.velocity.x);
                }
            }
            else if (collision.contacts[0].normal.y < -0.71f) // õ�忡 �ھ��� ���
            {
                rigid2D.velocity = new Vector2(lastVelocity.x, rigid2D.velocity.y); // �浹 ����� x ������ ���ۿ� �ӵ��� ��������
            }
        }
    }
    private void ResetKnockback()
    {
        anim.ResetTrigger(Hash_Knockback);
    }

    public void Aim(Vector2 aim, float powerPercent)
    {
        if ((controllable && Time.timeScale > 0.5f) && isGround) // ���� ������ ����(+ �Ͻ������� �ƴ� ��)�� ������ ���� ���
        {
            aiming = true;
            anim.SetBool(Hash_Aim, true);
            SpriteFlip(aim.x, true);

            float g = Physics2D.gravity.y * rigid2D.gravityScale; // �߷� = ���� �߷� ���� * ����� �߷� ����
            startSpeed = (AngleLock(aim) * powerPercent * jumpPower) + rigid2D.velocity; // �ʱ� �ӵ� = (���� ����(AngleLock ó��) * ���� �ۼ�Ʈ * �ִ� ���� �Ŀ�) + ���� ������Ʈ �ӵ�
            Color dotColor = new Color(1f, 1f - powerPercent, 1f - powerPercent, 0.75f); // ���� �÷� ����. ������ ������ ��������.
            line.SetAuxiliaryLine(g , startSpeed, dotColor); // ������ ����
        }
        else // ���߿� ���� ���
        {
            if (line.Visible) // �������� ���� ���
            {
                line.HideAimingLine(); // ������ ����
                aiming = false;
            }
            return;
        }
    }
    public Vector2 AngleLock(Vector2 aim) // �߷� ���⿡ ���� ���� ������ ���� �Ŵ� �޼ҵ� (���߷��� ����)
    {
        if (rigid2D.gravityScale > 0f) // �Ʒ� ���� �߷�(�⺻)�� ���
        {
            if (aim.y < angleVec.y) // ������ y ������ �������� ����� ��� x ���� ���⿡ �´� �ּ� ���ͷ� �ٲ��ش�.
            {
                aim.x = aim.x  < 0 ? -angleVec.x : angleVec.x;
                aim.y = angleVec.y;
            }
        }
        else // �� ���� �߷�(���߷�)�� ���
        {
            if (aim.y > angleVec.y) // ������ y ������ �������� ����� ���
            {
                aim.x = aim.x < 0 ? -angleVec.x : angleVec.x;
                aim.y = -angleVec.y;
            }
        }

        return aim.normalized;
    }
    private void SpriteFlip(float x, bool isAiming = false)
    {
        float flip = x > 0 ? 0f : 180f;
        if (isAiming)
            flip -= 180f;

        transform.GetChild(0).eulerAngles = new Vector3(0f, flip, 0f);
    }

    private Vector2 startSpeed;
    public void Jump(Vector2 aim, float powerPercent)
    {
        aiming = false;
        anim.SetBool(Hash_Aim, false);

        if (!isGround || !(controllable && Time.timeScale > 0.5f)) // ������ ���� �ʰų� ��Ʈ���� �� �� �� ������ ��ȿȭ��
        {
            Debug.Log("������ �� ���� ����");
            return;
        }

        if (SceneManager.GetActiveScene().buildIndex > (int)SCENE.StageSelect)
        {
            GameManager.Inst.JumpCount++;
        }

        line.HideAimingLine(); // ������ ����        
        EffectManager.Inst.PlaySFX(ResourceLoader<AudioClip>.ResourceLoad(FolderName.Player, "Jump"));

        aim = AngleLock(aim); // ���� ���� ����
        rigid2D.velocity = startSpeed; // ������ ������ ���� �̵��ӵ��� �����ش�. (�ƿ� �ٲ������ �̲������� �ִ� �ӵ� ���� �������� �����)
        PushTempVelo(); // �ٲ� �ӵ� ����. 2�� �ؾ� �� ���� �����Ƿ� 2�� �ݺ�
        PushTempVelo();
    }
    public void AngleRefresh()
    {
        float angle = 90f - jumpAngle; // ����� �ּ� ���Ϳ��� ����

        angleVec = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }

    private readonly int Hash_Aim = Animator.StringToHash("Aim");
    private readonly int Hash_Knockback = Animator.StringToHash("Knockback");
    private readonly int Hash_Landing = Animator.StringToHash("Landing");
    private readonly int Hash_Die = Animator.StringToHash("Die");
    private readonly int Hash_Alive = Animator.StringToHash("Alive");
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

    public static readonly Color acidDamageColor = new Color(0f, 0.15f, 0.05f, 1f);
    public static readonly Color fireDamageColor = new Color(0.15f, 0f, 0f, 1f);

}