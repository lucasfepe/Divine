using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetPasswordBackButton : MonoBehaviour
{

    public event EventHandler OnResetPasswordBackButtonPressed;
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnResetPasswordBackButtonPressed?.Invoke(this, EventArgs.Empty);
        });
    }
}
