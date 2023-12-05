using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgotPasswordButton : MonoBehaviour
{
    public event EventHandler OnForgotPasswordButtonPressed;
    private TextMeshProUGUI forgotPasswordButtonText;

    private void Awake()
    {
        forgotPasswordButtonText = GetComponentInChildren<TextMeshProUGUI>();
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if(UIInputManager.Instance.CheckResetRequest())
            OnForgotPasswordButtonPressed?.Invoke(this, EventArgs.Empty);
        });
    }
    private void OnMouseEnter()
    {
        forgotPasswordButtonText.color = Color.red;
    }
    private void OnMouseExit()
    {
        forgotPasswordButtonText.color = Color.blue;
    }


}
