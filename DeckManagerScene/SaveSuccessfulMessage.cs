using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSuccessfulMessage : MonoBehaviour
{
    private void Start()
    {
        CancelReturnButton.Instance.OnCancelReturnButtonPressed += CancelReturnButton_OnCancelReturnButtonPressed;
        SaveDeckButton.Instance.OnSaveDeck += SaveDeckButton_OnSaveDeck;
        Hide();
    }

    private void SaveDeckButton_OnSaveDeck(object sender, System.EventArgs e)
    {
        Show();
    }

    private void CancelReturnButton_OnCancelReturnButtonPressed(object sender, System.EventArgs e)
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
    private void OnDestroy()
    {
        CancelReturnButton.Instance.OnCancelReturnButtonPressed -= CancelReturnButton_OnCancelReturnButtonPressed;
        SaveDeckButton.Instance.OnSaveDeck -= SaveDeckButton_OnSaveDeck;
    }
}
