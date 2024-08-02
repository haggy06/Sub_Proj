using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class LanguageConverter : MonoBehaviour
{
    [SerializeField]
    private int textID;
    public int TextID
    {
        get => textID;
        set
        {
            textID = value;
            SetText();
        }
    }

    private void Awake()
    {
        GameManager.Inst.LanguageChangeEvent += SetText;
        SetText();
    }
    private void OnDestroy()
    {
        GameManager.Inst.LanguageChangeEvent -= SetText;
    }

    private void SetText()
    {
        GetComponent<TMP_Text>().text = GameManager.Inst.GetInteractionText(textID);
    }
}
