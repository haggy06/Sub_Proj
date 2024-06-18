using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField]
    private StageInfo stageInfo;

    private void Awake()
    {
        GameManager.Inst.SetStageInfo(stageInfo);
    }
}

[System.Serializable]
public struct StageInfo
{
    public int goalTime;
    public int goalJumpCount;

    public Vector2 doorPosition;
    public Vector2 startPosition;

    public StageInfo(int goalTime = 60, int goalJumpCount = 5, Vector2 doorPosition = new Vector2(), Vector2 startPosition = new Vector2())
    {
        this.goalTime = goalTime;
        this.goalJumpCount = goalJumpCount;

        this.doorPosition = doorPosition;
        this.startPosition = startPosition;
    }
}