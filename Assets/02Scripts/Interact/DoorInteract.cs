using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DoorInteract : PlayerDetection
{
    [SerializeField]
    private float openRequireTime = 1f; // 문이 열리기까지 소요되는 시간
    [SerializeField]
    private float closeRequireTime = 1f; // 문이 닫히기까지 소요되는 시간

    [SerializeField, Range(0f, 1f)]
    private float progress = 0f;

    private void FixedUpdate()
    {
        if (detection && PlayerController.Inst.IsGround) // 감지 중이고 플레이어가 착지해 있을 경우
        {
            progress += Time.fixedDeltaTime / openRequireTime; // 진척도 상승
            DoorOpening();

            if (progress >= 1f) // 진척도가 100% 이상 찼을 경우
            {
                DoorOpenComplete();

                enabled = false; // 여러 번 DoorOpenComplete 메소드가 호출되는 걸 방지하기 위해 스크립트 끔
            }
        }
        else // 감지 중이 아닐 경우
        {
            if (progress > 0f) // 진척도가 올라 있었을 경우
            {
                progress = Mathf.Clamp(progress - Time.fixedDeltaTime / closeRequireTime, 0f, 1f); // 진척도 하락(최소 0%)
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
