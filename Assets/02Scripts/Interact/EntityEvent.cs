using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityEvent : MonoBehaviour
{
    [SerializeField, Tooltip("트리거 방식이면 true, 껐다 켜지는 방식이면 false")]
    private bool isTrigger = false;
    public bool IsTrigger => isTrigger;

    [SerializeField]
    private bool playOnAwake = false;
    public bool PlayOnAwake => playOnAwake;

    protected virtual void Awake()
    {
        if (playOnAwake)
        {
            Run();
        }
    }

    public abstract void Run();
    public abstract void RunStop();
}
