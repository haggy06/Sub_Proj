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

    public event Action DetectionEndEvent = () => { };
    public event Action DetectionStartEvent = () => { };
    public event Action HitGroundEvent = () => { };
    protected override void DetectionEnd()
    {
        DetectionEndEvent.Invoke();
    }

    protected override void DetectionStart()
    {
        GameManager.Inst.ChangeGameStatus(GameStatus.GameOver, this);
        PlayerController.Inst.DamageInteract(this);

        DetectionStartEvent.Invoke();
    }

    protected override void HitGround(string tag)
    {
        HitGroundEvent.Invoke();
    }
}