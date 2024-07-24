using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObject : PoolObject
{
    [SerializeField]
    private ParticleType particleType;
    public ParticleType ParticleType => particleType;

    private ParticleSystem particle;
    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }
    public void PlayParticle()
    {
        particle.Play();

        StopCoroutine("ParticleTracking");
        StartCoroutine("ParticleTracking");
    }
    private IEnumerator ParticleTracking()
    {
        yield return YieldReturn.WaitForSeconds(particle.main.duration + particle.main.startLifetime.constantMax);

        if (gameObject.activeInHierarchy)
            ReturnToPool();
    }

    public override void ReturnToPool()
    {
        base.ReturnToPool();

        StopCoroutine("ParticleTracking");
    }

    public override void ExitFromPool(Transform newParent = null)
    {
        transform.parent = newParent;
    }
}
