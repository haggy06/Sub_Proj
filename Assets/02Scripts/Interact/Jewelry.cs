using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jewelry : DetectionBase
{
    protected override void DetectionEnd()
    {

    }

    protected override void DetectionStart()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        ParticleManager.Inst.PlayParticle(ParticleType.Gem, transform.position, Vector2.one);

        transform.root.GetComponent<StageManager>().GetJewelry();
    }

    protected override void HitGround(string tag)
    {

    }
}
