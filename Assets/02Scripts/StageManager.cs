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
        curJewelry = 0;
    }

    [SerializeField]
    private int goalJewelry;
    [SerializeField]
    private int curJewelry;
    public void GetJewelry()
    {
        curJewelry++;

        if (curJewelry >= goalJewelry) // 목표치의 보석을 먹었을 경우
        {
            GameManager.Inst.IsJewelryGet = true;
        }
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