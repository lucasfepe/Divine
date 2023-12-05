using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteDeckButton : MonoBehaviour
{
    public event EventHandler OnDeleteDeck;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnDeleteDeck?.Invoke(this, EventArgs.Empty);
            DecksManager.Instance.DeleteDeck(DecksManager.Instance.GetSelectedDeckTitle());
            
        });
    }
}
