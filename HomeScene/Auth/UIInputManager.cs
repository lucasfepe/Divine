using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using System;

// Manages all the text and button inputs
// Also acts like the main manager script for the game.
public class UIInputManager : MonoBehaviour
{
    public static UIInputManager Instance { get; private set; }
    public static string CachePath;

    public event EventHandler OnSignUp;
    public event EventHandler OnResetPasswordCodeSent;
    public event EventHandler OnPasswordReset;

    [SerializeField] private Message message;
    
    [SerializeField] private Button loginButton;
    [SerializeField] private Button newUserButton;
    [SerializeField] private Button signupButton;
    [SerializeField] private Button backButton;

    [SerializeField] private ForgotPasswordButton forgotPasswordButton;

    //public Button startButton;
    //public Button logoutButton;
    [SerializeField] private TMP_InputField emailFieldLogin;
    [SerializeField] private TMP_InputField passwordFieldLogin;
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;

    [SerializeField] private LoginUI loginUI;
    [SerializeField] private SignUpUI signUpUI;


    private AuthenticationManager _authenticationManager;
    //private GameObject _unauthInterface;
    //private GameObject _authInterface;
    //private GameObject _loading;
    //private GameObject _welcome;
    //private GameObject _confirmEmail;
    //private GameObject _signupContainer;

    private List<Selectable> _fieldsLogin;
    private List<Selectable> _fieldsSignUp;
    private int _selectedFieldIndex = 0;

    public void ResetSelectedField()
    {
        _selectedFieldIndex = 0;
    }

    private void displayComponentsFromAuthStatus(bool authStatus)
    {
        if (authStatus)
        {

            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
            //_loading.SetActive(false);
            //_unauthInterface.SetActive(false);
            //_authInterface.SetActive(true);
            //_welcome.SetActive(true);
        }
        else
        {
            //_loading.SetActive(false);
            //_unauthInterface.SetActive(true);
            //_authInterface.SetActive(false);
        }

        // clear out passwords
        passwordFieldLogin.text = "";
        passwordField.text = "";

        // set focus to email field on login form
        _selectedFieldIndex = 0;
    }

    private async void onLoginClicked()
    {
        //_unauthInterface.SetActive(false);
        //_loading.SetActive(true);
        string loginResponse = await _authenticationManager.Login(emailFieldLogin.text, passwordFieldLogin.text);

        bool status = loginResponse == "OK";
        displayComponentsFromAuthStatus(status);
        if(loginResponse == "OK")
        {
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        }
        else
        {
            message.SetMessageText(loginResponse);
            ResetSelectedField();
            SelectFirstField();
        }
        
    }

    private async void onSignupClicked()
    {
        //_unauthInterface.SetActive(false);
        //_loading.SetActive(true);

        string signupResponse = await _authenticationManager.Signup(usernameField.text, emailField.text, passwordField.text);
        bool successfulSignup = signupResponse == "OK";

        if (successfulSignup)
        {
            message.SetMessageText("Signup Successful! Confirm account by clicking link in email to login.");
            // here we re-enable the whole auth container but hide the sign up panel
            //_signupContainer.SetActive(false);

            //_confirmEmail.SetActive(true);

            // copy over the new credentials to make the process smoother
            emailFieldLogin.text = emailField.text;
            passwordFieldLogin.text = passwordField.text;

            // set focus to email field on login form
            _selectedFieldIndex = 0;
            ResetSelectedField();
            //select login button
            _fieldsLogin[2].Select();
            OnSignUp?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            //_confirmEmail.SetActive(false);
            message.SetMessageText(signupResponse);
            // set focus to email field on signup form
            ResetSelectedField();
            SelectFirstField();
        }

        //_loading.SetActive(false);
        //_unauthInterface.SetActive(true);
    }

    private void onLogoutClick()
    {
        _authenticationManager.SignOut();
        displayComponentsFromAuthStatus(false);
    }

    private void onStartClick()
    {
        //SceneManager.LoadScene("GameScene");

        // call to lambda to demonstrate use of credentials
        //_lambdaManager.ExecuteLambda();
    }

    private async void RefreshToken()
    {
        bool successfulRefresh = await _authenticationManager.RefreshSession();
        displayComponentsFromAuthStatus(successfulRefresh);
    }

    void Start()
    {
        // check if user is already authenticated 
        // We perform the refresh here to keep our user's session alive so they don't have to keep logging in.
        RefreshToken();

        signupButton.onClick.AddListener(onSignupClicked);
        loginButton.onClick.AddListener(onLoginClicked);
        forgotPasswordButton.OnForgotPasswordButtonPressed += ForgotPasswordButton_OnForgotPasswordButtonPressed;
        //startButton.onClick.AddListener(onStartClick);
        //logoutButton.onClick.AddListener(onLogoutClick);

        _fieldsLogin[0].Select();

    }
    public bool CheckResetRequest()
    {
        if (emailFieldLogin.text == "")
        {
            message.SetMessageText("Provide an email to send the reset code to.");
            return false;
        }
        else { return true; }
    }
    private async void ForgotPasswordButton_OnForgotPasswordButtonPressed(object sender, EventArgs e)
    {
        
       await _authenticationManager.ForgotPassword(emailFieldLogin.text);
        message.SetMessageText("Reset password code sent to email: '" + emailFieldLogin.text + "'");
        OnResetPasswordCodeSent?.Invoke(this, EventArgs.Empty);
    }

    public async void ConfirmResetPassword(string code, string newPass)
    {
       string responseMessage =  await _authenticationManager.ConfirmResetPassword(code, newPass);
        bool isOK = responseMessage == "OK";
        if (isOK)
        {
            message.SetMessageText("Password reset successfuly");
            OnPasswordReset?.Invoke(this, EventArgs.Empty);

        }
        else
        {
            message.SetMessageText(responseMessage);
        }
    }

    public void SelectFirstField()
    {
        if(!signUpUI.IsActive())
        {
            _fieldsLogin[0].Select();
        }else if(signUpUI.IsActive())
        {
            _fieldsSignUp[0].Select();
        }
    }

    void Update()
    {
        HandleInputTabbing();
    }

    // Handles tabbing between inputs and buttons
    private void HandleInputTabbing()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CheckForAndSetManuallyChangedIndex();
            // update index to where we need to tab to
            _selectedFieldIndex++;

            if (!signUpUI.IsActive())
            {
                if (_selectedFieldIndex >= _fieldsLogin.Count)
                {
                    // reset back to first input
                    _selectedFieldIndex = 0;
                }
                _fieldsLogin[_selectedFieldIndex].Select();
            }else if (signUpUI.IsActive())
            {
                if (_selectedFieldIndex >= _fieldsSignUp.Count)
                {
                    // reset back to first input
                    _selectedFieldIndex = 0;
                }
                _fieldsSignUp[_selectedFieldIndex].Select();
            }
            
        }
    }

    // If the user selects an input via mouse click, then the _selectedFieldIndex 
    // may not be accurate as the focused field wasn't change by tabbing. Here we
    // correct the _selectedFieldIndex in case they wish to start tabing from that point on.
    private void CheckForAndSetManuallyChangedIndex()
    {
        if (signUpUI.IsActive())
        {
            for (var i = 0; i < _fieldsSignUp.Count; i++)
            {
                if (_fieldsSignUp[i] is TMP_InputField && ((TMP_InputField)_fieldsSignUp[i]).isFocused && _selectedFieldIndex != i)
                {
                    _selectedFieldIndex = i;
                    break;
                }
            }
        }
        else if (!signUpUI.IsActive())
        {
            for (var i = 0; i < _fieldsLogin.Count; i++)
            {
                
                if (_fieldsLogin[i] is TMP_InputField && ((TMP_InputField)_fieldsLogin[i]).isFocused && _selectedFieldIndex != i)
                {
                    _selectedFieldIndex = i;
                    break;
                }
            }
        }

    }

    void Awake()
    {
        Instance = this;
        CachePath = Application.persistentDataPath;

        //_unauthInterface = GameObject.Find("UnauthInterface");
        //_authInterface = GameObject.Find("AuthInterface");
        //_loading = GameObject.Find("Loading");
        //_welcome = GameObject.Find("Welcome");
        //_confirmEmail = GameObject.Find("ConfirmEmail");
        //_signupContainer = GameObject.Find("SignupContainer");

        //_unauthInterface.SetActive(false); 
        // start as false so we don't just show the login screen during attempted token refresh
        //_authInterface.SetActive(false);
        //_welcome.SetActive(false);
        //_confirmEmail.SetActive(false);
        //_signupContainer.SetActive(true);

        _authenticationManager = FindObjectOfType<AuthenticationManager>();

        _fieldsLogin = new List<Selectable> { emailFieldLogin, passwordFieldLogin, loginButton, newUserButton };
        _fieldsSignUp = new List<Selectable> {  emailField, usernameField, passwordField, signupButton, backButton };

        newUserButton.GetComponent<NewUserButton>().OnNewUserButtonClicked += NewUserButton_OnNewUserButtonClicked;

    }

    private void NewUserButton_OnNewUserButtonClicked(object sender, EventArgs e)
    {
        ResetSelectedField();
    }
}