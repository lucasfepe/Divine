using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private NewUserButton newUserButton;
    [SerializeField] private BackButton backButton;
    [SerializeField] private ResetPasswordBackButton resetPasswordBackButton;
    [SerializeField] private ForgotPasswordButton forgotPasswordButton;

    private void Start()
    {
        newUserButton.OnNewUserButtonClicked += NewUserButton_OnNewUserButtonClicked;
        backButton.OnBackButtonPressed += BackButton_OnBackButtonPressed;
        UIInputManager.Instance.OnSignUp += UIInputManager_OnSignUp; ;
        resetPasswordBackButton.OnResetPasswordBackButtonPressed += ResetPasswordBackButton_OnResetPasswordBackButtonPressed;
        UIInputManager.Instance.OnPasswordReset += UIInputManager_OnPasswordReset;
        forgotPasswordButton.OnForgotPasswordButtonPressed += ForgotPasswordButton_OnForgotPasswordButtonPressed;
    }

    private void ForgotPasswordButton_OnForgotPasswordButtonPressed(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void UIInputManager_OnPasswordReset(object sender, System.EventArgs e)
    {
        Show();
    }

    private void ResetPasswordBackButton_OnResetPasswordBackButtonPressed(object sender, System.EventArgs e)
    {
        Show();
    }

    private void UIInputManager_OnSignUp(object sender, System.EventArgs e)
    {
        Show();
    }

    private void BackButton_OnBackButtonPressed(object sender, System.EventArgs e)
    {
        Show();
    }

    private void NewUserButton_OnNewUserButtonClicked(object sender, System.EventArgs e)
    {
        Hide();
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
