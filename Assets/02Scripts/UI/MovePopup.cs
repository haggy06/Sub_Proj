using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class MovePopup : PopupBase
{
    [SerializeField]
    private float offPosition = -1300f;
    [SerializeField]
    private float onPosition = 0f;

    public override void PopupOpen(bool reset = false)
    {
        base.PopupOpen(reset);

        openID = LeanTween.moveLocalY(gameObject, onPosition, Mathf.Abs((onPosition - gameObject.transform.localPosition.y) / (onPosition + offPosition)) * fadeDuration).setEase(fadeEase).setOnComplete(CloseComplete).id;
        //openID = LeanTween.move(gameObject, new Vector2(0f, onPosition), Mathf.Abs((onPosition - gameObject.transform.localPosition.y) / (onPosition + offPosition)) * fadeDuration).setEase(fadeEase).setOnComplete(CloseComplete).id;
    }
    public override void PopupShow()
    {
        base.PopupShow();

        transform.localPosition = new Vector2(0f, onPosition);
    }

    public override void PopupClose(bool reset = false)
    {
        base.PopupClose(reset);

        closeID = LeanTween.moveLocalY(gameObject, offPosition, Mathf.Abs((offPosition - gameObject.transform.position.y) / (onPosition + offPosition)) * fadeDuration).setEase(fadeEase).setOnComplete(CloseComplete).id;
        //closeID = LeanTween.move(gameObject, new Vector2(0f, offPosition), Mathf.Abs((offPosition - gameObject.transform.position.y) / (onPosition + offPosition)) * fadeDuration).setEase(fadeEase).setOnComplete(CloseComplete).id;
    }
    public override void PopupHide()
    {
        base.PopupHide();

        transform.localPosition = new Vector2(0f, offPosition);
    }
}
