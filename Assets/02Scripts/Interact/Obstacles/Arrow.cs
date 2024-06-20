using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PoolObject))]
public class Arrow : Obstacle, I_Projectile
{
    [SerializeField]
    private ParticleSystem breakParticle;
    public void Launch(Vector2 direction, float speed)
    {
        transform.eulerAngles = Vector3.forward * MyCalculator.Vec2Deg(direction);

        GetComponent<SpriteRenderer>().color = Color.white;
        GetComponent<Rigidbody2D>().velocity = direction * speed;
    }

    protected override void HitGround()
    {
        base.HitGround();

        GetComponent<SpriteRenderer>().color = CustomColor.zero;
        breakParticle.Play();

        GetComponent<PoolObject>().ReturnToPool();
    }
}

public interface I_Projectile
{
    public void Launch(Vector2 direction, float speed);
}