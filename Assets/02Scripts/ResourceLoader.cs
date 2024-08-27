using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

public static class ResourceLoader
{
    public static Sprite SpriteLoad(FolderName folder, string resourceName)
    {
        Sprite value = Resources.Load<Sprite>(Path.Combine("Sprite", Path.Combine(folder.ToString(), resourceName)));
        if (value == null)
            Debug.LogError(resourceName + " 로드 실패");

        return value;
    }
    public static Sprite SpriteLoad(FolderName folder, string resourceName, int index)
    {
        Sprite value = Resources.Load<Sprite>(Path.Combine("Sprite", Path.Combine(folder.ToString(), resourceName + index)));
        if (value == null)
            Debug.LogError(resourceName + index + " 로드 실패");

        return value;
    }

    public static AudioClip AudioLoad(FolderName folder, string resourceName)
    {
        AudioClip value = Resources.Load<AudioClip>(Path.Combine("Audio", Path.Combine(folder.ToString(), resourceName)));
        if (value == null)
            Debug.LogError(resourceName + " 로드 실패");

        return value;
    }
    public static AudioClip AudioLoad(FolderName folder, string resourceName, int index)
    {
        AudioClip value = Resources.Load<AudioClip>(Path.Combine("Audio", Path.Combine(folder.ToString(), resourceName + index)));
        if (value == null)
            Debug.LogError(resourceName + index + " 로드 실패");

        return value;
    }

    public static T PrefabLoad<T>() where T : Object
    {
        T value = Resources.Load<T>(Path.Combine("Prefab", typeof(T).Name));
        if (value == null)
            Debug.LogError(typeof(T).Name + " 로드 실패");

        return value;
    }
    public static T PrefabLoad<T>(string resourceName) where T : Object
    {
        T value = Resources.Load<T>(Path.Combine("Prefab", resourceName));
        if (value == null)
            Debug.LogError(resourceName + " 로드 실패");

        return value;
    }
}

public enum FolderName
{
    Player,

    BGM,
    Ect,

    Background,
    Icon,
}