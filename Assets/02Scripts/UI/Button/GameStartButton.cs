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
        if (GameManager.Inst.GetTutorialClear())
        {
            targetScene = SCENE.StageSelect;
        }
        else
        {
            targetScene = SCENE.Tutorial;
        }
    }
}
