using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResetPassword : MonoBehaviour
{
    [SerializeField] private ResetPasswordBackButton resetPasswordBackButton;
    [SerializeField] private ResetPasswordButton resetPasswordButton;
    [SerializeField] private TMP_InputField passwordResetCode;
    [SerializeField] private TMP_InputField newPassword;

    private void Start()
    {
        UIInputManager.Instance.OnResetPasswordCodeSent += UIInputManager_OnResetPasswordCodeSent;
        resetPasswordBackButton.OnResetPasswordBackButtonPressed += ResetPasswordBackButton_OnResetPasswordBackButtonPressed;
        resetPasswordButton.OnResetPasswordButtonPressed += ResetPasswordButton_OnResetPasswordButtonPressed;
        UIInputManager.Instance.OnPasswordReset += UIInputManager_OnPasswordReset;
        Hide();
    }

    private void UIInputManager_OnPasswordReset(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void ResetPasswordButton_OnResetPasswordButtonPressed(object sender, System.EventArgs e)
    {
        UIInputManager.Instance.ConfirmResetPassword(passwordResetCode.text, newPassword.text);
    }

    private void ResetPasswordBackButton_OnResetPasswordBackButtonPressed(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void UIInputManager_OnResetPasswordCodeSent(object sender, System.EventArgs e)
    {
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
