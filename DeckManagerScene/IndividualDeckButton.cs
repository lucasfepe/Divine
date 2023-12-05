using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IndividualDeckButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI deckTitleText;
    [SerializeField] private TextMeshProUGUI cardNumberText;

    private string deckTitle;

    public static event EventHandler OnCreateDeckUIButton;
    public event EventHandler<OnSelectDeckEventArgs> OnSelectDeck;
    public class OnSelectDeckEventArgs : EventArgs
    {
        public string deckTitle;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => {
            OnSelectDeck?.Invoke(this, new OnSelectDeckEventArgs
            {
                deckTitle = deckTitle
            });
        });
        OnCreateDeckUIButton?.Invoke(this, EventArgs.Empty);
        Image image = GetComponent<Image>();
        //image.material = new Material(image.material);
        //image.material.SetFloat("_ColorChangeTolerance", 1);
        
    }
    public void SetDeckTitle(string title)
    {
        deckTitle = title;
        deckTitleText.text = title;
    }
    public void SetCardNumber(int cardNumber)
    {
        cardNumberText.text = "CARDS: " + cardNumber;
    }
    public string GetDeckTitle()
    {
        return deckTitle;
    }
    

}
