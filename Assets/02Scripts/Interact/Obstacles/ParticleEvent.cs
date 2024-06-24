using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEvent : EntityEvent
{
    [SerializeField]
    private ParticleSystem particle;

    public override void Run()
    {
        particle.Play();
    }

    public override void RunStop()
    {
        particle.Stop();
    }
}
