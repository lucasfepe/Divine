using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerHand : NetworkBehaviour
{
    public static PlayerHand Instance { get; private set; }
    [SerializeField] private BaseCard expertCard;
    [SerializeField] private BaseCard civilizationCard;
    [SerializeField] private BaseCard subterfugeCard;
    [SerializeField] private LoadingTextUI loadingText;
    public int cardsOnHand;

    
    public event EventHandler OnDrawCard;
    


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        PlayerDeck.Instance.OnFetchActiveDeck += PlayerDeck_OnFetchActiveDeck;
        CardGameManager.Instance.OnBeginTurn += CardGameManager_OnBeginTurn;
        PlayerPlayingField.Instance.OnPlayCard += PlayerPlayingField_OnPlayCard;
        
    }

    private void PlayerPlayingField_OnPlayCard(object sender, EventArgs e)
    {
        cardsOnHand--;
        SetCardsOnHandServerRpc(Player.Instance.IAm(), cardsOnHand);
    }

    private void CardGameManager_OnBeginTurn(object sender, EventArgs e)
    {
        if(cardsOnHand < 5)DrawCard();
    }

    private void PlayerDeck_OnFetchActiveDeck(object sender, EventArgs e)
    {
        DrawCard();
        DrawCard();
        DrawCard();
        DrawCard();

        //will begin match by deciding who goes first when both players ready
        if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            SetPlayerOneReadyServerRpc();
        }
        else if(Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            SetPlayerTwoReadyServerRpc();
        }
        //ugly
        //Resizable.Instance.MaxSize();
    }
    [ServerRpc(RequireOwnership =false)]
    private void SetPlayerOneReadyServerRpc()
    {
        DivineMultiplayer.Instance.isPlayerOneReady.Value = true;
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerTwoReadyServerRpc()
    {
        DivineMultiplayer.Instance.isPlayerTwoReady.Value = true;
    }

    private void DrawCard()
    {
        if (PlayerDeck.Instance.totalCards == 0)
        {
            CardGameManager.Instance.MatchEndServerRpc(Player.Instance.OpponentIs());
            return;
        }
        if (loadingText.IsActive())
        {
            loadingText.Hide();
        }
        cardsOnHand++;
        //will update other player that this player drew a card
        SetCardsOnHandServerRpc(Player.Instance.IAm(), cardsOnHand);
        
        PlayerDeck.Instance.totalCards--;
        
        CardSO cardSO = PlayerDeck.Instance.playerDeck.LastOrDefault();
        PlayerDeck.Instance.playerDeck.RemoveAt(PlayerDeck.Instance.playerDeck.Count - 1);

        //Will instantiate appropriate prefab given cardtype
        GameObject cardCreated = CardGenerator.Instance.GenerateLocalCardFromCardSO();
        cardCreated.transform.SetParent(transform);

        BaseCardLocal baseCardLocal = cardCreated.GetComponent<BaseCardLocal>();
        baseCardLocal.SetCardSO(cardSO);
       

        baseCardLocal.SetCardGameArea(GameAreaEnum.Hand);

        OnDrawCard?.Invoke(this, EventArgs.Empty);

        //Will update other player's opponent deck card count
        if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            PlayerDeck.Instance.SetPlayerOneCardCountServerRpc(PlayerDeck.Instance.totalCards);
        }
        else if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            PlayerDeck.Instance.SetPlayerTwoCardCountServerRpc(PlayerDeck.Instance.totalCards);
        }
        //ugly
        //Resizable.Instance.UpdateSpacing();
    }

    [ServerRpc(RequireOwnership =false)]
    private void SetCardsOnHandServerRpc(PlayerEnum playerWhoCrewCard, int numberCards)
    {
        
        if (playerWhoCrewCard == PlayerEnum.PlayerOne)
        {
            DivineMultiplayer.Instance.playerOneHandCards.Value = numberCards;
        }else if (playerWhoCrewCard == PlayerEnum.PlayerTwo)
        {
            DivineMultiplayer.Instance.playerTwoHandCards.Value = numberCards;
        }
    }
    public List<BaseCardLocal> GetHandCards()
    {
        return GetComponentsInChildren<BaseCardLocal>().ToList();
    }


    

   
}
