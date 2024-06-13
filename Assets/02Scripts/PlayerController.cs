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
        // �ݶ��̴� �غκп��� ���� üũ
        isGround = Physics2D.OverlapArea(new Vector2(col.bounds.min.x + 0.05f, col.bounds.min.y - 0.05f), new Vector2(col.bounds.max.x - 0.05f, col.bounds.min.y - 0.05f), 1 << (int)LAYER.Ground);
        Debug.DrawLine(new Vector2(col.bounds.min.x + 0.05f, col.bounds.min.y - 0.05f), new Vector2(col.bounds.max.x - 0.05f, col.bounds.min.y - 0.05f));

        /*
        if (!isGround) // ���߿� ���� ���
        {
            Collider2D wallCollision;
            if (rigid2D.velocity.x < 0) // �������� �̵��ϰ� ���� ���
            {
                // �ݶ��̴� ���� �κп��� �� �浹 üũ
                wallCollision = Physics2D.OverlapArea(new Vector2(col.bounds.min.x - 0.1f, col.bounds.min.y + 0.01f), new Vector2(col.bounds.min.x - 0.1f, col.bounds.max.y - 0.01f), 1 << (int)LAYER.Ground);
            }
            else // ���������� �̵��ϰ� ���� ���
            {
                // �ݶ��̴� ������ �κп��� �� �浹�� �����Ǿ��� ���
                wallCollision = Physics2D.OverlapArea(new Vector2(col.bounds.max.x + 0.1f, col.bounds.min.y + 0.01f), new Vector2(col.bounds.max.x + 0.1f, col.bounds.max.y - 0.01f), 1 << (int)LAYER.Ground);
            }
            if (wallCollision) // �� �浹 ������ �������� ���
            {
                rigid2D.velocity = new Vector2(-rigid2D.velocity.x, rigid2D.velocity.y); // x �ӵ� ������
                Debug.Log("�� �浹 : " + rigid2D.velocity);
            }
        }
        */
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isGround && collision.gameObject.layer == (int)LAYER.Ground) // ���߿��� ���� �浹���� ���
        {
            Vector2 relativeVelocity = collision.contacts[0].relativeVelocity;

            if (Mathf.Abs(collision.contacts[0].normal.x) > 0.72f) // ��簡 �� 45�� �ʰ�, 135�� �̸��� ���� �΋H���� ���
            {
                Debug.Log("���߿��� ���� �浹��. ���ۿ� : " + relativeVelocity);
                rigid2D.velocity = new Vector2(relativeVelocity.x * speedRetention, -relativeVelocity.y); // �浹 ����� x ������ ���ۿ� �ӵ��� ��������
            }
            else if (collision.contacts[0].normal.y < -0.71f) // õ�忡 �ھ��� ���
            {
                Debug.Log("�Ӹ� ����. ���ۿ� : " + relativeVelocity);
                rigid2D.velocity = new Vector2(-relativeVelocity.x, /*relativeVelocity.y * speedRetention*/rigid2D.velocity.y); // �浹 ����� x ������ ���ۿ� �ӵ��� ��������
            }
        }
    }

    public void Aiming(Vector2 aim, float powerPercent)
    {
        if (isGround) // ������ ���� ���
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
                line.HideAuxiliaryLine(); // ������ ����
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
        if (!isGround) // ������ ���� ���� �� ������ ��ȿȭ��
        {
            Debug.Log("���� ������ ������ ����");
            return;
        }

        line.HideAuxiliaryLine(); // ������ ����        

        aim = AngleLock(aim); // ���� ���� ����
        rigid2D.velocity += aim * (powerPercent * jumpPower); // ������ ������ ���� �̵��ӵ��� �����ش�. (�ƿ� �ٲ������ �̲������� �ִ� �ӵ� ���� �������� �����)
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