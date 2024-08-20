using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Attack))]
public class Arrow : PoolObject//, I_Projectile
{
    protected override void Awake()
    {
        base.Awake();
    }
    /*
    public void Launch(float direction, float speed)
    {
        transform.eulerAngles = Vector3.forward * direction;

        GetComponent<SpriteRenderer>().color = Color.white;

        GetComponent<Rigidbody2D>().velocity = MyCalculator.Deg2Vec(direction) * speed;
    }
    */
    protected override void HitGround(string tag)
    {
        base.HitGround(tag);

        GetComponent<SpriteRenderer>().color = CustomColor.zero;
        ParticleManager.Inst.PlayParticle(ParticleType.Arrow, transform);

        ReturnToPool();
    }
}

public interface I_Projectile
{
    public void Launch(float direction, float speed);
}