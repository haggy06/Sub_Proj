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
            if (Mathf.Approximately(sec / 0.25f, 0f)) // 0.25�� ����� �ð��� ��쿣 waitForSeconds�� �����Ѵ�.(�ٸ� �� ���� �� ���� �� ���Ƽ� ���� ���� ���صδ� �� ���� ��)
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
            if (Mathf.Approximately(sec / 0.25f, 0f)) // 0.25�� ����� �ð��� ��쿣 waitForSeconds�� �����Ѵ�.(�ٸ� �� ���� �� ���� �� ���Ƽ� ���� ���� ���صδ� �� ���� ��)
            {
                waitForSecondsRealtime.Add(sec, value);
            }
        }

        return value;
    }
}
