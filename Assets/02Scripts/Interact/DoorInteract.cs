using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DoorInteract : DetectionBase
{
    [SerializeField]
    private float openRequireTime = 1f; // ���� ��������� �ҿ�Ǵ� �ð�
    [SerializeField]
    private float closeRequireTime = 1f; // ���� ��������� �ҿ�Ǵ� �ð�

    [SerializeField, Range(0f, 1f)]
    private float progress = 0f;

    protected override void HitGround(string tag)
    {

    }

    protected override void DetectionStart()
    {
        StartCoroutine("DoorOpenCoroutine");
    }
    protected override void DetectionEnd()
    {
        StartCoroutine("DoorCloseCoroutine");
    }

    protected IEnumerator DoorCloseCoroutine()
    {
        while (!detection) // �������� �ʰ� �ִ� ���� �ݺ�
        {
            if (progress > 0f) // ��ô���� �ö� �־��� ���
            {
                progress = Mathf.Clamp(progress - Time.fixedDeltaTime / closeRequireTime, 0f, 1f); // ��ô�� �϶�(�ּ� 0%)

                DoorClosing();
            }
            else // ��ô���� 0 ���ϰ� �Ǿ��� ���
            {
                StopCoroutine("DoorCloseCoroutine");
            }

            yield return YieldReturn.waitForFixedUpdate;
        }
    }
    protected IEnumerator DoorOpenCoroutine()
    {
        while (detection) // �����ǰ� �ִ� ���� �ݺ�
        {
            if (GameManager.Inst.GameStatus == GameStatus.Play && PlayerController.Inst.IsGround) // �÷��� ���̰� �÷��̾ ������ ���� ���
            {
                progress += Time.fixedDeltaTime / openRequireTime; // ��ô�� ���
                DoorOpening();

                if (progress >= 1f) // ��ô���� 100% �̻� á�� ���
                {
                    DoorOpenComplete();

                    GetComponent<Collider2D>().enabled = false; // ��� ���� �� ���� ���� �ݶ��̴� ��
                    StopCoroutine("DoorOpenCoroutine");
                }
            }

            yield return YieldReturn.waitForFixedUpdate;
        }
    }

    protected abstract void DoorOpening();
    protected abstract void DoorClosing();
    protected virtual void DoorOpenComplete()
    {
        PlayerController.Inst.DoorInteract(transform);
    }
}
