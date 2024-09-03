using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartButton : SceneMoveButton
{
    private void Awake()
    {
        SetTargetScene();
    }

    public void SetTargetScene()
    {
        if (GameManager.Inst.GetClearInfo(0).stageClear)
        {
            targetScene = SCENE.StageSelect;
        }
        else
        {
            targetScene = SCENE.Tutorial;
        }
    }
}
