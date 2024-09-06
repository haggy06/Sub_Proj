using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
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

    protected bool doorLocked = false;
    protected bool isComplete = false;

    private Transform lattice;
    private AudioSource openSound;
    protected virtual void Awake()
    {
        lattice = transform.GetChild(0).GetChild(0);

        openSound = GetComponent<AudioSource>();
        PopupManager.PauseEvent += AudioPause;                         

        Progress = 0f;
    }
    private void OnDestroy()
    {
        PopupManager.PauseEvent -= AudioPause;
    }
    private void AudioPause(bool isPaused)
    {
        openSound.pitch = isPaused ? 0f : 1f;
    }

    protected override void HitGround(string tag)
    {

    }

    protected override void DetectionStart()
    {
        if (!doorLocked && progress < 1f)
            StartCoroutine("DoorOpenCoroutine");
    }
    protected override void DetectionEnd()
    {
        if (!doorLocked && !isComplete)
            StartCoroutine("DoorCloseCoroutine");
    }

    protected IEnumerator DoorCloseCoroutine()
    {
        openSound.Stop();
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
        openSound.Play();
        while (detection) // 감지되고 있는 동안 반복
        {
            if (GameManager.Inst.GameStatus == GameStatus.Play && PlayerController.Inst.IsGround && !PlayerController.Inst.Aiming) // 플레이 중이고 플레이어가 착지해 있으며 조준하고 있지 않을 경우
            {
                openSound.UnPause();
                Progress += Time.fixedDeltaTime / openRequireTime; // 진척도 상승
                DoorOpening();

                if (progress >= 1f) // 진척도가 100% 이상 찼을 경우
                {
                    DoorOpenComplete();

                    GetComponent<Collider2D>().enabled = false; // 계속 도는 걸 막기 위해 콜라이더 끔
                    StopCoroutine("DoorOpenCoroutine");
                }
            }
            else
                openSound.Pause();

            yield return YieldReturn.waitForFixedUpdate;
        }
    }

    protected abstract void DoorOpening();
    protected abstract void DoorClosing();
    protected virtual void DoorOpenComplete()
    {
        openSound.Stop();
        isComplete = true;
        PlayerController.Inst.DoorInteract(transform);
    }
}
