using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CancelSaveButton : MonoBehaviour
{
    public static CancelSaveButton Instance { get; private set; }
    public event EventHandler OnCancelSave;

    private void Awake()
    {

        Instance = this;
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnCancelSave?.Invoke(this, EventArgs.Empty);
        });
    }
}
