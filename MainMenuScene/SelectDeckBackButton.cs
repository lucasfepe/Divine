using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectDeckBackButton : MonoBehaviour
{
    public event EventHandler OnSelectDeckBackButtonPressed;
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => OnSelectDeckBackButtonPressed?.Invoke(this, EventArgs.Empty));
    }
}
