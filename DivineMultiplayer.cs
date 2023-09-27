using Amazon.S3.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
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


    public event EventHandler<OnFieldCardsChangedEventArgs> OnFieldCardsChanged;
    public class OnFieldCardsChangedEventArgs : EventArgs
    {
        public PlayerEnum playerEnum;
        public CardTypeEnum cardType;
        public string cardName;
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
        if (!IsHost)
            return;
            
        
        if(isPlayerOneReady.Value && isPlayerTwoReady.Value)
        {
            CardGameManager.Instance.ChoosePlayerToStartServerRpc();
            //update player two on player one's deck card number
            UpdatePlayerTwoOnPlayerOneDeckCardNumberServerRpc();
        }
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
            playerOneDeckCards.Value = PlayerDeck.Instance.totalCards;
        }
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
    internal void AbsorbServerRpc(int cardIndex, PlayerEnum owner)
    {
        AbsorbClientRpc(cardIndex, owner);
    }
    [ClientRpc]
    private void AbsorbClientRpc(int cardIndex, PlayerEnum owner)
    {
        if (Player.Instance.IAm() == owner)
        {
            
                BaseCard targetCard = PlayerPlayingField.Instance.GetAllPlayerExpertCards()[cardIndex];
                Destroy(targetCard.gameObject);

            
            
        }
    }

    

    [ServerRpc(RequireOwnership = false)]
    public void UpdateCardStardustInOpponentFieldServerRpc(int index, int value, PlayerEnum opponent)
    {
        UpdateCardStardustInOpponentFieldClientRpc(index, value, opponent);
    }
    [ClientRpc]
    private void UpdateCardStardustInOpponentFieldClientRpc(int index, int value, PlayerEnum opponent)
    {
        if (Player.Instance.IAm() == opponent) {
            OpponentPlayingField.Instance.GetAllOpponentExpertCards()[index].SetCardStardust(value);
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateCardLightInOpponentFieldServerRpc(int index, int value, PlayerEnum opponent)
    {
        UpdateCardLightInOpponentFieldClientRpc(index, value, opponent);
    }
    [ClientRpc]
    private void UpdateCardLightInOpponentFieldClientRpc(int index, int value, PlayerEnum opponent)
    {
        //error if tries to update the light (black hole) after the card has vanished
        if (Player.Instance.IAm() == opponent)
        {
            OpponentPlayingField.Instance.GetAllOpponentExpertCards()[index].SetLight(value);
        }

    }
}