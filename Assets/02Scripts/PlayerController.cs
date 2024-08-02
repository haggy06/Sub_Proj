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

    private int damageTweenID = 0;
    private bool damageInsteract = true;
    ParticleObject particle;
    public void DamageInteract(Attack obstacle)
    {
        if (!damageInsteract) // ����� ��ȣ�ۿ��� ������ ��� �޼ҵ� ����
        {
            return;
        }

        line.HideAimingLine(); // ��� ����� �� �ڿ� ���� ���̱淡

        Vector2 knockback;
        knockback.x = obstacle.transform.position.x < transform.position.x ? obstacle.HitKnockback.x : -obstacle.HitKnockback.x; // ���� ��ֹ����� �����ʿ� �־����� ����������, �ݴ�� �������� Ʀ
        knockback.y = obstacle.HitKnockback.y;//obstacle.transform.position.y < transform.position.y ? obstacle.HitKnockback.y : -obstacle.HitKnockback.y; // ���� ��ֹ����� ���ʿ� �־����� ��������, �ݴ�� �Ʒ������� Ʀ
        if (!obstacle.VelocityImpulse) // �̵��ӵ��� ������ �ٲ������ ���� ���
        {
            knockback.x += rigid2D.velocity.x; // ���� �̵� �ӵ� �����ֱ�
            knockback.y += rigid2D.velocity.y;
        }
        rigid2D.velocity = knockback;

        rigid2D.gravityScale = obstacle.GravityScale;

        particle = ParticleManager.Inst.PlayParticle(obstacle.Obstacletype, transform);

        switch (obstacle.Obstacletype) // ��ֹ� Ÿ�� ��
        {
            case ParticleType.None: // �ƹ� �͵� �� ƥ ���. ���� ��Ƹ����ų� �������� �� ����.
                hitBox.enabled = false;
                break;

            case ParticleType.Dust: // ������ ƥ ���. ���� �б� ���ؿ� ����.

                break;

            case ParticleType.Fire: // �ҿ� Ż ���. �ҿ� Ÿ�ų� ��Ͽ� ������ �� ����.
                LeanTween.cancel(damageTweenID);
                damageTweenID = LeanTween.color(gameObject, CustomColor.fireDamageColor, 0.5f).setOnComplete(() => damageTweenID = 0).id;
                StopCoroutine("FireStop");
                StartCoroutine("FireStop");
                break;

            case ParticleType.Blood: // �ǰ� ƥ ���. �񸮰ų� ���� �� ����.
                LeanTween.cancel(damageTweenID);
                //damageTweenID = LeanTween.color(gameObject, CustomColor.bloodDamageColor, 0.5f).setOnComplete(() => damageTweenID = 0).id;
                break;

            case ParticleType.Steam: // ���Ⱑ �� ���. ���꿡 ���� �� ����.
                LeanTween.cancel(damageTweenID);

                rigid2D.simulated = false;
                col.enabled = false;
                damageTweenID = LeanTween.color(gameObject, CustomColor.zero, 0.5f).setOnComplete(() => damageTweenID = 0).id;
                damageInsteract = false;
                break;

            default:
                Debug.LogError("�˷����� ���� ���ظ� ����. �˸��� ����� �̺�Ʈ�� �������ּ���.");
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
        // �ݶ��̴� �غκп��� ���� üũ
        isGround = Physics2D.OverlapBox(new Vector2(col.bounds.center.x, col.bounds.min.y), new Vector2((col.bounds.extents.x * 2) - 0.02f, 0.02f), 0f, 1 << (int)LAYER.Ground);
        //isGround = Physics2D.OverlapArea(new Vector2(col.bounds.min.x + 0.05f, col.bounds.min.y - 0.05f), new Vector2(col.bounds.max.x - 0.05f, col.bounds.min.y - 0.05f), 1 << (int)LAYER.Ground);
        //Debug.DrawLine(new Vector2(col.bounds.min.x + 0.05f, col.bounds.min.y - 0.05f), new Vector2(col.bounds.max.x - 0.05f, col.bounds.min.y - 0.05f));

        PushTempVelo(); // ����ؼ� �̵��ӵ� ����
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == (int)LAYER.Ground) // ���߿��� ������ �浹���� ���
        {
            Vector2 lastVelocity = GetTempVelo();
            if (Mathf.Abs(collision.contacts[0].normal.x) > 0.72f) // ��簡 �� 45�� �ʰ�, 135�� �̸��� ���� �΋H���� ���
            {
                if (!Mathf.Approximately(Mathf.Sign(lastVelocity.x), Mathf.Sign(collision.contacts[0].normal.x))) // ������ �ӵ��� �� �浹 ������ �ݴ��� ���
                {
                    rigid2D.velocity = new Vector2(-lastVelocity.x * speedRetention, lastVelocity.y); // �浹 ����� x ������ ���ۿ� �ӵ��� ��������
                    Debug.Log("���߿��� ���� �浹��. ���ۿ� : " + rigid2D.velocity);
                }
            }
            else if (collision.contacts[0].normal.y < -0.71f) // õ�忡 �ھ��� ���
            {
                rigid2D.velocity = new Vector2(lastVelocity.x, rigid2D.velocity.y); // �浹 ����� x ������ ���ۿ� �ӵ��� ��������
                Debug.Log("�Ӹ� ����. ���ۿ� : " + rigid2D.velocity);
            }

            /*
            Vector2 relativeVelocity = collision.contacts[0].relativeVelocity;

            if (Mathf.Abs(collision.contacts[0].normal.x) > 0.72f) // ��簡 �� 45�� �ʰ�, 135�� �̸��� ���� �΋H���� ���
            {
                Debug.Log("���߿��� ���� �浹��. ���ۿ� : " + relativeVelocity);
                rigid2D.velocity = new Vector2(relativeVelocity.x * speedRetention, -relativeVelocity.y); // �浹 ����� x ������ ���ۿ� �ӵ��� ��������
            }
            else if (collision.contacts[0].normal.y < -0.71f) // õ�忡 �ھ��� ���
            {
                Debug.Log("�Ӹ� ����. ���ۿ� : " + relativeVelocity);
                rigid2D.velocity = new Vector2(-relativeVelocity.x, rigid2D.velocity.y); // �浹 ����� x ������ ���ۿ� �ӵ��� ��������
            }
            */
        }
    }
    private void OnCollisionStay2D(Collision2D collision) // �ӵ��� �����ų� ���� �پ� �����ϴ� �� Ư�� ��Ȳ���� ���� ƨ���� �ʴ� ��Ȳ�� �����ϱ� ����.
    {
        if (collision.gameObject.layer == (int)LAYER.Ground) // ���� �浹�� ���
        {
            Vector2 lastVelocity = GetTempVelo();
            if (Mathf.Abs(collision.contacts[0].normal.x) > 0.72f) // ��簡 �� 45�� �ʰ�, 135�� �̸��� ���� �Ǿ��� ���
            {
                if (!Mathf.Approximately(Mathf.Sign(lastVelocity.x), Mathf.Sign(collision.contacts[0].normal.x))) // ������ �ӵ��� �� �浹 ������ �ݴ��� ���
                {
                    rigid2D.velocity = new Vector2(-lastVelocity.x * speedRetention, lastVelocity.y); // �浹 ����� x ������ ���ۿ� �ӵ��� ��������
                    Debug.Log("���� ������ �浹��. ���ۿ� : " + rigid2D.velocity);
                }
            }
            else if (collision.contacts[0].normal.y < -0.71f) // õ�忡 �ھ��� ���
            {
                rigid2D.velocity = new Vector2(lastVelocity.x, rigid2D.velocity.y); // �浹 ����� x ������ ���ۿ� �ӵ��� ��������
                Debug.Log("������ �Ӹ� ����. ���ۿ� : " + rigid2D.velocity);
            }
        }
    }

    public void Aiming(Vector2 aim, float powerPercent)
    {
        if ((controllable && Time.timeScale > 0.5f) && isGround) // ���� ������ ����(+ �Ͻ������� �ƴ� ��)�� ������ ���� ���
        {
            float g = Physics2D.gravity.y * rigid2D.gravityScale; // �߷� = ���� �߷� ���� * ����� �߷� ����
            Vector2 startSpeed = (AngleLock(aim) * powerPercent * jumpPower) + rigid2D.velocity; // �ʱ� �ӵ� = (���� ����(AngleLock ó��) * ���� �ۼ�Ʈ * �ִ� ���� �Ŀ�) + ���� ������Ʈ �ӵ�
            Color dotColor = new Color(1f, 1f - powerPercent, 1f - powerPercent, 0.75f); // ���� �÷� ����. ������ ������ ��������.
            line.SetAuxiliaryLine(g, startSpeed, dotColor); // ������ ����

            sprite.flipX = (aim.x < 0f); // ���� �������� ��� ��������Ʈ ������
        }
        else // ���߿� ���� ���
        {
            if (line.Visible) // �������� ���� ���
            {
                line.HideAimingLine(); // ������ ����
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
    public void Jump(Vector2 aim, float powerPercent)
    {
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

        aim = AngleLock(aim); // ���� ���� ����
        rigid2D.velocity += aim * (powerPercent * jumpPower); // ������ ������ ���� �̵��ӵ��� �����ش�. (�ƿ� �ٲ������ �̲������� �ִ� �ӵ� ���� �������� �����)
        PushTempVelo(); // �ٲ� �ӵ� ����. 2�� �ؾ� �� ���� �����Ƿ� 2�� �ݺ�
        PushTempVelo();
    }
    public void AngleRefresh()
    {
        float angle = 90f - jumpAngle; // ����� �ּ� ���Ϳ��� ����

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