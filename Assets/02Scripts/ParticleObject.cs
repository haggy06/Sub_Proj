using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class ParticleObject : EffectObject
{
    private ParticleSystem particle;
    public ParticleSystem Particle => particle;
    protected override void Awake()
    {
        base.Awake();

        particle = GetComponent<ParticleSystem>();
        lifeTime = particle.main.duration + particle.main.startLifetime.constantMax;
    }
    protected override void SceneChanged(Scene replacedScene, Scene newScene)
    {
        base.SceneChanged(replacedScene, newScene);
        if (particle == null)
        {
            Debug.Log("audioSource가 null이 됨");
            particle = GetComponent<ParticleSystem>();
        }
    }

    public override void Clear()
    {
        base.Clear();

        particle.Stop();
        particle.Clear();
    }
    public override void Play()
    {
        base.Play();

        particle.Play();
    }
    public override void ReturnToPool()
    {
        base.ReturnToPool();

        particle.Stop(); // gameObject.SetActive(false) 대신 particle.Stop()을 넣었다.
    }
}
