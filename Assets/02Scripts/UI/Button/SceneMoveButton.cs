using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class SceneMoveButton : EventButton
{
    [SerializeField]
    protected SCENE targetScene;

    protected override void ClickEvent()
    {
        GameManager.Inst.SceneMove(targetScene);
    }
}
