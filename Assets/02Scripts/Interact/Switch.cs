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
        if (on) // 시작 이벤트 실행
        {
            interactON.Invoke();
        }
        else
        {
            interactOFF.Invoke();
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
            if (!on) // 꺼져 있었을 경우
            {
                ON = true;
                GetComponent<SpriteRenderer>().sprite = ResourceLoader.SpriteLoad(FolderName.Ect, "Switch_ON");
            }
            else // 켜져 있었을 경우
            {
                ON = false;
                GetComponent<SpriteRenderer>().sprite = ResourceLoader.SpriteLoad(FolderName.Ect, "Switch_OFF");
            }

            if (oneInteract) // 일회성 스위치일 경우
            {
                interactable = false;
            }
        }
    }
    public UnityEvent interactON;
    public UnityEvent interactOFF;
}
