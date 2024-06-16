using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayUI : MonoBehaviour
{
    [SerializeField]
    private Image jewlryCheck;
    [SerializeField]
    private TextMeshProUGUI jumpCount;
    [SerializeField]
    private TextMeshProUGUI timer;

    private void Awake()
    {
        SceneManager.activeSceneChanged += SceneChangeEvent; 
    }

    private void SceneChangeEvent(Scene beforeScene, Scene newScene)
    {

    }
}
