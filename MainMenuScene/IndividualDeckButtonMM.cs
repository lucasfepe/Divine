using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IndividualDeckButtonMM : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI deckTitleText;
    [SerializeField] private TextMeshProUGUI cardNumberText;

    private string deckTitle;
    private Button button;
    public static event EventHandler OnCreateDeckUIButton;
    public event EventHandler<OnSelectDeckEventArgs> OnSelectDeck;
    private Material material;
    private bool isSelected;
    public class OnSelectDeckEventArgs : EventArgs
    {
        public string deckTitle;
    }
    public void OnSelect(BaseEventData eventData)
    {
        if (!IsActiveDeck()) { 
            isSelected = true; 
            material.SetFloat("_Brightness", .08f);
        }
    }
    public void OnDeselect(BaseEventData eventData)
    {
        if (!IsActiveDeck()) { 
            material.SetFloat("_Brightness", .0f);
            isSelected = false;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsActiveDeck() && !isSelected)
            material.SetFloat("_Brightness", .04f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsActiveDeck() && !isSelected)
            material.SetFloat("_Brightness", .0f);
    }

    private bool IsActiveDeck()
    {
        return material.GetFloat("_ColorChangeTolerance") == 0;
    }

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => {
            OnSelectDeck?.Invoke(this, new OnSelectDeckEventArgs
            {
                deckTitle = deckTitle
            });
        });
        

        OnCreateDeckUIButton?.Invoke(this, EventArgs.Empty);
        Image image = GetComponent<Image>();
        image.material = new Material(image.material);
        material = image.material;
        material.SetFloat("_ColorChangeTolerance", 1);
    }
   
    public void SetDeckTitle(string title)
    {
        deckTitle = title;
        deckTitleText.text = title;
    }
    public void SetCardNumber(int cardNumber)
    {
        cardNumberText.text = "Cards: " + cardNumber;
    }
    public string GetDeckTitle()
    {
        return deckTitle;
    }

    
}
