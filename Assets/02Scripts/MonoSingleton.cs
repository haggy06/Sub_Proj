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
            if (instance == null) // ���� �ν��Ͻ��� �Ҵ��� �� �Ǿ��� ���
            {
                try
                {
                    instance = Resources.Load<T>(Path.Combine("MonoSingletons", typeof(T).Name)); // Resources ������ MonoSingleton �������� T�� �ҷ��´�.
                }
                catch (NullReferenceException) // �ε忡 �������� ���
                {
                    Debug.LogError(typeof(T).Name + " �ε忡 ������. Resources/MonoSingletons ������ " + typeof(T).Name + "�̶�� �������� �ִ��� Ȯ�����ּ���.");
                    return null;
                }
            }

            return instance;
        }
    }
    protected virtual void Awake()
    {
        if (instance == null || instance == this) // ���� �ν��Ͻ��� �Ҵ��� �� �Ǿ��ų� �� ������Ʈ�� �ν��Ͻ��� ���
        {
            SetInstanceToThis();

            if (transform.parent != null && transform.root != null) // �� ������Ʈ�� �ڽ����� �����Ǿ� ���� ���
            {
                DontDestroyOnLoad(transform.root.gameObject); // �ֻ��� �θ� �Ҹ� ó���Ѵ�.
            }
            else
            {
                DontDestroyOnLoad(gameObject); // �� ������Ʈ�� �Ҹ� ó���Ѵ�.
            }

            SceneManager.activeSceneChanged += SceneChanged; // ���� ����� ������ SceneChanged�� ����ǵ��� �Ѵ�.
        }
        else // �̹� �ν��Ͻ��� �Ҵ��� �Ǿ� ���� ���
        {
            Debug.Log(instance + "�� �̹� instance�� �־� �� ������Ʈ�� ������");
            Destroy(gameObject); // �ڡ���
        }
    }

    // todo : �޼���� ���� �ֱ�
    protected abstract void SetInstanceToThis(); // instance = this;
    protected abstract void SceneChanged(Scene replacedScene, Scene newScene);
}
