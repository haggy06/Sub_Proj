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
    [SerializeField]
    private float launchPower = 10f;

    public override void Run()
    {
        PoolObject projectile = pool.GetObject(launchObject);

        projectile.transform.position = launchPosition.position;

        projectile.GetComponent<I_Projectile>().Launch(transform.eulerAngles.z, launchPower);
    }

    public override void RunStop()
    {

    }
}
