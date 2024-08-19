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
    //[SerializeField]
    //private float launchPower = 10f;

    public override void Run()
    {
        PoolObject projectile = pool.GetObject(launchObject);
        projectile.Init(launchPosition, transform.eulerAngles.z);
        /*
        if (projectile.TryGetComponent<I_Projectile>(out I_Projectile proj))
        {
            proj.Launch(transform.eulerAngles.z, launchPower);
        }
        else
        {
            Debug.Log(launchObject.name + " 오브젝트는 발사체가 아님");
        }   
        */
    }

    public override void RunStop()
    {

    }
}
