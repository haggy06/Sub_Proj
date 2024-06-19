using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FadePopup : PopupBase
{
    public override void PopupOpen(bool reset = false)
    {
        base.PopupOpen(reset);

        openID = LeanTween.alphaCanvas(canvas, 1f, (1f - canvas.alpha) * fadeDuration).setEase(fadeEase).setOnComplete(OpenComplete).setIgnoreTimeScale(true).id;
    }
    public override void PopupShow()
    {
        base.PopupShow();

        canvas.alpha = 1f;
    }

    public override void PopupClose(bool reset = false)
    {
        base.PopupClose(reset);

        closeID = LeanTween.alphaCanvas(canvas, 0f, canvas.alpha * fadeDuration).setEase(fadeEase).setOnComplete(CloseComplete).setIgnoreTimeScale(true).id;
    }
    public override void PopupHide()
    {
        base.PopupHide();

        canvas.alpha = 0f;
    }
}
