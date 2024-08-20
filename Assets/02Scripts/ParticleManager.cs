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
            Debug.Log("None 타입의 파티클은 존재하지 않음");
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
        /* // 변경된 구조상 파티클 재활용이 불가함
        foreach (ParticleObject businessTripParticle in target.GetComponentsInChildren<ParticleObject>())
        {
            if (businessTripParticle.ParticleType == particleType) // 이미 보내 놓은 파티클 중 알맞는 파티클이 있었을 경우
            {
                Debug.Log("출장 가 있는 오브젝트 재활용");
                businessTripParticle.PlayParticle(); // 그 파티클 한 번 더 재활용

                return businessTripParticle;
            }
        }
        // 출장 가 있는 파티클이 없을 경우
        */
        ParticleObject particle = (ParticleObject)pool.GetObject(pool.InitialMembers[(int)particleType]);

        // 파티클 위치 초기화
        particle.Init(target, target.eulerAngles.z);

        particle.GetComponent<ParticleObject>().Follow(target);
        particle.transform.localScale = target.localScale;

        return particle;
    }
}

public enum ParticleType
{
    None = -1, // particleArray[ParticleType.Dust]를 했을 때 인덱스 0이 집히게 하기 위함.
    Dust,
    Fire,
    Blood,
    Steam,
    Arrow,
    Gem,
    Ash,

}