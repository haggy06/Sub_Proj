using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private DamageType obstacleType;
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

    public DamageType Obstacletype => obstacleType;
    public int ObstacleID => obstacleID;

    public bool VelocityImpulse => velocityImpulse;
    public Vector2 HitKnockback => hitKnockback;
    public float GravityScale => gravityScale;

    public float CameraShakeAmplitude => cameraShakeAmplitude;
}

public enum DamageType // 대미지를 받았을 때 튀는 파티클을 의미한다.
{
    None = -1, // ParticleList[DamageType.Dust]를 했을 때 인덱스 0이 집히게 하기 위함.
    Dust,
    Fire,
    Blood,
    Steam,

}