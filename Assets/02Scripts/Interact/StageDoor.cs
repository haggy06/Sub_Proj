using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class StageDoor : DoorInteract
{
    [SerializeField]
    private int stageIndex;

    [SerializeField]
    private LeanTweenType leanTweenType = LeanTweenType.linear;
    [SerializeField]
    private float leanTweenTime = 0.5f;

    [Header("Stage Info UI")]
    [SerializeField]
    private Transform infoBackground;
    [SerializeField]
    private Transform Stars;

    private LanguageConverter text;
    protected override void Awake()
    {
        base.Awake();

        if (GameManager.Inst.StageIndex == stageIndex)
            PlayerController.Inst.transform.position = transform.position + new Vector3(2f, 0.6f, 0f);

        StageClearInfo clearInfo = GameManager.Inst.GetClearInfo(stageIndex);
        text = GetComponentInChildren<LanguageConverter>();

        #region _Stage Popup Setting_
        if (stageIndex == 0 || GameManager.Inst.GetClearInfo(stageIndex - 1).stageClear) // 첫 스테이지거나 전 스테이지를 클리어했을 경우
        {
            text.ChangeText(104100, (stageIndex + 1).ToString());
            doorLocked = false;
        }
        else
        {
            text.ChangeText(104101, "");
            doorLocked = true;

            Stars.gameObject.SetActive(false);
        }
        
        if (clearInfo.clearAtOnce)
        {
            Stars.GetChild(0).GetComponent<SpriteRenderer>().color = 
                Stars.GetChild(1).GetComponent<SpriteRenderer>().color = 
                Stars.GetChild(2).GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            Color transparent = new Color(0f, 0f, 0f, 0f);

            Stars.GetChild(0).GetComponent<SpriteRenderer>().color = clearInfo.jewelryClear ? Color.yellow : transparent;
            Stars.GetChild(1).GetComponent<SpriteRenderer>().color = clearInfo.timeClear ? Color.yellow : transparent;
            Stars.GetChild(2).GetComponent<SpriteRenderer>().color = clearInfo.jumpClear ? Color.yellow : transparent;
        }
        #endregion

        infoBackground.localScale = Vector2.zero;
    }

    private int infoOpenID = 0;
    protected override void DetectionEnd()
    {
        base.DetectionEnd();

        LeanTween.cancel(infoOpenID);
        infoOpenID = LeanTween.scale(infoBackground.gameObject, Vector2.zero, infoBackground.localScale.x * leanTweenTime).setEase(leanTweenType).setOnComplete(() => infoOpenID = 0).id;
    }
    protected override void DetectionStart()
    {
        base.DetectionStart();

        LeanTween.cancel(infoOpenID);
        infoOpenID = LeanTween.scale(infoBackground.gameObject, Vector2.one, (1f - infoBackground.localScale.x) * leanTweenTime).setEase(leanTweenType).setOnComplete(() => infoOpenID = 0).id;
    }

    protected override void DoorOpenComplete()
    {
        base.DoorOpenComplete();

        GameManager.Inst.StageIndex = stageIndex;

        GameManager.Inst.SceneMove(SCENE.Tutorial + stageIndex);
    }

    protected override void DoorOpening()
    {

    }

    protected override void DoorClosing()
    {

    }
}
