using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePopup : FadePopup
{
    public override void PopupOpen(bool reset = false)
    {
        base.PopupOpen(reset);

        Time.timeScale = 0f;
    }
    public override void PopupClose(bool reset = false)
    {
        base.PopupClose(reset);

        Time.timeScale = 1f;
    }

    public override void PopupShow()
    {
        base.PopupShow();

        Time.timeScale = 0f;
    }
    public override void PopupHide()
    {
        base.PopupHide();

        Time.timeScale = 1f;
    }
}
