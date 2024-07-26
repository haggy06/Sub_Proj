using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class ParticleObject : PoolObject
{
    [SerializeField]
    private ParticleType particleType;
    public ParticleType ParticleType => particleType;

    private ParticleSystem particle;
    protected override void Awake()
    {
        base.Awake();

        particle = GetComponent<ParticleSystem>();

        SceneManager.activeSceneChanged += ClearParticle;
    }
    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= ClearParticle;
    }
    private void ClearParticle(Scene replacedScene, Scene newScene)
    {
        particle.Stop();
        particle.Clear();
    }

    private Transform target;
    public void Follow(Transform target)
    {
        this.target = target;
    }
    private void FixedUpdate()
    {
        if (target != null)
            transform.position = target.position;
    }

    public void FollowOFF()
    {
        target = null;
        particle.Stop();
    }

    public void PlayParticle()
    {
        if (gameObject.activeInHierarchy)
        {
            particle.Play();

            StopCoroutine("ParticleReturn");
            StartCoroutine("ParticleReturn");
        }
        else
        {
            Debug.Log("비활성화된 상태에선 파티클 실행 불가능함");
        }
    }
    private IEnumerator ParticleReturn()
    {
        yield return YieldReturn.WaitForSeconds(particle.main.duration + particle.main.startLifetime.constantMax);

        if (!isReturned)
        {
            if (gameObject.activeInHierarchy)
            {
                ReturnToPool();
            }
            else
            {
                Debug.Log("고립됨");
            }
        }
    }

    public override void ReturnToPool()
    {
        base.ReturnToPool();

        target = null;
        StopCoroutine("ParticleReturn");
    }

    public override void ExitFromPool(Transform newParent = null)
    {
        base.ExitFromPool(newParent);

        PlayParticle();
    }
}
