using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_Cycle : CycleObstacle
{
    [SerializeField]
    private ParticleSystem particle;

    [SerializeField, Tooltip("true로 시작 시 파티클이 자동으로 꺼지지 않음")]
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
