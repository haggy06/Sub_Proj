using System.Collections;
using System.Collections.Generic;
using UnityEngine;  

public class Launcher : EntityEvent
{
    [Header("Launcher Setting")]
    [SerializeField]
    private ObjectPool pool;
    [SerializeField]
    private Transform launchPosition;

    [Header("Projectile Setting")]
    [SerializeField]
    private PoolObject launchObject;

    public override void Run()
    {
        PoolObject projectile = pool.GetObject(launchObject);
        projectile.Init(launchPosition, transform.eulerAngles.z);
    }

    public override void RunStop()
    {

    }
}
