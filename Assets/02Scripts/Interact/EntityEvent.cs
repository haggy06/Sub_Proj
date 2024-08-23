using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class EntityEvent : MonoBehaviour
{
    [SerializeField, Tooltip("Æ®¸®°Å ¹æ½ÄÀÌ¸é true, ²°´Ù ÄÑÁö´Â ¹æ½ÄÀÌ¸é false")]
    private bool isTrigger = false;
    public bool IsTrigger => isTrigger;

    [SerializeField]
    private bool runWhenAwake = false;

    [Header("Event Setting")]
    [SerializeField]
    private UnityEvent runEvent;
    [SerializeField]
    private UnityEvent runStopEvent;

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
        print(transform.root.gameObject);
        if (runWhenAwake)
            Run();

        anim = GetComponent<Animator>();
        if (!spawnPosition)
            spawnPosition = transform;
    }

    #region _Run & RunStop_
    public void Run()
    {
        if (anim)
            anim.SetTrigger(AnimationHash.Run);
        else
            Invoke_Run();
    }
    [ContextMenu("RUN")]
    public void Invoke_Run()
    {
        if (runEvent != null)
            runEvent.Invoke();
    }

    public void Stop()
    {
        if (anim)
            anim.SetTrigger(AnimationHash.Run);
        else
            Invoke_Stop();
    }

    [ContextMenu("STOP")]
    public void Invoke_Stop()
    {
        if (runStopEvent != null)
            runStopEvent.Invoke();
    }
    #endregion

    #region _Metohds For UnityEvent_
    public void SpawnObject(PoolObject target)
    {
        pool.GetObject(target).Init(spawnPosition, transform.eulerAngles.z);
    }


    public void WorldMove_Horizontal(float displacement)
    {
         LeanTween.moveX(gameObject, displacement, lTRequiredTime).setEase(lTType); // Ã¢ Æ¢¾î³ª¿È
    }
    public void WorldMove_Vertical(float displacement)
    {
        LeanTween.moveY(gameObject, displacement, lTRequiredTime).setEase(lTType); // Ã¢ Æ¢¾î³ª¿È
    }
    public void LocalMove_Horizontal(float displacement)
    {
        LeanTween.moveLocalX(gameObject, displacement, lTRequiredTime).setEase(lTType); // Ã¢ Æ¢¾î³ª¿È
    }
    public void LocalMove_Vertical(float displacement)
    {
        LeanTween.moveLocalY(gameObject, displacement, lTRequiredTime).setEase(lTType); // Ã¢ Æ¢¾î³ª¿È
    }
    #endregion
}

public class AnimationHash
{
    public static readonly int Run = Animator.StringToHash("Run");
    public static readonly int Stop = Animator.StringToHash("Stop");
}