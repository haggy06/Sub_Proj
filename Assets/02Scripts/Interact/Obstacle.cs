using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private ObstacleType obstacletype;
    public ObstacleType Obstacletype => obstacletype;

    
}

public enum ObstacleType
{
    Physical,
    Chemical,
    Fall,

}