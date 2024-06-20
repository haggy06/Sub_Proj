using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class DetectionBase : MonoBehaviour
{
    [SerializeField]
    protected Tag targetTag = Tag.Player;
    [SerializeField]
    protected bool detection = false;
    public bool Detection
    {
        get => detection;
        protected set
        {
            if (detection != value)
            {
                detection = value;

                if (detection)
                {
                    DetectionStart();
                }
                else
                {
                    DetectionEnd();
                }
            }
        }
    }

    protected virtual void Awake()
    {
        gameObject.layer = (int)LAYER.Censor; // Censor 레이어는 Censor, Ground와만 충돌한다.
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == (int)LAYER.Ground) // 땅과 충돌했을 경우
        {
            HitGround();
        }
        else
        {
            if (collision.CompareTag(targetTag.ToString()))
            {
                Detection = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag.ToString()))
        {
            Detection = false;
        }
    }

    protected abstract void DetectionStart();
    protected abstract void DetectionEnd();

    protected abstract void HitGround();
}

public enum Tag
{
    Untagged,
    Respawn,
    Finish,
    EditorOnly,
    MainCamera,
    Player,
    GameController,
    Clear,

}