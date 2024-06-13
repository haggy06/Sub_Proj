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
    private float openRequireTime = 1f; // ���� ��������� �ҿ�Ǵ� �ð�

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
                progress += openRequireTime * Time.fixedDeltaTime; // ��ô�� ���
                if (Mathf.Approximately(progress, 1f) || progress > 1f) // ��ô���� 1 �̻��� �Ǿ��� ���
                {
                    GetComponent<Collider2D>().enabled = true;
                }

                return; // �÷��̾� ������ �������� ��� ���� �ϰ� �޼ҵ� ����
            }
        }

        if (progress > 0f) // �÷��̾� ������ �����ߴµ� ��ô���� �ö� ���� ���
        {
            Debug.Log("��ô�� �ʱ�ȭ");
            progress = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerInteract>(out PlayerInteract player)) // �÷��̾ ������ ���
        {
            progress = 0; // ��ô�� �ʱ�ȭ
        }
    }
}
