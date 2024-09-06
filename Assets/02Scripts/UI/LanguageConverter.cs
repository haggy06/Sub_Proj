using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class LanguageConverter : MonoBehaviour
{
    [SerializeField]
    private int textID;
    [SerializeField]
    private string addText; 

    public void ChangeText(int textID)
    {
        this.textID = textID;

        SetText();
    }
    public void ChangeText(int textID, string addText)
    {
        this.textID = textID;
        this.addText = addText;

        SetText();
    }


    private void Awake()
    {
        GameManager.LanguageChangeEvent += SetText;
        SetText();
    }
    private void OnDestroy()
    {
        GameManager.LanguageChangeEvent -= SetText;
    }

    private void SetText()
    {
        GetComponent<TMP_Text>().text = GameManager.Inst.GetInteractionText(textID) + addText;
    }
}
