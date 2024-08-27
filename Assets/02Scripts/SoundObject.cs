using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundObject : EffectObject
{
    [SerializeField]
    private AudioSource audioSource;
    public AudioClip clip
    { 
        set 
        {
            audioSource.clip = value;
            lifeTime = value.length;
        } 
    }
    protected override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
    }
    protected override void SceneChanged(Scene replacedScene, Scene newScene)
    {
        base.SceneChanged(replacedScene, newScene);
        if (audioSource == null)
        {
            Debug.Log("audioSource가 null이 됨");
            audioSource = GetComponent<AudioSource>();
        }
    }
    public override void Init(Transform owner, float rotation)
    {
        base.Init(owner, rotation);
        
    }

    public override void Clear()
    {
        base.Clear();

        audioSource.Stop();
    }
    public override void Play()
    {
        base.Play();

        audioSource.Play();
    }
    public override void ReturnToPool()
    {
        base.ReturnToPool();

        audioSource.Stop(); // gameObject.SetActive(false) 대신 particle.Stop()을 넣었다.
    }
}
