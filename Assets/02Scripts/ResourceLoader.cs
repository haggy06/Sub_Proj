using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using UnityEngine.WSA;
using Unity.VisualScripting;

public static class ResourceLoader
{
    private static Dictionary<KeyValuePair<FolderName, string>, Sprite> spriteCache = new Dictionary<KeyValuePair<FolderName, string>, Sprite>(); // 불러온 리소스  
    public static Sprite SpriteLoad(FolderName folder, string resourceName)
    {
        Sprite value;
        KeyValuePair<FolderName, string> key = new KeyValuePair<FolderName, string>(folder, resourceName);

        if (!spriteCache.TryGetValue(key, out value)) // 캐싱된 리소스가 있을 경우 불러옴
        { // 캐싱된 리소스가 없을 경우
            value = Resources.Load<Sprite>(Path.Combine("Sprite", Path.Combine(folder.ToString(), resourceName))); // 리소스 로드

            spriteCache.Add(key, value); // 불러온 리소스 캐싱
        }

        if (value == null)
            Debug.LogError(resourceName + " 로드 실패");

        return value;
    }

    private static Dictionary<KeyValuePair<FolderName, string>, AudioClip> audioCache = new Dictionary<KeyValuePair<FolderName, string>, AudioClip>(); // 불러온 리소스  
    public static AudioClip AudioLoad(FolderName folder, string resourceName)
    {
        AudioClip value;
        KeyValuePair<FolderName, string> key = new KeyValuePair<FolderName, string>(folder, resourceName);

        if (!audioCache.TryGetValue(key, out value)) // 캐싱된 리소스가 있을 경우 불러옴
        { // 캐싱된 리소스가 없을 경우
            value = Resources.Load<AudioClip>(Path.Combine("Audio", Path.Combine(folder.ToString(), resourceName))); // 리소스 로드

            audioCache.Add(key, value); // 불러온 리소스 캐싱
        }

        if (value == null)
            Debug.LogError(resourceName + " 로드 실패");

        return value;
    }

    private static Dictionary<string, GameObject> objectCache = new Dictionary<string, GameObject>(); // 불러온 리소스
    public static GameObject PrefabLoad(string resourceName)
    {
        GameObject value;

        if (!objectCache.TryGetValue(resourceName, out value)) // 캐싱된 리소스가 있을 경우 불러옴
        { // 캐싱된 리소스가 없을 경우
            value = Resources.Load<GameObject>(Path.Combine("Prefab", resourceName)); ; // 리소스 로드

            objectCache.Add(resourceName, value); // 불러온 리소스 캐싱
        }

        if (value == null)
            Debug.LogError(resourceName + " 로드 실패");

        return value;
    }
}

public enum FolderName
{
    Player,

    BGM,
    Death,

    Ect
}