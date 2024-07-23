using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DoorInteract : DetectionBase
{
    [SerializeField]
    private float openRequireTime = 1f; // 문이 열리기까지 소요되는 시간
    [SerializeField]
    private float closeRequireTime = 1f; // 문이 닫히기까지 소요되는 시간

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
        while (!detection) // 감지되지 않고 있는 동안 반복
        {
            if (progress > 0f) // 진척도가 올라 있었을 경우
            {
                progress = Mathf.Clamp(progress - Time.fixedDeltaTime / closeRequireTime, 0f, 1f); // 진척도 하락(최소 0%)

                DoorClosing();
            }
            else // 진척도가 0 이하가 되었을 경우
            {
                StopCoroutine("DoorCloseCoroutine");
            }

            yield return YieldReturn.waitForFixedUpdate;
        }
    }
    protected IEnumerator DoorOpenCoroutine()
    {
        while (detection) // 감지되고 있는 동안 반복
        {
            if (GameManager.Inst.GameStatus == GameStatus.Play && PlayerController.Inst.IsGround) // 플레이 중이고 플레이어가 착지해 있을 경우
            {
                progress += Time.fixedDeltaTime / openRequireTime; // 진척도 상승
                DoorOpening();

                if (progress >= 1f) // 진척도가 100% 이상 찼을 경우
                {
                    DoorOpenComplete();

                    GetComponent<Collider2D>().enabled = false; // 계속 도는 걸 막기 위해 콜라이더 끔
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
