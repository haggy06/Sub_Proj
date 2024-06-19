using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ResetButton : EventButton
{
    protected override void ClickEvent()
    {
        warningPopup.PopupOpen();
    }

    [SerializeField]
    private GameStartButton startBtn;

    [Space(5)]
    [SerializeField]
    private PopupBase warningPopup;

    [Space(5)]
    [SerializeField]
    private Button agreeBtn;
    [SerializeField]
    private Button disagreeBtn;

    private void Awake()
    {
        warningPopup.PopupClose();

        agreeBtn.onClick.AddListener(() =>
        {
            GameManager.Inst.ResetGameData();
            startBtn.SetTargetScene();

            warningPopup.PopupClose();
        });

        disagreeBtn.onClick.AddListener(() => warningPopup.PopupClose());
    }
}