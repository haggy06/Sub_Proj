using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class EntityEvent : MonoBehaviour
{
    [SerializeField, Tooltip("트리거 방식이면 true, 껐다 켜지는 방식이면 false")]
    private bool isTrigger = false;
    public bool IsTrigger => isTrigger;

    [SerializeField]
    private bool runWhenAwake = false;

    [Header("Event Setting")]
    [SerializeField]
    private AudioClip runSound;
    [SerializeField]
    private UnityEvent runEvent;

    [Space(5)]

    [SerializeField]
    private AudioClip stopSound;
    [SerializeField]
    private UnityEvent stopEvent;

    [Space(10)]
    [SerializeField]
    private Transform spawnPosition;
    [SerializeField]
    private LeanTweenType lTType = LeanTweenType.linear;
    [SerializeField, Range(0.01f, 5f)]
    private float lTRequiredTime = 5f;

    private Animator anim;
    private ObjectPool _pool;
    private ObjectPool pool
    {
        get
        {
            if (_pool == null)
                if (!transform.root.TryGetComponent<ObjectPool>(out _pool))
                    _pool = transform.root.gameObject.AddComponent<ObjectPool>();

            return _pool;
        }
    }
    protected virtual void Awake()
    {
        if (runWhenAwake)
            Run();

        anim = GetComponent<Animator>();
        if (!spawnPosition)
            spawnPosition = transform;
    }

    #region _Run & Stop_
    [ContextMenu("RUN")]
    public void Run()
    {
        if (anim)
            anim.SetTrigger(AnimationHash.Run);
        else
            Invoke_Run();
    }
    [ContextMenu("Invoke_Run")]
    public void Invoke_Run()
    {
        if (runSound)
            EffectManager.Inst.PlaySFX(runSound, spawnPosition);

        if (runEvent != null)
            runEvent.Invoke();
    }

    [ContextMenu("STOP")]
    public void Stop()
    {
        if (anim)
            anim.SetTrigger(AnimationHash.Run);
        else
            Invoke_Stop();
    }

    [ContextMenu("Invoke_Stop")]
    public void Invoke_Stop()
    {
        if (stopSound)
            EffectManager.Inst.PlaySFX(stopSound, spawnPosition);

        if (stopEvent != null)
            stopEvent.Invoke();
    }
    #endregion

    #region _Metohds For UnityEvent_
    public void SpawnObject(PoolObject target)
    {
        pool.GetObject(target).Init(spawnPosition, transform.eulerAngles.z);
    }

    private int cTweenID = 0;
    private void TweenIDReset()
    {
        cTweenID = 0;
    }
    public void WorldMove_Horizontal(float displacement)
    {
        LeanTween.cancel(cTweenID);
        cTweenID = LeanTween.moveX(gameObject, displacement, lTRequiredTime).setEase(lTType).setOnComplete(TweenIDReset).id;
    }
    public void WorldMove_Vertical(float displacement)
    {
        LeanTween.cancel(cTweenID);
        cTweenID = LeanTween.moveY(gameObject, displacement, lTRequiredTime).setEase(lTType).setOnComplete(TweenIDReset).id;
    }
    public void LocalMove_Horizontal(float displacement)
    {
        LeanTween.cancel(cTweenID);
        cTweenID = LeanTween.moveLocalX(gameObject, displacement, lTRequiredTime).setEase(lTType).setOnComplete(TweenIDReset).id;
    }
    public void LocalMove_Vertical(float displacement)
    {
        LeanTween.cancel(cTweenID);
        cTweenID = LeanTween.moveLocalY(gameObject, displacement, lTRequiredTime).setEase(lTType).setOnComplete(TweenIDReset).id;
    }
    #endregion
}

public class AnimationHash
{
    public static readonly int Run = Animator.StringToHash("Run");
    public static readonly int Stop = Animator.StringToHash("Stop");
}