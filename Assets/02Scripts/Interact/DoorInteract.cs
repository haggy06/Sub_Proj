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
    private float openRequireTime = 1f; // ���� ��������� �ҿ�Ǵ� �ð�
    [SerializeField]
    private float closeRequireTime = 1f; // ���� ��������� �ҿ�Ǵ� �ð�

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

                if (detectioning) // ���� ���� ��
                {

                }
                else // ���� ���� ��
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
            if (censor.TryGetComponent<PlayerInteract>(out _) && PlayerController.Inst.IsGround) // �÷��̾� ������ �����߰� �÷��̾ ������ ���� ���
            {
                DoorOpening();
                progress += openRequireTime * Time.fixedDeltaTime; // ��ô�� ���
                if (Mathf.Approximately(progress, 1f) || progress > 1f) // ��ô���� 1 �̻��� �Ǿ��� ���
                {
                    DoorOpenComplete();
                    //GetComponent<Collider2D>().enabled = true;
                }
                return; // ���� �ϰ� �޼ҵ� ����
            }
        }

        if (progress > 0f) // �÷��̾� ������ �����ߴµ� ��ô���� �ö� ���� ���
        {
            Debug.Log("��ô�� ������");
            progress -= Mathf.Clamp(closeRequireTime * Time.fixedDeltaTime, 0f, 1f); // ��ô�� ����
        }
    }

    protected abstract void DoorOpening();
    protected abstract void DoorOpenComplete();
}
