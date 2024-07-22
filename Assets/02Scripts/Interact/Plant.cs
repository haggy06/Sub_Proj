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

        LeanTween.color(gameObject, Color.black, burningTime).setOnComplete(BurnOut);
    }
    private void BurnOut()
    {
        ParticleManager.Inst.PlayParticle(ParticleType.Ash, transform.position, transform.localScale, transform.localEulerAngles.z);
        ParticleManager.Inst.PlayParticle(ParticleType.Gravel, transform.position, transform.localScale, transform.localEulerAngles.z);

        Destroy(gameObject);
    }

    protected override void HitGround()
    {

    }
}
