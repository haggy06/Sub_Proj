using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class DoorInteract : DetectionBase
{
    [SerializeField]
    private float openRequireTime = 1f; // ���� ��������� �ҿ�Ǵ� �ð�
    [SerializeField]
    private float closeRequireTime = 1f; // ���� ��������� �ҿ�Ǵ� �ð�

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
        while (!detection) // �������� �ʰ� �ִ� ���� �ݺ�
        {
            if (progress > 0f) // ��ô���� �ö� �־��� ���
            {
                Progress = Mathf.Clamp(progress - Time.fixedDeltaTime / closeRequireTime, 0f, 1f); // ��ô�� �϶�(�ּ� 0%)
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
        openSound.Play();
        while (detection) // �����ǰ� �ִ� ���� �ݺ�
        {
            if (GameManager.Inst.GameStatus == GameStatus.Play && PlayerController.Inst.IsGround && !PlayerController.Inst.Aiming) // �÷��� ���̰� �÷��̾ ������ ������ �����ϰ� ���� ���� ���
            {
                openSound.UnPause();
                Progress += Time.fixedDeltaTime / openRequireTime; // ��ô�� ���
                DoorOpening();

                if (progress >= 1f) // ��ô���� 100% �̻� á�� ���
                {
                    DoorOpenComplete();

                    GetComponent<Collider2D>().enabled = false; // ��� ���� �� ���� ���� �ݶ��̴� ��
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
