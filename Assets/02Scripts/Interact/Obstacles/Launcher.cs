using System.Collections;
using System.Collections.Generic;
using UnityEngine;  

public class Launcher : EntityEvent // ��, UnityEvent ���ɼ� �־�
{
    [Header("Launcher Setting")]
    [SerializeField]
    private Transform launchPosition;

    [Header("Projectile Setting")]
    [SerializeField]
    private PoolObject launchObject;
    /*
    public override void Run()
    {
        PoolObject projectile = pool.GetObject(launchObject);
        projectile.Init(launchPosition, transform.eulerAngles.z);
    }

    public override void RunStop()
    {

    }
    */
}
