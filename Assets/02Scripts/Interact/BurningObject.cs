using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningObject : DetectionBase
{
    [SerializeField]
    private float burningTime = 2f;
    [SerializeField]
    private Collider2D burningAttack;
    private PoolObject burningParticle;

    private int burnTweenID;
    private void OnEnable()
    {
        burningAttack.enabled = false;
        burnTweenID = 0;
    }
    private void OnDisable()
    {
        if (burnTweenID != 0)
        {
            LeanTween.cancel(burnTweenID);
        }
    }
    protected override void DetectionEnd()
    {

    }

    protected override void DetectionStart()
    {
        if (burnTweenID != 0) // �̹� ���� �پ��� ���
        {
            return;
        }

        Invoke("FireSpread", 0.2f);
        burningParticle = ParticleManager.Inst.PlayParticle(ParticleType.Fire, transform);
        burnTweenID = LeanTween.color(gameObject, Color.black, burningTime).setOnComplete(BurnOut).id;
    }
    private void FireSpread()
    {
        burningAttack.enabled = true;
    }

    private void BurnOut()
    {
        burningParticle.ReturnToPool();
        burnTweenID = 0;

        ParticleManager.Inst.PlayParticle(ParticleType.Ash, transform.position, transform.localScale, transform.localEulerAngles.z);
        ParticleManager.Inst.PlayParticle(ParticleType.Gravel, transform.position, transform.localScale, transform.localEulerAngles.z);

        gameObject.SetActive(false);
    }

    protected override void HitGround(string tag)
    {

    }
}
