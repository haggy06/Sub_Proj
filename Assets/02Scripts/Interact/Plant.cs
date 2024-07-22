using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : DetectionBase
{
    [SerializeField]
    private float burningTime = 2f;
    [SerializeField]
    private Collider2D burningAttack;

    private int burnTweenID;
    private void OnEnable()
    {
        burningAttack.enabled = false;
        burnTweenID = 0;
    }
    protected override void DetectionEnd()
    {

    }

    protected override void DetectionStart()
    {
        if (burnTweenID != 0) // 이미 불이 붙었을 경우
        {
            return;
        }

        Invoke("FireSpread", 0.2f);
        ParticleManager.Inst.PlayParticle(ParticleType.Fire, transform);
        burnTweenID = LeanTween.color(gameObject, Color.black, burningTime).setOnComplete(BurnOut).id;
    }
    private void FireSpread()
    {
        burningAttack.enabled = true;
    }

    private void BurnOut()
    {
        ParticleManager.Inst.PlayParticle(ParticleType.Ash, transform.position, transform.localScale, transform.localEulerAngles.z);
        ParticleManager.Inst.PlayParticle(ParticleType.Gravel, transform.position, transform.localScale, transform.localEulerAngles.z);

        gameObject.SetActive(false);
    }

    protected override void HitGround()
    {

    }
}
