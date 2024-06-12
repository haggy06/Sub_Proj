using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class PlayerController : MonoSingleton<PlayerController>
{
    protected override void SetInstanceToThis()
    {
        instance = this;
    }

    [SerializeField, Range(0f, 90f), Tooltip("������ ���� �ִ� ���� ����")]
    private float angleLock = 60f;
    [SerializeField, Range(0f, 100f)]
    private float jumpPower = 20f;
    public float JumpPower => jumpPower;
    public float AngleLock => angleLock;


    [SerializeField]
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

    public Collider2D Col => col;
    public  Rigidbody2D Rigid2D => rigid2D;
    protected override void Awake()
    {
        base.Awake();

        col = GetComponent<Collider2D>();
        rigid2D = GetComponent<Rigidbody2D>();
        DragManager.MouseUpEvent += Jump;
        AngleRefresh();
    }

    [SerializeField]
    private bool isGround = false;
    public bool IsGround => isGround;
    private void FixedUpdate()
    {
        isGround = Physics2D.OverlapArea(
                                                            new Vector2(col.bounds.min.x + 0.01f, col.bounds.min.y - 0.01f),
                                                            new Vector2(col.bounds.max.x - 0.01f, col.bounds.min.y - 0.01f),
                                                            1 << (int)LAYER.Ground
                                                            ); // �ݶ��̴� �غκп��� ���� üũ
    }

    public void Jump(Vector2 aim, float powerPercent)
    {
        if (!isGround) // ������ ���� ���� �� ������ ��ȿȭ��
        {
            Debug.Log("���� ������ ������ ����");
            return;
        }

        // �߷� ���⿡ ���� ���� ������ ���� �Ŵ� ���� (���߷��� ����)
        if (rigid2D.gravityScale > 0f) // �Ʒ� ���� �߷�(�⺻)�� ���
        {
            aim = new Vector2(aim.x, Mathf.Clamp(aim.y, angleVec.y, 1f)).normalized; // �ִ� �������� ���� �ʰ� y���� ������ �� ����ȭ�����ش�.
        }
        else // �� ���� �߷�(���߷�)�� ���
        {
            aim = new Vector2(aim.x, Mathf.Clamp(aim.y, -1f, -angleVec.y)).normalized; // �ִ� �������� ���� �ʰ� y���� ������ �� ����ȭ�����ش�.
        }

        rigid2D.velocity += aim * (powerPercent * jumpPower); // ������ ������ ���� �̵��ӵ��� �����ش�. (�ƿ� �ٲ������ �̲������� �ִ� �ӵ� ���� �������� �����)
    }
    public void AngleRefresh()
    {
        float angle = 90f - angleLock; // ����� �ּ� ���Ϳ��� ����

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