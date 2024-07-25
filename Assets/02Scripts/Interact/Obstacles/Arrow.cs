using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Attack))]
public class Arrow : BurningObject, I_Projectile
{
    protected override void Awake()
    {
        base.Awake();
    }

    public void Launch(float direction, float speed)
    {
        transform.eulerAngles = Vector3.forward * direction;

        GetComponent<SpriteRenderer>().color = Color.white;

        GetComponent<Rigidbody2D>().velocity = MyCalculator.Deg2Vec(direction) * speed;
    }

    protected override void HitGround(string tag)
    {
        base.HitGround(tag);

        GetComponent<SpriteRenderer>().color = CustomColor.zero;
        ParticleManager.Inst.PlayParticle(ParticleType.Gravel, transform.position, new Vector2(0.1f, 0.1f), transform.eulerAngles.z + 90f);

        ReturnToPool();
    }
}

public interface I_Projectile
{
    public void Launch(float direction, float speed);
}