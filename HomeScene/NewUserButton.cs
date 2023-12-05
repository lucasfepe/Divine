using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewUserButton : MonoBehaviour
{

    public event EventHandler OnNewUserButtonClicked;
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnNewUserButtonClicked?.Invoke(this, new EventArgs());
            
        });
    }
}
