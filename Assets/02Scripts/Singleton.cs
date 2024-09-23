using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using UnityEngine.SceneManagement;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;
    public static T Inst
    {
        get
        {
            if (instance == null) // 아직 인스턴스가 할당이 안 되었을 경우
            {                
                instance = (T)FindObjectOfType(typeof(T));
                if (instance == null) // 씬 내에 아예 인스턴스가 없을 경우
                try
                {
                    T inst = Instantiate(ResourceLoader<GameObject>.ResourceLoad(FolderName.Singleton, typeof(T).Name).GetComponent<T>()); // Resources 폴더에서 개체 소환
                    instance = inst;
                }
                catch (NullReferenceException) // 로드에 실패했을 경우
                {
                    Debug.LogError(typeof(T).Name + " 로드에 실패함. Resources/GameObject/Singleton 폴더에 " + typeof(T).Name + "이라는 프리팹이 있는지 확인해주세요.");
                    return null;
                }
            }

            return instance;
        }
    }

    protected virtual void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneChanged;
    }
    protected virtual void Awake()
    {
        if (Inst == this) // 이 오브젝트가 싱글톤 인스턴스일 경우
        {
            DontDestroyOnLoad(transform.root.gameObject); // 최상위 오브젝트를 불멸 처리한다.

            SceneManager.activeSceneChanged += SceneChanged; // 씬이 변경될 때마다 SceneChanged가 실행되도록 한다.
        }
        else // 이미 인스턴스가 할당이 되어 있을 경우
        {
            Debug.Log(instance + "가 이미 instance에 있어 이 오브젝트는 삭제함");
            Destroy(gameObject); // 자★폭
        }
    }

    /// <summary>씬 변경 시 실행되는 메소드</summary>///
    protected abstract void SceneChanged(Scene replacedScene, Scene newScene);
}
