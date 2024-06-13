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
        if (collision.gameObject.TryGetComponent<Obstacle>(out Obstacle obstacle)) // ��ֹ� ����
        {
            HitEvent.Invoke(obstacle);
        }
        else if (collision.gameObject.TryGetComponent<ClearZone>(out ClearZone clear)) // Ŭ���� ����
        {
            ClearEvent.Invoke();
        }
    }
}