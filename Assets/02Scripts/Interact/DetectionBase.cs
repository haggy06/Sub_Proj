using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class DetectionBase : MonoBehaviour
{
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
        EnterLogic(other, true);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnterLogic(collision.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnterLogic(collision.gameObject);
    }
    private void EnterLogic(GameObject gameObject, bool isParticleCollision = false)
    {
        if (gameObject.layer == (int)LAYER.Ground) // 땅과 충돌했을 경우
        {
            HitGround(gameObject.tag);
        }
        else if (gameObject.CompareTag(targetTag.ToString())) // 타겟과 충돌했을 경우
        {
            if (isParticleCollision)
            {
                if (particleCoolDown)
                {
                    Detection = true;
                    StartCoroutine("ParticleDetectionCoolDown");
                }
            }
            else
                Detection = true;
        }
    }
    private bool particleCoolDown = true; // 파티클을 통해 잠깐 동안 너무 많이 감지하는 걸 완화하기 위한 장치.
    private IEnumerator ParticleDetectionCoolDown()
    {
        particleCoolDown = false;

        yield return YieldReturn.WaitForSeconds(0.25f);

        particleCoolDown = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        ExitLogic(collision.gameObject);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        ExitLogic(collision.gameObject);
    }
    private void ExitLogic(GameObject gameObject)
    {
        if (gameObject.CompareTag(targetTag.ToString()))
        {
            Detection = false;
        }
    }

    public event Action DetectionEndEvent = () => { };
    public event Action DetectionStartEvent = () => { };
    public event Action<string> HitGroundEvent = (_) => { };
    protected virtual void DetectionStart()
    {
        DetectionStartEvent.Invoke();
    }
    protected virtual void DetectionEnd()
    {
        DetectionEndEvent.Invoke();
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