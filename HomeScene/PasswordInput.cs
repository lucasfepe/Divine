using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PasswordInput : MonoBehaviour
{
    [SerializeField] private SeePasswordToggle seePasswordToggle;

    private TMP_InputField passwordInput;

    private void Awake()
    {
        passwordInput = GetComponent<TMP_InputField>(); 
    }

    private void Start()
    {
        seePasswordToggle.OnSeePasswordToggleValueChanged += SeePasswordToggle_OnSeePasswordToggleValueChanged;
    }

    private void SeePasswordToggle_OnSeePasswordToggleValueChanged(object sender, System.EventArgs e)
    {
        if (seePasswordToggle.IsOn())
        {
            passwordInput.contentType = TMP_InputField.ContentType.Standard;

        }else if (!seePasswordToggle.IsOn())
        {
            passwordInput.contentType = TMP_InputField.ContentType.Password;
        }
        passwordInput.ForceLabelUpdate();
    }
}
