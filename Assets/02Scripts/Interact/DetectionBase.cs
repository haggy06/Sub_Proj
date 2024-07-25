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

        if (other.CompareTag(targetTag.ToString())) // Ÿ�ٰ� �浹���� ���
        {
            if (particleCoolDown)
            {
                DetectionStart();
                StartCoroutine("ParticleDetectionCoolDown");
            }
        }        
        else if (other.layer == (int)LAYER.Ground) // ���� �浹���� ���
        {
            HitGround(other.tag);
        }
    }
    private bool particleCoolDown = true; // ��ƼŬ�� ���� ��� ���� �ʹ� ���� �����ϴ� �� ��ȭ�ϱ� ���� ��ġ.
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

        if (collision.gameObject.layer == (int)LAYER.Ground) // ���� �浹���� ���
        {
            HitGround(collision.gameObject.tag);
        }
        else if (collision.CompareTag(targetTag.ToString())) // Ÿ�ٰ� �浹���� ���
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