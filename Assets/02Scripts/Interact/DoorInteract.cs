using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DoorInteract : PlayerDetection
{
    [SerializeField]
    private float openRequireTime = 1f; // ���� ��������� �ҿ�Ǵ� �ð�
    [SerializeField]
    private float closeRequireTime = 1f; // ���� ��������� �ҿ�Ǵ� �ð�

    [SerializeField, Range(0f, 1f)]
    private float progress = 0f;

    private void FixedUpdate()
    {
        if (detection && PlayerController.Inst.IsGround) // ���� ���̰� �÷��̾ ������ ���� ���
        {
            progress += Time.fixedDeltaTime / openRequireTime; // ��ô�� ���
            DoorOpening();

            if (progress >= 1f) // ��ô���� 100% �̻� á�� ���
            {
                DoorOpenComplete();

                enabled = false; // ���� �� DoorOpenComplete �޼ҵ尡 ȣ��Ǵ� �� �����ϱ� ���� ��ũ��Ʈ ��
            }
        }
        else // ���� ���� �ƴ� ���
        {
            if (progress > 0f) // ��ô���� �ö� �־��� ���
            {
                progress = Mathf.Clamp(progress - Time.fixedDeltaTime / closeRequireTime, 0f, 1f); // ��ô�� �϶�(�ּ� 0%)
                DoorClosing();
            }
        }
    }

    protected abstract void DoorOpening();
    protected abstract void DoorClosing();
    protected virtual void DoorOpenComplete()
    {
        PlayerController.Inst.DoorInteract(transform);
    }
}
