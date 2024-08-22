using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ObjectPool))]
public class ParticleManager : Singleton<ParticleManager>
{
    protected override void SceneChanged(Scene replacedScene, Scene newScene)
    {

    }

    private ObjectPool pool;
    protected override void Awake()
    {
        base.Awake();
        pool = GetComponent<ObjectPool>();
    }
    public ParticleObject PlayParticle(ParticleType particleType, Transform target)
    {
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