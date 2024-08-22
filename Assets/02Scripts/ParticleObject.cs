using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class ParticleObject : PoolObject
{
    [Header("Particle Setting")]
    [SerializeField]
    private ParticleType particleType;
    public ParticleType ParticleType => particleType;

    [SerializeField]
    private Transform target;


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
    public void Follow(Transform target)
    {
        this.target = target;
    }
    private void FixedUpdate()
    {
        if (target != null)
        {
            if (target.gameObject.activeInHierarchy)
                transform.position = target.position;
            else
                target = null;
        }
    }
    public void PlayParticle()
    {
        particle.Play();

        if (useAutoReturn)
        {
            StopCoroutine("AutoReturn");
            StartCoroutine("AutoReturn");
        }
    }
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
