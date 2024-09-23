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
            if (instance == null) // ���� �ν��Ͻ��� �Ҵ��� �� �Ǿ��� ���
            {                
                instance = (T)FindObjectOfType(typeof(T));
                if (instance == null) // �� ���� �ƿ� �ν��Ͻ��� ���� ���
                try
                {
                    T inst = Instantiate(ResourceLoader<GameObject>.ResourceLoad(FolderName.Singleton, typeof(T).Name).GetComponent<T>()); // Resources �������� ��ü ��ȯ
                    instance = inst;
                }
                catch (NullReferenceException) // �ε忡 �������� ���
                {
                    Debug.LogError(typeof(T).Name + " �ε忡 ������. Resources/GameObject/Singleton ������ " + typeof(T).Name + "�̶�� �������� �ִ��� Ȯ�����ּ���.");
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
        if (Inst == this) // �� ������Ʈ�� �̱��� �ν��Ͻ��� ���
        {
            DontDestroyOnLoad(transform.root.gameObject); // �ֻ��� ������Ʈ�� �Ҹ� ó���Ѵ�.

            SceneManager.activeSceneChanged += SceneChanged; // ���� ����� ������ SceneChanged�� ����ǵ��� �Ѵ�.
        }
        else // �̹� �ν��Ͻ��� �Ҵ��� �Ǿ� ���� ���
        {
            Debug.Log(instance + "�� �̹� instance�� �־� �� ������Ʈ�� ������");
            Destroy(gameObject); // �ڡ���
        }
    }

    /// <summary>�� ���� �� ����Ǵ� �޼ҵ�</summary>///
    protected abstract void SceneChanged(Scene replacedScene, Scene newScene);
}
