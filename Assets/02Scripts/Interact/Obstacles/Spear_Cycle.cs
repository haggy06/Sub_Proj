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
    [SerializeField, Range(1, 2)]
    private int outPosition = 1;

    private int spearTweenID = 0;
    protected override void Run()
    {
        LeanTween.cancel(spearTweenID);
        spearTweenID = LeanTween.moveLocal(gameObject, Vector2.up * outPosition, spearSpeed).setEase(spearOutType).setOnComplete(() => spearTweenID = 0).id; // 창 튀어나옴
    }

    protected override void RunStop()
    {
        LeanTween.cancel(spearTweenID);
        spearTweenID = LeanTween.moveLocal(gameObject, Vector2.zero, spearSpeed).setEase(spearOutType).setOnComplete(() => spearTweenID = 0).id; // 창 집어넣음
    }
}
