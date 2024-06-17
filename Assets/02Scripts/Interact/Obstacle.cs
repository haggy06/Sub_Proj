using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private ObstacleType obstacletype;
    [SerializeField]
    private int obstacleID;

    public int ObstacleID => obstacleID;
    public ObstacleType Obstacletype => obstacletype;    
}

public enum ObstacleType
{
    Physical,
    Chemical,
    Fall,

}