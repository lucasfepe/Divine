using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveWarningMessage : MonoBehaviour
{
    public static SaveWarningMessage Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SaveDeckButton.Instance.OnSaveExistingDeck += SaveDeckButton_OnSaveExistingDeck;
        CancelSaveButton.Instance.OnCancelSave += CancelSaveButton_OnCancelSave;
        ConfirmSaveButton.Instance.OnConfirmSave += ConfirmSaveButon_OnConfirmSave;
        Hide();
    }

    private void ConfirmSaveButon_OnConfirmSave(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void CancelSaveButton_OnCancelSave(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void SaveDeckButton_OnSaveExistingDeck(object sender, System.EventArgs e)
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
    private void OnDestroy()
    {
        SaveDeckButton.Instance.OnSaveExistingDeck -= SaveDeckButton_OnSaveExistingDeck;
        CancelSaveButton.Instance.OnCancelSave -= CancelSaveButton_OnCancelSave;
        ConfirmSaveButton.Instance.OnConfirmSave -= ConfirmSaveButon_OnConfirmSave;
    }
}
