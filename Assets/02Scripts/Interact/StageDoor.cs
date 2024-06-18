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
    private Transform jewelyStar;
    [SerializeField]
    private Transform timeStar;
    [SerializeField]
    private Transform jumpStar;
    #endregion
    private void Awake()
    {
        infoBackground.localScale = Vector2.zero;
        jewelyStar.localScale = Vector2.zero;
        timeStar.localScale = Vector2.zero;
        jumpStar.localScale = Vector2.zero;
    }

    private int infoOpenID = 0;
    private int jewelyStarID = 0;
    private int timeStarID = 0;
    private int jumpStarID = 0;
    protected override void DetectionEnd()
    {
        leanTweenCancle();

        infoOpenID = LeanTween.scale(infoBackground.gameObject, Vector2.zero, infoBackground.localScale.x * leanTweenTime).setEase(leanTweenType).setOnComplete(() => infoOpenID = 0).id;
        jewelyStarID = LeanTween.scale(jewelyStar.gameObject, Vector2.zero, jewelyStar.localScale.x * leanTweenTime).setEase(leanTweenType).setOnComplete(() => jewelyStarID = 0).id;
        timeStarID = LeanTween.scale(timeStar.gameObject, Vector2.zero, timeStar.localScale.x * leanTweenTime).setEase(leanTweenType).setOnComplete(() => timeStarID = 0).id;
        jumpStarID = LeanTween.scale(jumpStar.gameObject, Vector2.zero, jumpStar.localScale.x * leanTweenTime).setEase(leanTweenType).setOnComplete(() => jumpStarID = 0).id;
    }
    protected override void DetectionStart()
    {
        leanTweenCancle();

        infoOpenID = LeanTween.scale(infoBackground.gameObject, Vector2.one, (1f - infoBackground.localScale.x) * leanTweenTime).setEase(leanTweenType).setOnComplete(() => infoOpenID = 0).id;
        jewelyStarID = LeanTween.scale(jewelyStar.gameObject, Vector2.one, (1f - jewelyStar.localScale.x) * leanTweenTime).setEase(leanTweenType).setOnComplete(() => jewelyStarID = 0).id;
        timeStarID = LeanTween.scale(timeStar.gameObject, Vector2.one, (1f - timeStar.localScale.x) * leanTweenTime).setEase(leanTweenType).setOnComplete(() => timeStarID = 0).id;
        jumpStarID = LeanTween.scale(jumpStar.gameObject, Vector2.one, (1f - jumpStar.localScale.x) * leanTweenTime).setEase(leanTweenType).setOnComplete(() => jumpStarID = 0).id;
    }
    private void leanTweenCancle()
    {
        LeanTween.cancel(infoOpenID);
        LeanTween.cancel(jewelyStarID);
        LeanTween.cancel(timeStarID);
        LeanTween.cancel(jumpStarID);
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
