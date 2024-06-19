using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private ObstacleType obstacletype;
    [SerializeField]
    private int obstacleID;

    [Space(5)]
    [SerializeField]
    private Vector2 hitKnockback;
    [SerializeField]
    private float gravityScale = 3f;

    [SerializeField]
    private float cameraShakeAmplitude = 10f;

    public int ObstacleID => obstacleID;
    public ObstacleType Obstacletype => obstacletype;

    public float CameraShakeAmplitude => cameraShakeAmplitude;
}

public enum ObstacleType
{
    Physical,
    Chemical,
    Fall,

}