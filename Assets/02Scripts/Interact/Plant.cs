using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : DetectionBase
{
    [SerializeField]
    private float burningTime = 2f;
    protected override void DetectionEnd()
    {

    }

    protected override void DetectionStart()
    {
        ParticleManager.Inst.PlayParticle(ParticleType.Fire, transform);

        LeanTween.color(gameObject, Color.black, burningTime).setOnComplete(() => GetComponent<SpriteRenderer>().enabled = false);

        ParticleManager.Inst.PlayParticle(ParticleType.Fire, transform.position, transform.localScale, transform.localEulerAngles.z);
    }

    protected override void HitGround()
    {

    }
}
