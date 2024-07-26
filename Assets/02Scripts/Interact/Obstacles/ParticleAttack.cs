using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem), typeof(Attack))]
public class ParticleAttack : PoolObject
{
    ParticleSystem particle;
    Collider2D col;

    [Header("ParticleAttack")]

    [SerializeField]
    private float detectionStartDelay = 0.2f;
    [SerializeField]
    private float detectionDuration = 1f;

    protected override void Awake()
    {
        base.Awake();

        particle = GetComponent<ParticleSystem>();
        col = GetComponent<Collider2D>();

        lifeTime = particle.main.duration + particle.main.startLifetime.constantMax;
    }

    public override void ExitFromPool(Transform newParent = null)
    {
        base.ExitFromPool(newParent);

        StopCoroutine("ParticleDetectionCor");
        StartCoroutine("ParticleDetectionCor");
    }
    public override void ReturnToPool()
    {
        base.ReturnToPool();

        StopCoroutine("ParticleDetectionCor");
    }

    private IEnumerator ParticleDetectionCor()
    {
        yield return YieldReturn.WaitForSeconds(detectionStartDelay);

        col.enabled = true;

        yield return YieldReturn.WaitForSeconds(detectionDuration);

        col.enabled = false;
    }
}
