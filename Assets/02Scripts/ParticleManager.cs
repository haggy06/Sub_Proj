using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ObjectPool))]
public class ParticleManager : MonoSingleton<ParticleManager>
{
    protected override void SceneChanged(Scene replacedScene, Scene newScene)
    {
        /*
        ParticleSystem particle;

        foreach (GameObject obj in particleArray)
        {
            particle = obj.GetComponent<ParticleSystem>();
            particle.Stop();
            particle.Clear();
        }
        for (int i = 0; i < trackingParticles.childCount; i++)
        {
            particle = trackingParticles.GetChild(i).GetComponent<ParticleSystem>();
            particle.Stop();
            particle.Clear();
        }
        */
    }

    [SerializeField]
    private Transform particles;

    private ObjectPool pool;
    protected override void Awake()
    {
        base.Awake();
        pool = GetComponent<ObjectPool>();
    }
    /*
    public void PlayParticle(ParticleType particleType, Vector2 position, Vector2 scale, float rotation = 0f)
    {
        if (particleType == ParticleType.None)
        {
            Debug.Log("None Ÿ���� ��ƼŬ�� �������� ����");
            return;
        }

        Transform particle = particles.GetChild((int)particleType);

        particle.position = position;
        particle.localScale = scale;
        particle.eulerAngles = Vector3.forward * rotation;

        particle.GetComponent<ParticleSystem>().Play();
    }
    */
    public ParticleObject PlayParticle(ParticleType particleType, Transform target)
    {
        /* // ����� ������ ��ƼŬ ��Ȱ���� �Ұ���
        foreach (ParticleObject businessTripParticle in target.GetComponentsInChildren<ParticleObject>())
        {
            if (businessTripParticle.ParticleType == particleType) // �̹� ���� ���� ��ƼŬ �� �˸´� ��ƼŬ�� �־��� ���
            {
                Debug.Log("���� �� �ִ� ������Ʈ ��Ȱ��");
                businessTripParticle.PlayParticle(); // �� ��ƼŬ �� �� �� ��Ȱ��

                return businessTripParticle;
            }
        }
        // ���� �� �ִ� ��ƼŬ�� ���� ���
        */
        ParticleObject particle = (ParticleObject)pool.GetObject(pool.InitialMembers[(int)particleType]);

        // ��ƼŬ ��ġ �ʱ�ȭ
        particle.Init(target, target.eulerAngles.z);

        particle.GetComponent<ParticleObject>().Follow(target);
        particle.transform.localScale = target.localScale;

        return particle;
    }
}

public enum ParticleType
{
    None = -1, // particleArray[ParticleType.Dust]�� ���� �� �ε��� 0�� ������ �ϱ� ����.
    Dust,
    Fire,
    Blood,
    Steam,
    Arrow,
    Gem,
    Ash,

}