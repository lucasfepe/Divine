using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetPasswordButton : MonoBehaviour
{
    public event EventHandler OnResetPasswordButtonPressed;
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnResetPasswordButtonPressed?.Invoke(this, EventArgs.Empty);
        });
    }
}
