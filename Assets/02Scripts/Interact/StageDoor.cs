using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class StageDoor : DoorInteract
{
    [SerializeField]
    private SCENE targetStage;
    [SerializeField]
    private int stageIndex;

    [SerializeField]
    private LeanTweenType leanTweenType = LeanTweenType.linear;
    [SerializeField]
    private float leanTweenTime = 0.5f;

    #region _Stage Info UI_
    [Header("Stage Info UI")]
    [SerializeField]
    private Transform infoBackground;
    [SerializeField]
    private TextMeshPro stageTitle;

    [Space(5)]
    [SerializeField]
    private SpriteRenderer jewelyStar;
    [SerializeField]
    private SpriteRenderer timeStar;
    [SerializeField]
    private SpriteRenderer jumpStar;
    #endregion

    private void Awake()
    {
        StageClearInfo clearInfo = GameManager.Inst.GetClearInfo(stageIndex);
        
        if (clearInfo.clearAtOnce)
        {
            jewelyStar.color = timeStar.color = jumpStar.color = Color.red;
        }
        else
        {
            Color transparent = new Color(0f, 0f, 0f, 0f);

            jewelyStar.color = clearInfo.jewelryClear ? Color.yellow : transparent;
            timeStar.color = clearInfo.timeClear ? Color.yellow : transparent;
            jumpStar.color = clearInfo.jumpClear ? Color.yellow : transparent;
        }

        infoBackground.localScale = Vector2.zero;
    }

    private int infoOpenID = 0;
    protected override void DetectionEnd()
    {
        base.DetectionEnd();

        leanTweenCancle();
        infoOpenID = LeanTween.scale(infoBackground.gameObject, Vector2.zero, infoBackground.localScale.x * leanTweenTime).setEase(leanTweenType).setOnComplete(() => infoOpenID = 0).id;
    }
    protected override void DetectionStart()
    {
        base.DetectionStart();

        leanTweenCancle();
        infoOpenID = LeanTween.scale(infoBackground.gameObject, Vector2.one, (1f - infoBackground.localScale.x) * leanTweenTime).setEase(leanTweenType).setOnComplete(() => infoOpenID = 0).id;
    }
    private void leanTweenCancle()
    {
        LeanTween.cancel(infoOpenID);
    }

    protected override void DoorOpenComplete()
    {
        base.DoorOpenComplete();

        GameManager.Inst.StageIndex = stageIndex;

        GameManager.Inst.SceneMove(targetStage);
    }

    protected override void DoorOpening()
    {

    }

    protected override void DoorClosing()
    {

    }
}
