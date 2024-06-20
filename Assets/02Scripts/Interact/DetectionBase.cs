using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class DetectionBase : MonoBehaviour
{
    [SerializeField]
    private bool isParticle = false;

    [SerializeField]
    protected Tag targetTag = Tag.Player;

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

        if (isParticle) // 파티클용일 땐 센서를 끈 상태로 시작
        {
            GetComponent<Collider2D>().enabled = false;
        }
    }
    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag(targetTag.ToString())) // 타겟과 충돌했을 경우
        {
            DetectionStart();
        }        
        else if (other.layer == (int)LAYER.Ground) // 땅과 충돌했을 경우
        {
            HitGround();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == (int)LAYER.Ground) // 땅과 충돌했을 경우
        {
            HitGround();
        }
        else if (collision.CompareTag(targetTag.ToString())) // 타겟과 충돌했을 경우
        {
            Detection = true;
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