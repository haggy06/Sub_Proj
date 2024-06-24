using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ParticleManager : MonoSingleton<ParticleManager>
{
    protected override void SetInstanceToThis()
    {
        instance = this;
    }
    protected override void SceneChanged(Scene replacedScene, Scene newScene)
    {
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
    }

    [SerializeField]
    private GameObject[] particleArray;
    [SerializeField]
    private Transform trackingParticles;
    private Stack<GameObject>[] trackingParticleStack;
    protected override void Awake()
    {
        base.Awake();
        trackingParticleStack = new Stack<GameObject>[particleArray.Length]; // trackingParticleStack �ʱ�ȭ
        for (int i = 0; i < particleArray.Length; i++)
        {
            trackingParticleStack[i] = new Stack<GameObject>();
        }
    }

    public void PlayParticle(ParticleType particleType, Vector2 position, Vector2 scale, float rotation = 0f)
    {
        if (particleType == ParticleType.None)
        {
            Debug.Log("None Ÿ���� ��ƼŬ�� �������� ����");
            return;
        }

        GameObject particle = particleArray[(int)particleType];

        particle.transform.position = position;
        particle.transform.localScale = scale;
        particle.transform.eulerAngles = Vector3.forward * rotation;

        particle.GetComponent<ParticleSystem>().Play();
    }
    public void PlayParticle(ParticleType particleType, Transform target)
    {
        foreach (ParticleObject businessTripParticle in target.GetComponentsInChildren<ParticleObject>())
        {
            if (businessTripParticle.ParticleType == particleType) // �̹� ���� ���� ��ƼŬ �� �˸´� ��ƼŬ�� �־��� ���
            {
                Debug.Log("���� �� �ִ� ������Ʈ ��Ȱ��");
                businessTripParticle.PlayParticle(); // �� ��ƼŬ �� �� �� ��Ȱ��

                return;
            }
        }

        // ���� �� �ִ� ��ƼŬ�� ���� ���
        if (!trackingParticleStack[(int)particleType].TryPop(out GameObject particle)) // ���ÿ��� �ϳ� ����
        { // ���ÿ� ���� ��ƼŬ�� ���� ���
            particle = Instantiate(particleArray[(int)particleType]);
            particle.GetComponent<ParticleObject>().RememberPool(trackingParticleStack[(int)particleType], trackingParticles); // �ϳ� ���� ����� Ǯ ���
        }

        // ��ƼŬ ��ġ �ʱ�ȭ
        particle.transform.parent = target;
        particle.transform.localPosition = Vector3.zero;
        particle.transform.localEulerAngles = Vector3.zero;
        particle.transform.localScale = Vector3.one;

        particle.GetComponent<ParticleObject>().PlayParticle();
    }
}

public enum ParticleType
{
    None = -1, // particleArray[ParticleType.Dust]�� ���� �� �ε��� 0�� ������ �ϱ� ����.
    Dust,
    Fire,
    Blood,
    Steam,
    Gravel,
    Gem,
    Ash,

}