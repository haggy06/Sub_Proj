using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class YieldReturn
{
    public readonly static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    private static Dictionary<float, WaitForSeconds> waitForSeconds = new Dictionary<float, WaitForSeconds>();
    public static WaitForSeconds WaitForSeconds(float sec)
    {
        WaitForSeconds value;
        if (!waitForSeconds.TryGetValue(sec, out value))
        {
            value = new WaitForSeconds(sec);
            if (Mathf.Approximately(sec / 0.25f, 0f)) // 0.25의 배수의 시간일 경우엔 waitForSeconds에 저장한다.(다른 건 자주 안 나올 것 같아서 따로 저장 안해두는 게 나을 듯)
            {
                waitForSeconds.Add(sec, value);
            }
        }

        return value;
    }

    private static Dictionary<float, WaitForSecondsRealtime> waitForSecondsRealtime = new Dictionary<float, WaitForSecondsRealtime>();
    public static WaitForSecondsRealtime WaitForSecondsRealtime(float sec)
    {
        WaitForSecondsRealtime value;
        if (!waitForSecondsRealtime.TryGetValue(sec, out value))
        {
            value = new WaitForSecondsRealtime(sec);
            if (Mathf.Approximately(sec / 0.25f, 0f)) // 0.25의 배수의 시간일 경우엔 waitForSeconds에 저장한다.(다른 건 자주 안 나올 것 같아서 따로 저장 안해두는 게 나을 듯)
            {
                waitForSecondsRealtime.Add(sec, value);
            }
        }

        return value;
    }
}
