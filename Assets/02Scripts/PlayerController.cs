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

    [SerializeField, Range(0f, 90f), Tooltip("정수리 기준 최대 점프 각도")]
    private float angleLock = 60f;
    [SerializeField, Range(0f, 100f)]
    private float jumpPower = 20f;
    public float JumpPower => jumpPower;
    public float AngleLock => angleLock;


    [SerializeField]
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
                                                            ); // 콜라이더 밑부분에서 착지 체크
    }

    public void Jump(Vector2 aim, float powerPercent)
    {
        if (!isGround) // 착지해 있지 않을 시 점프를 무효화함
        {
            Debug.Log("공중 점프는 허용되지 않음");
            return;
        }

        // 중력 방향에 맞춰 조준 각도에 락을 거는 로직 (무중력은 없음)
        if (rigid2D.gravityScale > 0f) // 아래 방향 중력(기본)일 경우
        {
            aim = new Vector2(aim.x, Mathf.Clamp(aim.y, angleVec.y, 1f)).normalized; // 최대 각도보다 낮지 않게 y값을 조절한 뒤 정규화시켜준다.
        }
        else // 위 방향 중력(반중력)일 경우
        {
            aim = new Vector2(aim.x, Mathf.Clamp(aim.y, -1f, -angleVec.y)).normalized; // 최대 각도보다 높지 않게 y값을 조절한 뒤 정규화시켜준다.
        }

        rigid2D.velocity += aim * (powerPercent * jumpPower); // 조정된 점프를 현재 이동속도에 더해준다. (아예 바꿔버리면 미끄러지고 있던 속도 등의 디테일이 사라짐)
    }
    public void AngleRefresh()
    {
        float angle = 90f - angleLock; // 지면과 최소 벡터와의 각도

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