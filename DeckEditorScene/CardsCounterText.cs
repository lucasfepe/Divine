using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardsCounterText : MonoBehaviour
{
    TextMeshProUGUI text;
    [SerializeField] private AddButton addButton;
    [SerializeField] private RemoveButton removeButton;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();

    }
    private void Start()
    {
        addButton.OnAdd += UpdateText;
        removeButton.OnRemove += UpdateText;
        DeckEditorAreaContent.Instance.OnDeckToEditCardsAssigned += OnDeckToEditCardsAssigned;
    }

    private void OnDeckToEditCardsAssigned(object sender, System.EventArgs e)
    {
        text.text = "Size: " + DeckEditorAreaContent.Instance.CardsInEditDeck();
    }

    private void UpdateText(object sender, System.EventArgs e)
    {
        SetText();
    }

    public void SetText()
    {
        
        text.text = "Size: " + DeckEditorAreaContent.Instance.GetCardsCount();
    }
}
