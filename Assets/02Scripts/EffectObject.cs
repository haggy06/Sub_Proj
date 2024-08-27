using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class EffectObject : PoolObject
{
    [SerializeField]
    private Transform target;

    protected override void Awake()
    {
        base.Awake();

        SceneManager.activeSceneChanged += SceneChanged;
    }
    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneChanged;
    }

    protected virtual void SceneChanged(Scene replacedScene, Scene newScene)
    {
        Clear();
    }
    public virtual void Clear()
    {
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
    public virtual void Play()
    {
        if (useAutoReturn)
        {
            StopCoroutine("AutoReturn");
            StartCoroutine("AutoReturn");
        }
    }
    public override void Init(Transform owner, float rotation)
    {
        base.Init(owner, rotation);

        Play();
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
        target = null;

        isReturned = true;

        parentPool.ReturnObject(this);
    }
}
