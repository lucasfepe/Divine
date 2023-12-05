using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeePasswordToggle : MonoBehaviour
{

    public event EventHandler OnSeePasswordToggleValueChanged;
    private Toggle toggle;
    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(value =>
        {
            OnSeePasswordToggleValueChanged?.Invoke(this, EventArgs.Empty);
        });
    }
    public bool IsOn()
    {
        return toggle.isOn;
    }
}
