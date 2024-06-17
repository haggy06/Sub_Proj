using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using UnityEngine.SceneManagement;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;
    public static T Inst
    {
        get
        {
            if (instance == null) // 아직 인스턴스가 할당이 안 되었을 경우
            {
                try
                {
                    instance = Resources.Load<T>(Path.Combine("MonoSingletons", typeof(T).Name)); // Resources 폴더의 MonoSingleton 폴더에서 T를 불러온다.
                }
                catch (NullReferenceException) // 로드에 실패했을 경우
                {
                    Debug.LogError(typeof(T).Name + " 로드에 실패함. Resources/MonoSingletons 폴더에 " + typeof(T).Name + "이라는 프리팹이 있는지 확인해주세요.");
                    return null;
                }
            }

            return instance;
        }
    }
    protected virtual void Awake()
    {
        if (instance == null || instance == this) // 아직 인스턴스가 할당이 안 되었거나 이 오브젝트가 인스턴스일 경우
        {
            SetInstanceToThis();

            if (transform.parent != null && transform.root != null) // 이 오브젝트가 자식으로 지정되어 있을 경우
            {
                DontDestroyOnLoad(transform.root.gameObject); // 최상위 부모를 불멸 처리한다.
            }
            else
            {
                DontDestroyOnLoad(gameObject); // 이 오브젝트를 불멸 처리한다.
            }

            SceneManager.activeSceneChanged += SceneChanged; // 씬이 변경될 때마다 SceneChanged가 실행되도록 한다.
        }
        else // 이미 인스턴스가 할당이 되어 있을 경우
        {
            Debug.Log(instance + "가 이미 instance에 있어 이 오브젝트는 삭제함");
            Destroy(gameObject); // 자★폭
        }
    }

    // todo : 메서드용 툴팁 넣기
    protected abstract void SetInstanceToThis(); // instance = this;
    protected abstract void SceneChanged(Scene replacedScene, Scene newScene);
}
