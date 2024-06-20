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
        transform.GetChild(0).gameObject.SetActive(true); // ù ��° �ڽ� = ��ƼŬ

        transform.root.GetComponent<StageManager>().GetJewelry();
    }

    protected override void HitGround()
    {

    }
}
