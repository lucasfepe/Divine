using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectCardUI : MonoBehaviour
{
    public static InspectCardUI Instance { get; private set; }
    [SerializeField] private Image background;
    [SerializeField] private Button closeButton;
    private ICard activeCard;
    private bool isActiveCardMine;

    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener(() => Hide());
        background.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }
   
    public void Show(ICard card)
    {
        this.activeCard = card;
        background.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        if (background.isActiveAndEnabled) { 
        background.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        activeCard.DoneInspectingCard();
        activeCard = null;
        }
    }
    public bool IsInspectingMyCard()
    {
        return activeCard != null && activeCard.IsCardMine();
    }
    public ICard GetActiveCard()
    {
        return activeCard;
    }
    
    
    
}
