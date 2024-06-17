using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LanguageConverter : MonoBehaviour
{
    [SerializeField]
    private int textID;

    private void Start() // Inst를 쓰는 것들은 Start에 넣어놔야 오류가 안 난다.
    {
        GameManager.Inst.LanguageChangeEvent += SetText;
        SetText();
    }

    private void SetText()
    {
        GetComponent<TextMeshProUGUI>().text = GameManager.Inst.GetInteractionText(textID);
    }
}
