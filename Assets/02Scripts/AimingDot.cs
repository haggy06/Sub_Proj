using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingDot : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer stageSprite;
    [SerializeField]
    private SpriteRenderer minimapSprite;

    private void Awake()
    {
        OFF();
    }

    public void SetPosition(Vector2 position, Color color)
    {
        transform.localPosition = position;

        stageSprite.color = minimapSprite.color = color;
    }

    public void OFF()
    {
        transform.localPosition = Vector2.zero;

        stageSprite.color = minimapSprite.color = CustomColor.zero;
    }
}
