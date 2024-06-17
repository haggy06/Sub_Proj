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
        //openID = LeanTween.alphaCanvas(canvas, 1f, (1f - canvas.alpha) * fadeDuration).setEase(fadeEase).setOnComplete(OpenComplete).id;
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
        //closeID = LeanTween.alphaCanvas(canvas, 0f, canvas.alpha * fadeDuration).setEase(fadeEase).setOnComplete(CloseComplete).id;
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
        //canvas.alpha = 1f;
    }
    public virtual void PopupHide()
    {
        FadeCancle();

        canvas.blocksRaycasts = false;
        //canvas.alpha = 0f;
    }
}