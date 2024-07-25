using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObject : PoolObject
{
    [SerializeField]
    private ParticleType particleType;
    public ParticleType ParticleType => particleType;

    private ParticleSystem particle;
    protected override void Awake()
    {
        base.Awake();

        particle = GetComponent<ParticleSystem>();
    }

    public void PlayParticle()
    {
        if (gameObject.activeInHierarchy)
        {
            particle.Play();

            StopCoroutine("ParticleTracking");
            StartCoroutine("ParticleTracking");
        }
        else
        {
            Debug.LogError("��Ȱ��ȭ�� ���¿��� ��ƼŬ ���� �Ұ�����");
        }
    }
    private IEnumerator ParticleTracking()
    {
        yield return YieldReturn.WaitForSeconds(particle.main.duration + particle.main.startLifetime.constantMax);

        if (!isReturned)
        {
            if (gameObject.activeInHierarchy)
            {
                ReturnToPool();
            }
            else
            {
                Debug.Log("����");
            }
        }
    }

    public override void ReturnToPool()
    {
        base.ReturnToPool();

        StopCoroutine("ParticleTracking");
    }

    public override void ExitFromPool(Transform newParent = null)
    {
        base.ExitFromPool(newParent);

        PlayParticle();
    }
}
