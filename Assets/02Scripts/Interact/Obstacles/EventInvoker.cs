using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventInvoker : MonoBehaviour
{
    [SerializeField]
    protected EntityEvent targetEvent;

    [Space(5)]
    [SerializeField]
    protected bool playOnAwake = true;
    [SerializeField]
    protected float startDelay = 0f;
    [SerializeField]
    protected float cycle = 2f;
    [SerializeField]
    protected float maintainTime = 0.5f;

    public float StartDelay => startDelay;
    public float Cycle => cycle;
    public float MaintainTime => maintainTime;

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
                    if (!targetEvent.IsTrigger && !cycleComplete)
                    {
                        targetEvent.RunStop();
                    }
                }
            }
        }
    }
    protected void Awake()
    {
        Repeat = playOnAwake;
    }

    protected bool cycleComplete = false;
    protected IEnumerator LaunchCoroutine()
    {
        yield return YieldReturn.WaitForSeconds(startDelay);

        while (repeat)
        {
            cycleComplete = false;
            targetEvent.Run();

            if (!targetEvent.IsTrigger) // 트리거 방식이 아닐 경우
            {
                yield return YieldReturn.WaitForSeconds(maintainTime);

                targetEvent.RunStop();
            }
            cycleComplete = true;

            yield return YieldReturn.WaitForSeconds(cycle);
        }
    }
}
