using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
    protected virtual void OnDestroy()
    {
        StopCoroutine("ParticleDetectionCoolDown");
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag(targetTag.ToString())) // 타겟과 충돌했을 경우
        {
            if (particleCoolDown)
            {
                DetectionStart();
                StartCoroutine("ParticleDetectionCoolDown");
            }
        }        
        else if (other.layer == (int)LAYER.Ground) // 땅과 충돌했을 경우
        {
            HitGround();
        }
    }
    private bool particleCoolDown = true; // 파티클을 통해 잠깐 동안 너무 많이 감지하는 걸 완화하기 위한 장치.
    private IEnumerator ParticleDetectionCoolDown()
    {
        particleCoolDown = false;

        yield return new WaitForSeconds(0.25f);

        particleCoolDown = true;
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
    Fire,

}