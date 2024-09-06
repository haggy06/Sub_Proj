using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialText : LanguageConverter
{
    [Space(10)]
    [SerializeField]
    private int clickONTextID;
    [SerializeField]
    private int clickOFFTextID;

    private void Start()
    {
        DragManager.MouseClickEvent += MouseON;
        DragManager.MouseUpEvent += MouseOFF;
    }
    private void OnDestroy()
    {
        DragManager.MouseClickEvent -= MouseON;
        DragManager.MouseUpEvent -= MouseOFF;
    }

    private void MouseON()
    {
        ChangeText(clickONTextID);
    }
    private void MouseOFF(Vector2 vec, float f)
    {
        ChangeText(clickOFFTextID);
    }
}
