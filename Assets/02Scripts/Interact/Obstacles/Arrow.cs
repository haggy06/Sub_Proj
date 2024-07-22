using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Attack))]
public class Arrow : PoolObject, I_Projectile
{
    private Attack attack;
    private void Awake()
    {
        attack = GetComponent<Attack>();

        attack.HitGroundEvent += HitGround;
    }

    public void Launch(float direction, float speed)
    {
        transform.eulerAngles = Vector3.forward * direction;

        GetComponent<SpriteRenderer>().color = Color.white;

        GetComponent<Rigidbody2D>().velocity = MyCalculator.Deg2Vec(direction) * speed;
    }

    private void HitGround()
    {
        GetComponent<SpriteRenderer>().color = CustomColor.zero;
        ParticleManager.Inst.PlayParticle(ParticleType.Gravel, transform.position, new Vector2(0.1f, 0.1f), transform.eulerAngles.z + 90f);

        GetComponent<PoolObject>().ReturnToPool();
    }
}

public interface I_Projectile
{
    public void Launch(float direction, float speed);
}