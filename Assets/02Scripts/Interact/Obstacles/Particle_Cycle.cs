using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_Cycle : CycleObstacle
{
    [SerializeField]
    private ParticleSystem particle;

    [SerializeField, Tooltip("true�� ���� �� ��ƼŬ�� �ڵ����� ������ ����")]
    private bool infinity = false;

    protected override void Run()
    {
        particle.Play();

        if (infinity)
        {
            StopCoroutine("LaunchCoroutine");
        }
    }

    protected override void RunStop()
    {
        particle.Stop();
    }
}
