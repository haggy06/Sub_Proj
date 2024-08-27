using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ObjectPool), typeof(AudioSource))]
public class EffectManager : Singleton<EffectManager>
{
    public const float bgmFade = 0.5f;

    protected override void SceneChanged(Scene replacedScene, Scene newScene)
    {

    }

    [SerializeField]
    private PoolObject[] effectArray; // ObjectPool의 initialMembers로 썼더니 중복으로 파괴될 놈이 풀을 만듦

    private static ObjectPool pool;
    private static AudioSource bgmSpeaker;
    protected override void Awake()
    {
        base.Awake();

        if (Inst == this)
        {
            pool = GetComponent<ObjectPool>();
            bgmSpeaker = GetComponent<AudioSource>();

            foreach (PoolObject pObj in effectArray)
                pool.MakePool(pObj);
            Debug.Log("Awake");
        }
    }
    public static ParticleObject PlayParticle(ParticleType particleType, Transform target)
    {
        if (particleType < ParticleType.Dust)
            return null;

        ParticleObject particle = (ParticleObject)pool.GetObject(Inst.effectArray[(int)particleType]);

        particle.Follow(target);
        particle.transform.localScale = target.localScale;

        // 파티클 위치 초기화
        particle.Init(target, target.eulerAngles.z);

        return particle;
    }
    public static SoundObject PlaySFX(AudioClip clip, Transform target)
    {
        SoundObject speaker = (SoundObject)pool.GetObject(Inst.effectArray[0]);

        speaker.Follow(target);
        speaker.clip = clip;

        speaker.Init(target, 0f);

        return speaker;
    }
    public static SoundObject PlaySFX(AudioClip clip)
    {
        SoundObject speaker = (SoundObject)pool.GetObject(Inst.effectArray[1]);

        speaker.clip = clip;

        speaker.Init(Inst.transform, 0f);

        return speaker;
    }
    public static void ChangeBGM(AudioClip bgm, bool useFade = true)
    {
        if (useFade)
        {
            LeanTween.value(bgmSpeaker.volume, 0f, bgmSpeaker.volume * 0.5f).setOnUpdate(ChangeVolume).setOnComplete(
                () => {
                    BGM = bgm;

                    LeanTween.value(bgmSpeaker.volume, 1f, (1f - bgmSpeaker.volume) * bgmFade).setOnUpdate(ChangeVolume);
                } );
        }
        else
        {
            BGM = bgm;
        }
    }
    private static AudioClip BGM
    {
        set 
        { 
            bgmSpeaker.clip = value;
            bgmSpeaker.Play();
        }
    }
    private static void ChangeVolume(float value)
    {
        bgmSpeaker.volume = value;
    }
}

public enum ParticleType
{ // 이제부터 인덱스 0, 1은 오디오 소스다!
    None,
    Dust = 2, // particleArray[ParticleType.Dust]를 했을 때 인덱스 2가 잡히게 하기 위함. (0, 1은 SoundObject 자리)
    Fire,
    Blood,
    Steam,
    Arrow,
    Gem,
    Ash,
}