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
        trackingParticleStack = new Stack<GameObject>[particleArray.Length]; // trackingParticleStack 초기화
        for (int i = 0; i < particleArray.Length; i++)
        {
            trackingParticleStack[i] = new Stack<GameObject>();
        }
    }

    public void PlayParticle(ParticleType particleType, Vector2 position, Vector2 scale, float rotation = 0f)
    {
        if (particleType == ParticleType.None)
        {
            Debug.Log("None 타입의 파티클은 존재하지 않음");
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
            if (businessTripParticle.ParticleType == particleType) // 이미 보내 놓은 파티클 중 알맞는 파티클이 있었을 경우
            {
                Debug.Log("출장 가 있는 오브젝트 재활용");
                businessTripParticle.PlayParticle(); // 그 파티클 한 번 더 재활용

                return;
            }
        }

        // 출장 가 있는 파티클이 없을 경우
        if (!trackingParticleStack[(int)particleType].TryPop(out GameObject particle)) // 스택에서 하나 빼옴
        { // 스택에 빼올 파티클이 없을 경우
            particle = Instantiate(particleArray[(int)particleType]);
            particle.GetComponent<ParticleObject>().RememberPool(trackingParticleStack[(int)particleType], trackingParticles); // 하나 새로 만들고 풀 등록
        }

        // 파티클 위치 초기화
        particle.transform.parent = target;
        particle.transform.localPosition = Vector3.zero;
        particle.transform.localEulerAngles = Vector3.zero;
        particle.transform.localScale = Vector3.one;

        particle.GetComponent<ParticleObject>().PlayParticle();
    }
}

public enum ParticleType
{
    None = -1, // particleArray[ParticleType.Dust]를 했을 때 인덱스 0이 집히게 하기 위함.
    Dust,
    Fire,
    Blood,
    Steam,
    Gravel,
    Gem,
    Ash,

}