using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmSaveButton : MonoBehaviour
{
    public static ConfirmSaveButton Instance { get; private set; }
    public event EventHandler OnConfirmSave;

    private void Awake()
    {

        Instance = this;
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnConfirmSave?.Invoke(this, EventArgs.Empty);
        });
    }
}
