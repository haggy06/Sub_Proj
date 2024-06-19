using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class DetectionBase : MonoBehaviour
{
    [SerializeField]
    private string targetTag = "Player";
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            Detection = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Detection = false;
    }

    protected abstract void DetectionStart();
    protected abstract void DetectionEnd();
}
