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
    [SerializeField]
    private Vector2 hitKnockback;

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
            Gizmos.DrawCube(new Vector2(col.bounds.center.x, col.bounds.min.y), new Vector2((col.bounds.extents.x * 2) - 0.05f, 0.1f));
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
    private PlayerInteract interact;

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
        interact = GetComponentInChildren<PlayerInteract>();
        interact.HitEvent += Dead;
        interact.ClearEvent += Clear;

        DragManager drag = GetComponentInChildren<DragManager>();

        drag.MouseDragEvent += Aiming;
        drag.MouseUpEvent += Jump;

        AngleRefresh();
    }
    private void Dead(Obstacle obstacle)
    {
        Debug.Log(obstacle.gameObject.name + "�� ���� ���");
        controllable = false;
        alive = false;

        switch (obstacle.Obstacletype) // ��ֹ� Ÿ�� ��
        {
            case ObstacleType.Physical: // ���� Ÿ�� ������ ���
                Vector2 knockback;
                knockback.x = obstacle.transform.position.x < transform.position.x ? hitKnockback.x : -hitKnockback.x; // ���� ��ֹ����� �����ʿ� �־����� ����������, �ݴ�� �������� Ʀ
                knockback.y = obstacle.transform.position.y < transform.position.y ? hitKnockback.y : -hitKnockback.y; // ���� ��ֹ����� ���ʿ� �־����� ��������, �ݴ�� �Ʒ������� Ʀ

                rigid2D.velocity = knockback; // �ӵ��� �˹� ����
                break;

            case ObstacleType.Chemical: // ȭ�� Ÿ�� ������ ���
                sprite.color = new Color(0f, 0f, 0f, 0f);
                rigid2D.simulated = false;
                break;

            case ObstacleType.Fall: // ������ ���
                rigid2D.simulated = false;
                break;
        }
    }
    private void Clear()
    {
        Debug.Log("Ŭ����!");
    }
    private void Revive()
    {
        controllable = true;
        alive = true;

        sprite.color = Color.white;
        rigid2D.simulated = true;
    }

    [SerializeField]
    private bool isGround = false;
    public bool IsGround => isGround;
    private void FixedUpdate()
    {
        // �ݶ��̴� �غκп��� ���� üũ
        isGround = Physics2D.OverlapBox(new Vector2(col.bounds.center.x, col.bounds.min.y), new Vector2((col.bounds.extents.x * 2) - 0.05f, 0.1f), 0f, 1 << (int)LAYER.Ground);
        //isGround = Physics2D.OverlapArea(new Vector2(col.bounds.min.x + 0.05f, col.bounds.min.y - 0.05f), new Vector2(col.bounds.max.x - 0.05f, col.bounds.min.y - 0.05f), 1 << (int)LAYER.Ground);
        //Debug.DrawLine(new Vector2(col.bounds.min.x + 0.05f, col.bounds.min.y - 0.05f), new Vector2(col.bounds.max.x - 0.05f, col.bounds.min.y - 0.05f));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isGround && collision.gameObject.layer == (int)LAYER.Ground) // ���߿��� ������ �浹���� ���
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
        if (controllable && isGround) // ���� ������ ���°� ������ ���� ���
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
        if (!isGround || !controllable) // ������ ���� �ʰų� ��Ʈ���� �� �� �� ������ ��ȿȭ��
        {
            Debug.Log("������ �� ���� ����");
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