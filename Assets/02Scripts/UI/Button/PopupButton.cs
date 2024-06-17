using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class PopupButton : EventButton
{
    [SerializeField]
    private bool isOpen = true;
    [SerializeField]
    private Popup targetPopup;

    protected override void ClickEvent()
    {
        if (isOpen)
        {
            PopupManager.Inst.PopupOpen(targetPopup);
        }
        else
        {
            PopupManager.Inst.PopupClose(targetPopup);
        }
    }
}
