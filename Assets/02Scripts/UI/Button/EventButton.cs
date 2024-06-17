using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class EventButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(ClickEvent);
    }

    protected abstract void ClickEvent();
}
