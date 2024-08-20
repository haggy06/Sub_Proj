using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class ParticleObject : PoolObject
{
    /*
    [SerializeField]
    private bool autoReturn;
    */
    [SerializeField]
    private ParticleType particleType;
    public ParticleType ParticleType => particleType;

    private ParticleSystem particle;
    public ParticleSystem Particle => particle;
    protected override void Awake()
    {
        base.Awake();

        particle = GetComponent<ParticleSystem>();

        SceneManager.activeSceneChanged += SceneChanged;
    }
    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneChanged;
    }
    private void SceneChanged(Scene replacedScene, Scene newScene)
    {
        ClearParticle();
    }
    public void ClearParticle()
    {
        particle.Stop();
        particle.Clear();

        if (parentPool && !isReturned)
        {
            ReturnToPool();
        }
    }

    private Transform target;
    public void Follow(Transform target)
    {
        this.target = target;
    }
    private void FixedUpdate()
    {
        if (target != null)
            transform.position = target.position;
    }
    /*
    public void FollowOFF()
    {
        target = null;
        particle.Stop();
    }
    */
    public void PlayParticle()
    {
        particle.Play();

        if (useAutoReturn)
        {
            StopCoroutine("AutoReturn");
            StartCoroutine("AutoReturn");
        }
        /*
        if (gameObject.activeInHierarchy)
        {
            particle.Play();

            if (autoReturn)
            {
                StopCoroutine("ParticleReturn");
                StartCoroutine("ParticleReturn");
            }
        }
        else
        {
            Debug.Log("��Ȱ��ȭ�� ���¿��� ��ƼŬ ���� �Ұ�����");
        }
        */
    }
    /*
    private IEnumerator ParticleReturn()
    {
        yield return YieldReturn.WaitForSeconds(particle.main.duration + particle.main.startLifetime.constantMax);

        if (!isReturned)
        {
            if (gameObject.activeInHierarchy)
            {
                ClearParticle();
            }
            else
            {
                Debug.Log("����");
            }
        }
    }

    
    */
    public override void Init(Transform owner, float rotation)
    {
        base.Init(owner, rotation);

        PlayParticle();
    }
    public override void ReturnToPool()
    {
        if (parentPool == null)
        {
            Debug.Log(name + "�� �θ� ����");
            Destroy(gameObject);
            return;
        }

        if (isReturned)
        {
            Debug.Log("�� �̹� �ݳ��� ������Ʈ��");
            return;
        }

        StopCoroutine("AutoReturn"); // �ڵ� ���� ���

        particle.Stop(); // gameObject.SetActive(false) ��� particle.Stop()�� �־���.
        target = null;

        isReturned = true;

        parentPool.ReturnObject(this);
    }
}
