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
    private GameObject launchObject;
    [SerializeField]
    private float launchPower = 10f;

    public override void Run()
    {
        if (!pool.TryGetObject(launchObject, out GameObject projectile)) // �� �߻�ü�� ���� ������ƮǮ�� ���� ���
        {
            pool.MakePool(launchObject);
            pool.TryGetObject(launchObject, out projectile); // Ǯ ���� �� �ٽ� ��������
        }

        projectile.transform.position = launchPosition.position;


        projectile.GetComponent<I_Projectile>().Launch(transform.eulerAngles.z, launchPower);
    }

    public override void RunStop()
    {

    }
}
