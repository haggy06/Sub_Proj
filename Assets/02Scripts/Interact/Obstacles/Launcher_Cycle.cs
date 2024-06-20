using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher_Cycle : CycleObstacle
{
    [Header("Launcher Setting")]
    [SerializeField]
    private ObjectPool pool;
    [SerializeField]
    private Transform launchPosition;
    [SerializeField]
    private bool dynamicLaunchAngle = false;

    [Header("Projectile Setting")]
    [SerializeField]
    private GameObject launchObject;
    [SerializeField]
    private Vector2 launchDirection = Vector2.right;
    [SerializeField]
    private float launchPower = 10f;

    protected override void Run()
    {
        if (!pool.TryGetObject(launchObject, out GameObject projectile)) // 이 발사체에 대한 오브젝트풀이 없을 경우
        {
            pool.MakePool(launchObject);
            pool.TryGetObject(launchObject, out projectile); // 풀 제작 후 다시 가져오기
        }

        projectile.transform.position = launchPosition.position;
        if (dynamicLaunchAngle)
        {
            launchDirection = MyCalculator.Deg2Vec(transform.eulerAngles.z);
        }

        projectile.GetComponent<I_Projectile>().Launch(launchDirection, launchPower);
    }
}
