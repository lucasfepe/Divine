using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddButton : MonoBehaviour
{
    private Button button;
    private TextMeshProUGUI text;
    private PlayerCards playerCards;
    public event EventHandler OnAdd;
    [SerializeField] private Transform cardInventory;
    [SerializeField] private Transform deckEditor;
    private void Awake()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        button.onClick.AddListener(() =>
        {
            OnAdd?.Invoke(this, EventArgs.Empty);
        });
        DisableButton();
    }
    private void Start()
    {
        BaseCardLocal.OnCardPreview += BaseCardLocal_OnCardPreview;
        playerCards = CardInventory.Instance.GetPlayerCardsObject();
    }
    private void OnDestroy()
    {
        BaseCardLocal.OnCardPreview -= BaseCardLocal_OnCardPreview;
    }
    private void BaseCardLocal_OnCardPreview(object sender, System.EventArgs e)
    {
        BaseCardLocal previewCard = sender as BaseCardLocal;
        string previewCardTitle = previewCard.GetCardSO().Title;
        List<BaseCardLocal> cardsInInventory = cardInventory.GetComponentsInChildren<BaseCardLocal>().ToList();
        BaseCardLocal cardInInventory = cardsInInventory.Find(x => x.GetCardSO().Title == previewCardTitle);

        if (Int32.Parse(cardInInventory.GetCounterText()) > 0)
        { EnableButton(); }
        else
        {
            DisableButton();
        }
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
