using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionEvent : DetectionBase
{
    [SerializeField]
    private EntityEvent targetEvent;

    [Space(5)]
    [SerializeField]
    private bool autoRunStop = true;
    [SerializeField]
    private float delayTime = 0.5f;
    [SerializeField]
    private float maintainTime = 0.5f;

    protected override void DetectionEnd()
    {

    }

    protected override void DetectionStart()
    {
        StartCoroutine("DetectionCor");
    }
    private IEnumerator DetectionCor()
    {
        yield return YieldReturn.WaitForSeconds(delayTime);

        targetEvent.Run();

        if (autoRunStop)
        {
            yield return YieldReturn.WaitForSeconds(maintainTime);

            targetEvent.RunStop();
        }
    }

    protected override void HitGround()
    {

    }
}
