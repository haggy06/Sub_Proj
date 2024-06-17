using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class SceneReloadButton : EventButton
{
    [SerializeField]
    private int sceneOffset = 0;

    protected override void ClickEvent()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex + sceneOffset;
        if (buildIndex < 0 || buildIndex > (int)SCENE.Ending) // buildIndex가 음수이거나 해당하는 씬이 없을 경우
        {
            Debug.LogError("해당되는 BuildIndex가 없음.");
        }
        else
        {
            GameManager.Inst.SceneMove((SCENE)buildIndex);
        }
    }
}
