using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningDetection : DetectionBase
{
    [Header("Burning Setting")]
    [SerializeField]
    private float SpreadTerm = 0.1f;

    [Space(5)]
    [SerializeField, Tooltip("������ Ÿ ������� ����")]
    private bool isBurnOut = true;
    [SerializeField]
    private float burningTime = 2f;
    [SerializeField]
    private GameObject burningAttack;

    private int burnTweenID = 0;
    private void Awake()
    {
        targetTag = Tag.Fire;
    }

    private void OnEnable()
    {
        Clear();
        burningAttack.SetActive(false);
    }
    private void OnDisable()
    {
        Clear();
    }
    private void Clear()
    {
        if (fireParticle)
        {
            fireParticle.ReturnToPool();
            fireParticle = null;
        }
            
        LeanTween.cancel(burnTweenID);
        burnTweenID = 0;
    }

    ParticleObject fireParticle;
    protected override void DetectionStart()
    {
        if (burnTweenID != 0) // �̹� ���� �پ��� ���
        {
            return;
        }

        Invoke("FireSpread", SpreadTerm);
        fireParticle = ParticleManager.Inst.PlayParticle(ParticleType.Fire, transform);

        if (isBurnOut)
            burnTweenID = LeanTween.color(gameObject, Color.black, burningTime).setOnComplete(BurnOut).id;
        else
            Invoke("FireAttackOFF", fireParticle.Particle.main.duration);
    }

    private void FireSpread()
    {
        if (burningAttack)
            burningAttack.SetActive(true);
        else
            Debug.LogError("burningAttack�� �����Ǿ� ���� ����.");
    }
    private void BurnOut()
    {
        Clear();

        ParticleManager.Inst.PlayParticle(ParticleType.Ash, transform);
        ParticleManager.Inst.PlayParticle(ParticleType.Arrow, transform);

        if (TryGetComponent<PoolObject>(out PoolObject pObj))
            pObj.ReturnToPool();
        else
            gameObject.SetActive(false);
    }
}
