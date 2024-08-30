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
    private float Progress
    {
        get => progress;
        set
        {
            progress = value;
            lattice.localPosition = Vector2.up * value * 3f;
        }
    }

    protected bool isComplete = false;
    private Transform lattice;
    protected virtual void Awake()
    {
        lattice = transform.GetChild(0).GetChild(0);

        Progress = 0f;
    }

    protected override void HitGround(string tag)
    {

    }

    protected override void DetectionStart()
    {
        if (progress < 1f)
            StartCoroutine("DoorOpenCoroutine");
    }
    protected override void DetectionEnd()
    {
        if (!isComplete)
            StartCoroutine("DoorCloseCoroutine");
    }

    protected IEnumerator DoorCloseCoroutine()
    {
        while (!detection) // 감지되지 않고 있는 동안 반복
        {
            if (progress > 0f) // 진척도가 올라 있었을 경우
            {
                Progress = Mathf.Clamp(progress - Time.fixedDeltaTime / closeRequireTime, 0f, 1f); // 진척도 하락(최소 0%)
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
            if (GameManager.Inst.GameStatus == GameStatus.Play && PlayerController.Inst.IsGround && !PlayerController.Inst.Aiming) // 플레이 중이고 플레이어가 착지해 있으며 조준하고 있지 않을 경우
            {
                Progress += Time.fixedDeltaTime / openRequireTime; // 진척도 상승
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
        isComplete = true;
        PlayerController.Inst.DoorInteract(transform);
    }
}
