using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingDot : MonoBehaviour
{
    private SpriteRenderer stageSprite;
    private SpriteRenderer minimapSprite;

    private void Awake()
    {
        stageSprite = GetComponent<SpriteRenderer>();
        minimapSprite = GetComponentsInChildren<SpriteRenderer>()[1];

        OFF();
    }

    public void SetPosition(Vector2 position, Color color)
    {
        transform.localPosition = position;

        stageSprite.enabled = true;
        stageSprite.color = minimapSprite.color = color;
    }

    public void OFF()
    {
        transform.localPosition = Vector2.zero;

        stageSprite.enabled = false;
        stageSprite.color = minimapSprite.color = CustomColor.zero;
    }
}
