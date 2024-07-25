using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningObject : PoolObject
{
    [Header("Burning Object")]

    [SerializeField]
    private float SpreadTerm = 0.1f;
    [SerializeField]
    private float burningTime = 2f;
    [SerializeField]
    private GameObject burningAttack;
    private PoolObject burningParticle;

    private int burnTweenID = 0;

    protected override void Awake()
    {
        base.Awake();

        burningAttack.SetActive(false);
    }
    public override void ExitFromPool(Transform newParent = null)
    {
        base.ExitFromPool(newParent);

        burningAttack.SetActive(false);
        burnTweenID = 0;
    }
    public override void ReturnToPool()
    {
        base.ReturnToPool();

        if (burnTweenID != 0)
        {
            LeanTween.cancel(burnTweenID);
        }
    }

    protected override void DetectionStart()
    {
        print("불이야");

        if (burnTweenID != 0) // 이미 불이 붙었을 경우
        {
            return;
        }

        Invoke("FireSpread", SpreadTerm);
        burningParticle = ParticleManager.Inst.PlayParticle(ParticleType.Fire, transform);
        burnTweenID = LeanTween.color(gameObject, Color.black, burningTime).setOnComplete(BurnOut).id;
    }

    private void FireSpread()
    {
        burningAttack.SetActive(true);
    }
    private void BurnOut()
    {
        burningParticle.ReturnToPool();
        burnTweenID = 0;

        ParticleManager.Inst.PlayParticle(ParticleType.Ash, transform.position, transform.localScale, transform.localEulerAngles.z);
        ParticleManager.Inst.PlayParticle(ParticleType.Gravel, transform.position, transform.localScale, transform.localEulerAngles.z);

        ReturnToPool();
    }
}
