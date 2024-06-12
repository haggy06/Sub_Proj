using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingLine : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;

    private void Awake()
    {
        DragManager.MouseDragEvent += SetAuxiliaryLine;
    }

    private void SetAuxiliaryLine(Vector2 aim, float powerPercent)
    {
        float g = Physics2D.gravity.y * player.Rigid2D.gravityScale; // 공통 중력 세기 * 플레이어 중력 세기


    }
}