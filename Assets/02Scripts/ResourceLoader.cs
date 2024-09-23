using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using UnityEngine.WSA;
using Unity.VisualScripting;

public static class ResourceLoader<T> where T : Object
{
    private static Dictionary<KeyValuePair<FolderName, string>, T> resourceCache = new Dictionary<KeyValuePair<FolderName, string>, T>(); // �ҷ��� ���ҽ�  
    public static T ResourceLoad(FolderName folder, string resourceName)
    {
        T value;
        KeyValuePair<FolderName, string> key = new KeyValuePair<FolderName, string>(folder, resourceName);

        if (!resourceCache.TryGetValue(key, out value)) // ĳ�̵� ���ҽ��� ���� ��� �ҷ���
        { // ĳ�̵� ���ҽ��� ���� ���
            value = Resources.Load<T>(Path.Combine(typeof(T).Name, Path.Combine(folder.ToString(), resourceName))); // ���ҽ� �ε�

            resourceCache.Add(key, value); // �ҷ��� ���ҽ� ĳ��
        }

        if (value == null)
            Debug.LogError(resourceName + " �ε� ����");

        return value;
    }
}

public enum FolderName
{
    Player,

    BGM,
    Death,

    Singleton,

    Ect
}