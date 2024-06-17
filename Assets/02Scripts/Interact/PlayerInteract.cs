using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

[RequireComponent(typeof(Collider2D))]
public class PlayerInteract : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Obstacle>(out Obstacle obstacle)) // 장애물 감지
        {
            GameManager.Inst.ChangeGameStatus(GameStatus.GameOver, obstacle);
            PlayerController.Inst.DamageInteract(obstacle);
        }
    }
}