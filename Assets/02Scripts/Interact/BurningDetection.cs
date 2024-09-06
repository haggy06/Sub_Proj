using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningDetection : DetectionBase
{
    [Header("Burning Setting")]
    [SerializeField]
    private float SpreadTerm = 0.1f;

    [Space(5)]
    [SerializeField, Tooltip("타 사라질지 여부")]
    private bool isBurnOut = true;
    [SerializeField, Range(0f, 2f)]
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
        if (burningAttack)
            burningAttack.SetActive(false);
    }
    private void OnDisable()
    {
        Clear();
    }
    private void Clear()
    {
        burning = false;
        if (fireParticle)
        {
            fireParticle.ReturnToPool();
            fireParticle = null;
        }
            
        LeanTween.cancel(burnTweenID);
        burnTweenID = 0;
    }

    ParticleObject fireParticle;
    SoundObject fireSound;

    private bool burning = false;
    protected override void DetectionStart()
    {
        if (burning || !gameObject.activeInHierarchy) // 이미 불이 붙었거나 비활성화된 오브젝트일 경우
        {
            return;
        }
        burning = true;

        StartCoroutine("FireSpread");
        fireParticle = EffectManager.Inst.PlayParticle(ParticleType.Fire, transform);
        fireSound = EffectManager.Inst.PlaySFX(ResourceLoader.AudioLoad(FolderName.Death, ParticleType.Fire.ToString()));

        if (isBurnOut)
            burnTweenID = LeanTween.color(gameObject, Color.black, burningTime).setOnComplete(BurnOut).id;
        else
        {
            StopCoroutine("FireAttackOFF");
            StartCoroutine("FireAttackOFF");
        }
    }
    private IEnumerator FireAttackOFF()
    {
        yield return YieldReturn.WaitForSeconds(fireParticle.Particle.main.duration);

        if (burningAttack)
            burningAttack.SetActive(false);
        burning = false;
    }
    private IEnumerator FireSpread()
    {
        yield return YieldReturn.WaitForSeconds(SpreadTerm);

        if (burningAttack)
            burningAttack.SetActive(true);
    }

    private void BurnOut()
    {
        fireParticle.ReturnToPool();
        fireSound.ReturnToPool();

        Clear();

        EffectManager.Inst.PlayParticle(ParticleType.Ash, transform);
        EffectManager.Inst.PlayParticle(ParticleType.Arrow, transform);

        if (TryGetComponent<PoolObject>(out PoolObject pObj))
            pObj.ReturnToPool();
        else
            gameObject.SetActive(false);
    }
}
