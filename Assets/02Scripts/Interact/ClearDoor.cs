using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ClearDoor : MonoBehaviour
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

    [SerializeField,Range(0f, 1f)]
    private float progress = 0f;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
        col.enabled = false;
    }
    private void FixedUpdate()
    {
        Collider2D[] collisions = Physics2D.OverlapBoxAll(transform.position, censorSize, 1 << (int)LAYER.Censor);
        foreach (Collider2D censor in collisions)
        {
            if (censor.TryGetComponent<PlayerInteract>(out _))
            {
                progress += openRequireTime * Time.fixedDeltaTime; // 진척도 상승
                if (Mathf.Approximately(progress, 1f) || progress > 1f) // 진척도가 1 이상이 되었을 경우
                {
                    GetComponent<Collider2D>().enabled = true;
                }

                return; // 플레이어 감지에 성공했을 경우 할일 하고 메소드 종료
            }
        }

        if (progress > 0f) // 플레이어 감지에 실패했는데 진척도가 올라 있을 경우
        {
            Debug.Log("진척도 초기화");
            progress = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerInteract>(out PlayerInteract player)) // 플레이어가 나갔을 경우
        {
            progress = 0; // 진척도 초기화
        }
    }
}
