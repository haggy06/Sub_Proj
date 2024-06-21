using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CycleObstacle : MonoBehaviour
{
    [SerializeField]
    protected bool playOnAwake = true;
    [SerializeField]
    protected float startDelay = 0f;
    [SerializeField]
    protected float cycle = 2f;

    [Space(5)]
    [SerializeField, Tooltip("Ʈ���� ����̸� true, ���� ������ ����̸� false")]
    private bool isTrigger= false;
    [SerializeField]
    protected float maintainTime = 0f;

    protected bool repeat = false;
    public bool Repeat
    {
        get => repeat;
        set
        {
            if (repeat != value)
            {
                repeat = value;
                if (repeat)
                {
                    StartCoroutine("LaunchCoroutine");
                }
                else
                {
                    StopCoroutine("LaunchCoroutine");
                    RunStop();
                }
            }
        }
    }
    protected void Awake()
    {
        Repeat = playOnAwake;
    }
    protected IEnumerator LaunchCoroutine()
    {
        yield return YieldReturn.WaitForSeconds(startDelay);

        while (repeat)
        {
            Run();

            if (!isTrigger) // Ʈ���� ����� �ƴ� ���
            {
                yield return YieldReturn.WaitForSeconds(maintainTime);

                RunStop();
            }

            yield return YieldReturn.WaitForSeconds(cycle);
        }
    }

    protected abstract void Run();
    protected abstract void RunStop();
}
