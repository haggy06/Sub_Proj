using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class Attack : DetectionBase
{
    [SerializeField]
    private ParticleType obstacleType;
    [SerializeField]
    private int obstacleID;

    [Space(5)]
    [SerializeField]
    private bool velocityImpulse = true;
    [SerializeField]
    private Vector2 hitKnockback;
    [SerializeField]
    private float gravityScale = 3f;

    [Space(5)]
    [SerializeField]
    private float cameraShakeAmplitude = 10f;

    public ParticleType Obstacletype => obstacleType;
    public int ObstacleID => obstacleID;

    public bool VelocityImpulse => velocityImpulse;
    public Vector2 HitKnockback => hitKnockback;
    public float GravityScale => gravityScale;

    public float CameraShakeAmplitude => cameraShakeAmplitude;
    protected override void DetectionStart()
    {
        base.DetectionStart();

        if (targetTag == Tag.Player)
        {
            GameManager.Inst.ChangeGameStatus(GameStatus.GameOver, this);
            PlayerController.Inst.DamageInteract(this);
        }
    }
}