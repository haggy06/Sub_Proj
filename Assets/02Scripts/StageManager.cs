using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField]
    private StageInfo stageInfo;

    private void Awake()
    {
        
    }
}


public struct StageInfo
{
    public int targetTime;
    public int targetJumpCount;

    public Vector2 doorPosition;

    public StageInfo(int targetTime = 60, int targetJumpCount = 5, Vector2 doorPosition = new Vector2())
    {
        this.targetTime = targetTime;
        this.targetJumpCount = targetJumpCount;

        this.doorPosition = doorPosition;
    }
}