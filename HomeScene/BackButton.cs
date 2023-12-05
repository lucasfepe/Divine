using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    public event EventHandler OnBackButtonPressed;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => {
            OnBackButtonPressed?.Invoke(this, EventArgs.Empty);
        });
    }
}
