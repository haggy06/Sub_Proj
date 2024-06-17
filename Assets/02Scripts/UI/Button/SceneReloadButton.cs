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
        if (buildIndex < 0 || buildIndex > (int)SCENE.Ending) // buildIndex�� �����̰ų� �ش��ϴ� ���� ���� ���
        {
            Debug.LogError("�ش�Ǵ� BuildIndex�� ����.");
        }
        else
        {
            GameManager.Inst.SceneMove((SCENE)buildIndex);
        }
    }
}
