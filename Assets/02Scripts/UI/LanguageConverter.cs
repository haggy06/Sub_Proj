using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LanguageConverter : MonoBehaviour
{
    [SerializeField]
    private int textID;

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
        GetComponent<TextMeshProUGUI>().text = GameManager.Inst.GetInteractionText(textID);
    }
}
