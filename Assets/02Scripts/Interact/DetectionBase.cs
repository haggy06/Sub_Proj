using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class DetectionBase : MonoBehaviour
{
    public bool detecting = true;
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

    protected virtual void OnDestroy()
    {
        StopCoroutine("ParticleDetectionCoolDown");
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!detecting)
            return;

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
            HitGround(other.tag);
        }
    }
    private bool particleCoolDown = true; // 파티클을 통해 잠깐 동안 너무 많이 감지하는 걸 완화하기 위한 장치.
    private IEnumerator ParticleDetectionCoolDown()
    {
        particleCoolDown = false;

        yield return YieldReturn.WaitForSeconds(0.25f);

        particleCoolDown = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!detecting)
            return;

        if (collision.gameObject.layer == (int)LAYER.Ground) // 땅과 충돌했을 경우
        {
            HitGround(collision.gameObject.tag);
        }
        else if (collision.CompareTag(targetTag.ToString())) // 타겟과 충돌했을 경우
        {
            Detection = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!detecting)
            return;

        if (collision.CompareTag(targetTag.ToString()))
        {
            Detection = false;
        }
    }

    public event Action DetectionEndEvent = () => { };
    public event Action DetectionStartEvent = () => { };
    public event Action<string> HitGroundEvent = (_) => { };
    protected virtual void DetectionStart()
    {
        DetectionEndEvent.Invoke();
    }
    protected virtual void DetectionEnd()
    {
        DetectionStartEvent.Invoke();
    }

    protected virtual void HitGround(string tag)
    {
        HitGroundEvent.Invoke(tag);
    }
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