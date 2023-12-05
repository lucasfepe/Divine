using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class DivineMultiplayer : NetworkBehaviour
{
    //ugly perhaps it would be better to split this class into the classes that it affects...
    //getting there
    public static DivineMultiplayer Instance { get; private set; }
    public NetworkVariable<bool> isPlayerOneReady = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> isPlayerTwoReady = new NetworkVariable<bool>(false);
    public NetworkVariable<int> playerOneDeckCards = new NetworkVariable<int>(-1);
    public NetworkVariable<int> playerTwoDeckCards = new NetworkVariable<int>(-1);
    public NetworkVariable<int> playerOneHandCards = new NetworkVariable<int>(-1);
    public NetworkVariable<int> playerTwoHandCards = new NetworkVariable<int>(-1);

    public event EventHandler OnSpawnCard;

    public NetworkList<FixedString64Bytes> fieldExpertCardsPlayerOne;
    public NetworkList<FixedString64Bytes> fieldExpertCardsPlayerTwo;




    public event EventHandler<OnDeckCardsNumberChangedEventArgs> OnDeckCardsNumberChanged;
    public class OnDeckCardsNumberChangedEventArgs : EventArgs
    {
        public PlayerEnum playerEnum;
    }

    public event EventHandler<OnHandCardsNumberChangedEventArgs> OnHandCardsNumberChanged;
    public class OnHandCardsNumberChangedEventArgs : EventArgs
    {
        public PlayerEnum playerEnum;
    }


   
    
    private void Awake()
    {
        Instance = this;
        fieldExpertCardsPlayerOne = new NetworkList<FixedString64Bytes>();
        fieldExpertCardsPlayerTwo = new NetworkList<FixedString64Bytes>();
      
    }
   
    public override void OnNetworkSpawn()
    {
       
        playerOneDeckCards.OnValueChanged += PlayerOneDeckCards_OnValueChanged;
        playerTwoDeckCards.OnValueChanged += PlayerTwoDeckCards_OnValueChanged;
        playerOneHandCards.OnValueChanged += PlayerOneHandCards_OnValueChanged;
        playerTwoHandCards.OnValueChanged += PlayerTwoHandCards_OnValueChanged;
        //fieldExpertCardsPlayerOne.OnListChanged += FieldExpertCardsPlayerOne_OnListChanged;
        //fieldExpertCardsPlayerTwo.OnListChanged += FieldExpertCardsPlayerTwo_OnListChanged;
        
        isPlayerOneReady.OnValueChanged += PlayersReady_OnValueChanged;
        isPlayerTwoReady.OnValueChanged += PlayersReady_OnValueChanged;

        
    }

   


    private void PlayersReady_OnValueChanged(bool previousValue, bool newValue)
    {
        if (!IsServer)
            return;


    }
    [ClientRpc]
    private void PlayersReadyClientRpc()
    {
        if (CardGameManager.Instance.GetPlayer() is PlayerTwo)
            return;
        CardGameManager.Instance.ChoosePlayerToStartServerRpc();
        //update player two on player one's deck card number
        UpdatePlayerTwoOnPlayerOneDeckCardNumberServerRpc();
    }

    [ServerRpc(RequireOwnership =false)]
    private void UpdatePlayerTwoOnPlayerOneDeckCardNumberServerRpc()
    {
        UpdatePlayerTwoOnPlayerOneDeckCardNumberClientRpc();
    }
    [ClientRpc]
    private void UpdatePlayerTwoOnPlayerOneDeckCardNumberClientRpc()
    {
        if(Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            SetPlayerOneDeckCardsServerRpc(PlayerDeck.Instance.totalCards);
        }
    }
    [ServerRpc(RequireOwnership =false)]
    private void SetPlayerOneDeckCardsServerRpc(int num)
    {
        playerOneDeckCards.Value = num;
    }
   

    

    private void PlayerOneDeckCards_OnValueChanged(int previousValue, int newValue)
    {
        OnDeckCardsNumberChanged?.Invoke(this, new OnDeckCardsNumberChangedEventArgs
        {
            playerEnum = PlayerEnum.PlayerOne
        });
    }
    private void PlayerTwoDeckCards_OnValueChanged(int previousValue, int newValue)
    {
        OnDeckCardsNumberChanged?.Invoke(this, new OnDeckCardsNumberChangedEventArgs
        {
            playerEnum = PlayerEnum.PlayerTwo
        });
    }
    private void PlayerOneHandCards_OnValueChanged(int previousValue, int newValue)
    {
        OnHandCardsNumberChanged?.Invoke(this, new OnHandCardsNumberChangedEventArgs
        {
            playerEnum = PlayerEnum.PlayerOne
        });
    }
    private void PlayerTwoHandCards_OnValueChanged(int previousValue, int newValue)
    {
        OnHandCardsNumberChanged?.Invoke(this, new OnHandCardsNumberChangedEventArgs
        {
            playerEnum = PlayerEnum.PlayerTwo
        });
    }

   

    

    

   
    

    [ServerRpc(RequireOwnership = false)]
    public void PlayCardServerRpc(string title, PlayerEnum owner)
    {
        //BaseCard baseCard = 
        CreateCardInNetwork(title, owner);
        



    }
    [ClientRpc]
    private void OnSpawnCardClientRpc()
    {
        OnSpawnCard?.Invoke(this, EventArgs.Empty);
    }
    public void CallOnSpawnCard()
    {
        OnSpawnCard?.Invoke(this, EventArgs.Empty);
    }
    public async void CreateCardInNetwork(string title, PlayerEnum owner)
    {

        CardSO cardSO = await CardGenerator.Instance.CardNameToCardSO(title);
        //generate card from cardSO

        

        BaseCard baseCard = CardGenerator.Instance.GenerateCardFromCardSO(cardSO);
        

        baseCard.GetComponent<NetworkObject>().Spawn();
        baseCard.SetCardOwner(owner);
        baseCard.SetCardStardust(cardSO.Stardust);
        baseCard.SetCardLight(cardSO.Light);
        baseCard.SetLifetime(cardSO.Lifetime);
        baseCard.SetCardTitle(title);
        
        OnSpawnCard?.Invoke(this, EventArgs.Empty);
        
        

        //baseCard.InitializeOpponentCard(title);
        //if (cardSO.cardType == CardTypeEnum.Civilization)
        //{
        //    RectTransform rectTransform = 
        //        cardCreated.gameObject.GetComponent<RectTransform>();
        //    rectTransform.localScale = new Vector2(UniversalConstants.FIELD_CARD_SCALE, UniversalConstants.FIELD_CARD_SCALE);
        //}
        //else if (cardSO.cardType == CardTypeEnum.Expert)
        //{

        //}
        //else if (cardSO.cardType == CardTypeEnum.Subterfuge)
        //{
        //    subterfugeCardPlayed = true;
        //    subterfugeCard = cardCreated;
        //}


    }
}