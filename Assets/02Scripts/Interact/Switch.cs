using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class Switch : DetectionBase
{
    [SerializeField]
    private bool oneInteract = true;

    [SerializeField]
    private bool on = false;
    public bool ON
    {
        get => on;
        set
        {
            if (on != value)
            {
                on = value;

                if (on)
                {
                    interactON.Invoke();
                }
                else
                {
                    interactOFF.Invoke();
                }
            }
        }
    }

    private void Awake()
    {
        string fileName = null;
        if (on) // ���� �̺�Ʈ ����
        {
            interactON.Invoke();

            fileName = "Switch_ON";
            GetComponent<SpriteRenderer>().sprite = ResourceLoader.SpriteLoad(FolderName.Ect, fileName);
        }
        else
        {
            interactOFF.Invoke();

            fileName = "Switch_OFF";
            GetComponent<SpriteRenderer>().sprite = ResourceLoader.SpriteLoad(FolderName.Ect, fileName);
        }
    }
    protected override void DetectionEnd()
    {

    }
    protected override void HitGround(string tag)
    {

    }

    private bool interactable = true;
    protected override void DetectionStart()
    {
        if (interactable)
        {
            string fileName = null;
            AudioClip clip = null;

            if (!on) // ���� �־��� ���
            {
                ON = true;

                fileName = "Switch_ON";
                GetComponent<SpriteRenderer>().sprite = ResourceLoader.SpriteLoad(FolderName.Ect, fileName);
                clip = ResourceLoader.AudioLoad(FolderName.Ect, fileName);
            }
            else // ���� �־��� ���
            {
                ON = false;

                fileName = "Switch_OFF";
                GetComponent<SpriteRenderer>().sprite = ResourceLoader.SpriteLoad(FolderName.Ect, fileName);
                clip = ResourceLoader.AudioLoad(FolderName.Ect, fileName);
            }

            EffectManager.Inst.PlaySFX(clip, transform);

            if (oneInteract) // ��ȸ�� ����ġ�� ���
            {
                interactable = false;
            }
        }
    }
    public UnityEvent interactON;
    public UnityEvent interactOFF;
}
