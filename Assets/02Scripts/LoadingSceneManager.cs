using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.LoadScene((int)GameManager.Inst.TargetScene);
    }
}
