using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    public void PlayParticle()
    {
        if (gameObject.activeInHierarchy)
        {
            particle.Play();

            StopCoroutine("ParticleTracking");
            StartCoroutine("ParticleTracking");
        }
        else
        {
            Debug.LogError("비활성화된 상태에선 파티클 실행 불가능함");
        }
    }
    private IEnumerator ParticleTracking()
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

        StopCoroutine("ParticleTracking");
    }

    public override void ExitFromPool(Transform newParent = null)
    {
        base.ExitFromPool(newParent);

        PlayParticle();
    }
}
