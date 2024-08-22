using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Attack))]
public class Arrow : PoolObject
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void HitGround(string tag)
    {
        base.HitGround(tag);

        GetComponent<SpriteRenderer>().color = CustomColor.zero;
        ParticleManager.Inst.PlayParticle(ParticleType.Arrow, transform);

        ReturnToPool();
    }
}