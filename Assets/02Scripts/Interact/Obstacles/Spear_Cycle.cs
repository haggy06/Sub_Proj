using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear_Cycle : CycleObstacle
{
    [SerializeField]
    private LeanTweenType spearOutType = LeanTweenType.linear;
    [SerializeField, Range(0.01f, 2f)]
    private float spearSpeed = 0.25f;

    [Space(5)]
    [SerializeField]
    private Vector2 inPosition = new Vector2(0f, -1f);
    [SerializeField]
    private Vector2 outPosition = new Vector2(0f, -0.5f);

    private int spearTweenID = 0;
    protected override void Run()
    {
        LeanTween.cancel(spearTweenID);
        spearTweenID = LeanTween.moveLocal(gameObject, outPosition, spearSpeed).setEase(spearOutType).setOnComplete(() => spearTweenID = 0).id; // 창 튀어나옴
    }

    protected override void RunStop()
    {
        LeanTween.cancel(spearTweenID);
        spearTweenID = LeanTween.moveLocal(gameObject, inPosition, spearSpeed).setEase(spearOutType).setOnComplete(() => spearTweenID = 0).id; // 창 집어넣음
    }
}
