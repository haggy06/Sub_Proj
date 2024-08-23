using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEvent : EntityEvent // 너, UnityEvent 가능성 있어
{
    [SerializeField]
    private ParticleSystem particle;

    /*
    public override void Run()
    {
        particle.Play();
    }

    public override void RunStop()
    {
        particle.Stop();
    }
    */
}
