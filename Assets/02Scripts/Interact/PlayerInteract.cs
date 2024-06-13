using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

[RequireComponent(typeof(Collider2D))]
public class PlayerInteract : MonoBehaviour
{
    public event Action<Obstacle> HitEvent = (_) => { };
    public event Action ClearEvent = () => { };
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Obstacle>(out Obstacle obstacle)) // 장애물 감지
        {
            HitEvent.Invoke(obstacle);
        }
        else if (collision.gameObject.TryGetComponent<ClearZone>(out ClearZone clear)) // 클리어 감지
        {
            ClearEvent.Invoke();
        }
    }
}