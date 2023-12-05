using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CancelReturnButton : MonoBehaviour
{
    public static CancelReturnButton Instance { get; private set; }
    public event EventHandler OnCancelReturnButtonPressed;
    private void Awake()
    {
        Instance = this;
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnCancelReturnButtonPressed?.Invoke(this, EventArgs.Empty);
        });
    }
}
