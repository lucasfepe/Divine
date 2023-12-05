using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignUpUI : MonoBehaviour
{

    [SerializeField] private NewUserButton newUserButton;
    [SerializeField] private BackButton backButton;
    private bool isActive = false;
    private void Awake()
    {
        
    }

    private void Start()
    {
        newUserButton.OnNewUserButtonClicked += NewUserButton_OnNewUserButtonClicked;
        backButton.OnBackButtonPressed += BackButton_OnBackButtonPressed;
        UIInputManager.Instance.OnSignUp += UIInputManager_OnSignUp;
        Hide();

    }
    public bool IsActive()
    {
        return isActive;
    }
    private void UIInputManager_OnSignUp(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void BackButton_OnBackButtonPressed(object sender, System.EventArgs e)
    {
        Hide();
        UIInputManager.Instance.SelectFirstField();
    }

    private void NewUserButton_OnNewUserButtonClicked(object sender, System.EventArgs e)
    {
        Show();
        UIInputManager.Instance.SelectFirstField();
    }

    private void Show()
    {
        isActive = true;
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        isActive = false;
        gameObject.SetActive(false);
    }
}
