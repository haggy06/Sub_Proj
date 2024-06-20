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
            yield return YieldReturn.WaitForSeconds(cycle);

            Run();
        }
    }

    protected abstract void Run();
}
