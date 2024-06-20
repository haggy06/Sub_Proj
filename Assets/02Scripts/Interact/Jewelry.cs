using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jewelry : DetectionBase
{
    protected override void DetectionEnd()
    {

    }

    protected override void DetectionStart()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(true); // 첫 번째 자식 = 파티클

        transform.root.GetComponent<StageManager>().GetJewelry();
    }

    protected override void HitGround()
    {

    }
}
