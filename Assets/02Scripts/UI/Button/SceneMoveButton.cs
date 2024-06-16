using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SceneMoveButton : MonoBehaviour
{
    [SerializeField]
    private SCENE targetScene;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => GameManager.Inst.SceneMove(targetScene));
    }
}
