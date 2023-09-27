using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BaseCardLocal : MonoBehaviour, ICard
{
    //how do I keep track of this card's stardust and light with a network variable in case a player disconnects 
    //if I make is a network Behaviour it will make two of it, can I have two of it but in different places in the two different players??
    //public event EventHandler<OnCardHoverEventArgs> OnCardHover;
    public event EventHandler OnCardHoverEnter;
    public event EventHandler OnCardHoverExit;
    public event EventHandler OnCardSOAssigned;

    protected CardSO cardSO;
    protected CardUI cardUI;
    protected RectTransform rectTransform;
    private int cardStardust;
    private int cardLight;
    private IPlayer player;
    private GameAreaEnum gameArea;
    private Transform hand;

    [SerializeField] private SelectedCardVisual selectedCardVisual;



    public void PlayCard()
    {
        player.SetStardust(player.GetStardust() - cardStardust);
        player.SetLight(player.GetLight() + cardLight);
        DivineMultiplayer.Instance.PlayCardServerRpc(cardSO.Title, Player.Instance.IAm());
        Destroy(gameObject);
    }

    protected void CallOnCardSOAssigned(){
OnCardSOAssigned?.Invoke(this, EventArgs.Empty);
}

    public void PlaceCardInField()
    {
        //want to do this if card is mine
       
            player.SetStardust(player.GetStardust() - cardStardust);
                player.SetLight(player.GetLight() + cardLight);
                Transform emptyExpertCardSlot = PlayerPlayingField.Instance.GetFirstOpenExpertCardPosition();
                cardUI.GlowLight();
                SetCardGameArea(GameAreaEnum.Field);
                transform.SetParent(emptyExpertCardSlot);
                //Instead of adding it to a list and having the other play client make the card there, make the card as a network object here (in a server rpc) and have a client rpc to reparent the card based on whether it is I who played this card or my opponent
                //AddToFieldCardList(card.GetCardSO().Title);
                RectTransform rectTransform = transform.gameObject.GetComponent<RectTransform>();
                rectTransform.position = emptyExpertCardSlot.position;
    }
    virtual protected void Awake()
    {
        hand = GameObject.Find("PlayerHand").transform;
        gameArea = GameAreaEnum.Hand;
        cardUI = GetComponentInChildren<CardUI>();
        rectTransform = GetComponent<RectTransform>();
    }
    protected void Start()
    {
        player = CardGameManager.Instance.GetPlayer();
        
        
    }

    
    
    virtual public void InspectCard()
    {
        InspectCardUI inspectCardUI1 = GameObject.Find("InspectCardUI").GetComponent<InspectCardUI>();

        inspectCardUI1.Show(this);

        transform.SetParent(inspectCardUI1.transform);
        rectTransform.anchorMax = new Vector2(.5f, .5f);
        rectTransform.anchorMin = new Vector2(.5f, .5f);
        rectTransform.position = Vector2.zero;
        
        SetCardGameArea(GameAreaEnum.Inspect);
        //turn off highlight
        selectedCardVisual.Hide();
    }
    virtual public void DoneInspectingCard()
    {
        if (GetCardGameArea() == GameAreaEnum.Inspect)
        {
            transform.SetParent(hand);
            rectTransform.anchorMax = new Vector2(.5f, .5f);
            rectTransform.anchorMin = new Vector2(.5f, .5f);

            rectTransform.localPosition = Vector2.zero;
            float scale;
           
                scale = UniversalConstants.HAND_CARD_SCALE;
                SetCardGameArea(GameAreaEnum.Hand);
           
            
            

        }
    }
    private void OnMouseEnter()
    {
        OnCardHoverEnter?.Invoke(this, EventArgs.Empty);
        
    }
    private void OnMouseExit()
    {
        OnCardHoverExit?.Invoke(this, EventArgs.Empty);
    }
    virtual public void SetCardSO(CardSO cardSO)
    {
        this.cardSO = cardSO;
        //This is quite sneakily put in here
        //if I just set cardSO = cardSO instead of calling this method
        //light and stardust will never be set...
        cardStardust = cardSO.Stardust;
        cardLight = cardSO.Light;

        
        //Show();



    }
    public CardSO GetCardSO()
    {
        return cardSO;
    }
    public GameAreaEnum GetCardGameArea()
    {
        return gameArea;
    }
    public void SetCardGameArea(GameAreaEnum gameArea)
    {
        this.gameArea = gameArea;
        cardUI.AdaptLook();
    }
    
    
   
    
    
    
    virtual public bool IsCardDestroyed()
    {
        return false;
    }
  

    
    virtual public bool IsGiant()
    {
        return false;
    }

    virtual public void TargetSelected(BaseCard targetCard) { }
    virtual public ViableTarget GetViableTarget() {  return null; }

   
    public int GetStardust()
    {
        return cardStardust;
    }
  
    


    
    public int GetCardLight()
    {
        return cardLight;
    }

    public CardUI GetCardUI()
    {
        return cardUI;
    }

   
    private int GetIndex()
    {
        return PlayerPlayingField.Instance
                .GetAllPlayerExpertCards()
                .FindIndex(x => x == this);
    }

    public bool IsCardMine()
    {
        return true;
    }

    public int GetCardStardust()
    {
        return cardStardust;
    }

    virtual public int GetLifetime()
    {
        return 0;
    }
}
