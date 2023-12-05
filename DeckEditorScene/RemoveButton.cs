using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RemoveButton : MonoBehaviour
{
    private Button button;
    private TextMeshProUGUI text;
    [SerializeField] private AddButton addButton;
    public event EventHandler OnRemove;
    [SerializeField] private Transform cardInventory;
    [SerializeField] private Transform deckEditor;
    private void Awake()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        DisableButton();
        addButton.OnAdd += AddButton_OnAdd;
        button.onClick.AddListener(() =>
        {
            OnRemove?.Invoke(this, EventArgs.Empty);
        });
    }
    private void Start()
    {
        BaseCardLocal.OnCardPreview += BaseCardLocal_OnCardPreview; 
    }

    private void BaseCardLocal_OnCardPreview(object sender, EventArgs e)
    {
        BaseCardLocal previewCard = sender as BaseCardLocal;
        string previewCardTitle = previewCard.GetCardSO().Title;
        List<BaseCardLocal> cardsInDeckEditor = deckEditor.GetComponentsInChildren<BaseCardLocal>().ToList();

        if(cardsInDeckEditor.Any(x => x.GetCardSO().Title == previewCardTitle))
        {
            EnableButton();
        }
        else
        {
            DisableButton();
        }
        
    }

    private void OnDestroy()
    {
        addButton.OnAdd -= AddButton_OnAdd;
        BaseCardLocal.OnCardPreview -= BaseCardLocal_OnCardPreview;
    }
    private void AddButton_OnAdd(object sender, System.EventArgs e)
    {
        EnableButton();
    }

    public void DisableButton()
    {
        button.interactable = false;
        text.alpha = .5f;
    }
    public void EnableButton()
    {
        button.interactable = true;
        text.alpha = 1f;
    }
}
