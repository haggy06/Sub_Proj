using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LanguageConverter : MonoBehaviour
{
    [SerializeField]
    private int textID;

    private void Start() // Inst�� ���� �͵��� Start�� �־���� ������ �� ����.
    {
        GameManager.Inst.LanguageChangeEvent += SetText;
        SetText();
    }

    private void SetText()
    {
        GetComponent<TextMeshProUGUI>().text = GameManager.Inst.GetInteractionText(textID);
    }
}
