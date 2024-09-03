using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : EntityEvent // 너, UnityEvent 가능성 있어
{
    /*
    [SerializeField]
    private LeanTweenType spearOutType = LeanTweenType.linear;
    [SerializeField, Range(0.01f, 2f)]
    private float spearSpeed = 0.25f;

    [Space(5)]
    [SerializeField]
    private float outPosition = 1f;

    private int spearTweenID = 0;
    public override void Run()
    {
        LeanTween.cancel(spearTweenID);
        spearTweenID = LeanTween.moveLocal(gameObject, Vector2.up * outPosition, spearSpeed).setEase(spearOutType).setOnComplete(() => spearTweenID = 0).id; // 창 튀어나옴
    }

    public override void RunStop()
    {
        LeanTween.cancel(spearTweenID);
        spearTweenID = LeanTween.moveLocal(gameObject, Vector2.zero, spearSpeed).setEase(spearOutType).setOnComplete(() => spearTweenID = 0).id; // 창 집어넣음
    }
    */
}
