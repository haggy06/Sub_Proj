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
        gameObject.layer = (int)LAYER.Censor; // Censor ���̾�� Censor, Ground�͸� �浹�Ѵ�.
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == (int)LAYER.Ground) // ���� �浹���� ���
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