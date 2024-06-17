using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DoorInteract : MonoBehaviour
{
    [SerializeField]
    private Vector2 censorSize = new Vector2(1.5f, 0.5f);
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position, censorSize);
    }

    [SerializeField]
    private float openRequireTime = 1f; // 문이 열리기까지 소요되는 시간
    [SerializeField]
    private float closeRequireTime = 1f; // 문이 닫히기까지 소요되는 시간

    [SerializeField, Range(0f, 1f)]
    private float progress = 0f;

    private bool detectioning = false;
    public bool Detectioning
    {
        get => detectioning;
        protected set
        {
            if (detectioning != value)
            {
                detectioning = value;

                if (detectioning) // 감지 시작 시
                {

                }
                else // 감지 끊길 시
                {

                }
            }
        }
    }
    private void FixedUpdate()
    {
        Collider2D[] collisions = Physics2D.OverlapBoxAll(transform.position, censorSize, 1 << (int)LAYER.Censor);
        foreach (Collider2D censor in collisions)
        {
            if (censor.TryGetComponent<PlayerInteract>(out _) && PlayerController.Inst.IsGround) // 플레이어 감지에 성공했고 플레이어가 착지해 있을 경우
            {
                DoorOpening();
                progress += openRequireTime * Time.fixedDeltaTime; // 진척도 상승
                if (Mathf.Approximately(progress, 1f) || progress > 1f) // 진척도가 1 이상이 되었을 경우
                {
                    DoorOpenComplete();
                    //GetComponent<Collider2D>().enabled = true;
                }
                return; // 할일 하고 메소드 종료
            }
        }

        if (progress > 0f) // 플레이어 감지에 실패했는데 진척도가 올라 있을 경우
        {
            Debug.Log("진척도 감소중");
            progress -= Mathf.Clamp(closeRequireTime * Time.fixedDeltaTime, 0f, 1f); // 진척도 감소
        }
    }

    protected abstract void DoorOpening();
    protected abstract void DoorOpenComplete();
}
