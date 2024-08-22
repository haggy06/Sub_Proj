using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PopupBase : MonoBehaviour
{
    [SerializeField]
    protected float fadeDuration = 0.5f;
    public float FadeDuration => fadeDuration;

    [SerializeField]
    protected LeanTweenType fadeEase = LeanTweenType.linear;
    
    protected CanvasGroup canvas;
    protected virtual void Awake()
    {
        canvas = GetComponent<CanvasGroup>();
    }

    // 서서히 열고 닫음
    protected int openID = 0;
    protected int closeID = 0;
    protected virtual void FadeCancle()
    {
        LeanTween.cancel(openID);
        LeanTween.cancel(closeID);
    }

    public virtual void PopupOpen(bool reset = false)
    {
        FadeCancle();
        if (reset)
        {
            PopupHide();
        }

        canvas.blocksRaycasts = true;
    }
    protected virtual void OpenComplete()
    {
        openID = 0;
    }

    public virtual void PopupClose(bool reset = false)
    {
        FadeCancle();
        if (reset)
        {
            PopupShow();
        }

        canvas.blocksRaycasts = false;
    }
    protected virtual void CloseComplete()
    {
        closeID = 0;
    }

    // 즉시 열고 닫음
    public virtual void PopupShow()
    {
        FadeCancle();

        canvas.blocksRaycasts = true;
    }
    public virtual void PopupHide()
    {
        FadeCancle();

        canvas.blocksRaycasts = false;
    }
}