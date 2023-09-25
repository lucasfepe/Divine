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
    public static DivineMultiplayer Instance { get; private set; }
    public NetworkVariable<bool> isPlayerOneReady = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> isPlayerTwoReady = new NetworkVariable<bool>(false);
    public NetworkVariable<int> playerOneDeckCards = new NetworkVariable<int>(-1);
    public NetworkVariable<int> playerTwoDeckCards = new NetworkVariable<int>(-1);
    public NetworkVariable<int> playerOneHandCards = new NetworkVariable<int>(-1);
    public NetworkVariable<int> playerTwoHandCards = new NetworkVariable<int>(-1);
    public NetworkVariable<int> playerOneProgress = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerTwoProgress = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerOneLight = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerTwoLight = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerOneStardust = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerTwoStardust = new NetworkVariable<int>(0);

    public NetworkVariable<int> playerOneBlackHole = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerTwoBlackHole = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerOneNeutronStar = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerTwoNeutronStar = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerOneWhiteDwarf = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerTwoWhiteDwarf = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerOneBlackDwarf = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerTwoBlackDwarf = new NetworkVariable<int>(0);

    public NetworkList<FixedString64Bytes> fieldExpertCardsPlayerOne;
    public NetworkList<FixedString64Bytes> fieldExpertCardsPlayerTwo;
    public NetworkList<FixedString64Bytes> fieldCivilizationCardsPlayerOne;
    public NetworkList<FixedString64Bytes> fieldCivilizationCardsPlayerTwo;
    public NetworkVariable<FixedString64Bytes> fieldSubterfugeCardPlayerOne =
        new NetworkVariable<FixedString64Bytes>();
    public NetworkVariable<FixedString64Bytes> fieldSubterfugeCardPlayerTwo =
        new NetworkVariable<FixedString64Bytes>();



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
    public event EventHandler<OnPlayerProgressChangedEventArgs> OnPlayerProgressChanged;
    public class OnPlayerProgressChangedEventArgs : EventArgs
    {
        public PlayerEnum playerEnum;
    }
    private void Awake()
    {
        Instance = this;
        fieldExpertCardsPlayerOne = new NetworkList<FixedString64Bytes>();
        fieldExpertCardsPlayerTwo = new NetworkList<FixedString64Bytes>();
        fieldCivilizationCardsPlayerOne = new NetworkList<FixedString64Bytes>();
        fieldCivilizationCardsPlayerTwo = new NetworkList<FixedString64Bytes>();
    }
   
    public override void OnNetworkSpawn()
    {
        playerOneLight.OnValueChanged += PlayerOneLight_OnValueChanged;
        playerTwoLight.OnValueChanged += PlayerTwoLight_OnValueChanged;
        playerOneStardust.OnValueChanged += PlayerOneStardust_OnValueChanged;
        playerTwoStardust.OnValueChanged += PlayerTwoStardust_OnValueChanged;
        playerOneDeckCards.OnValueChanged += PlayerOneDeckCards_OnValueChanged;
        playerTwoDeckCards.OnValueChanged += PlayerTwoDeckCards_OnValueChanged;
        playerOneHandCards.OnValueChanged += PlayerOneHandCards_OnValueChanged;
        playerTwoHandCards.OnValueChanged += PlayerTwoHandCards_OnValueChanged;
        fieldExpertCardsPlayerOne.OnListChanged += FieldExpertCardsPlayerOne_OnListChanged;
        fieldExpertCardsPlayerTwo.OnListChanged += FieldExpertCardsPlayerTwo_OnListChanged;
        fieldCivilizationCardsPlayerOne.OnListChanged += FieldCivilizationCardsPlayerOne_OnListChanged;
        fieldCivilizationCardsPlayerTwo.OnListChanged += FieldCivilizationCardsPlayerTwo_OnListChanged;
        fieldSubterfugeCardPlayerOne.OnValueChanged += FieldSubtergufeCardPlayerOne_OnValueChanged;
        fieldSubterfugeCardPlayerTwo.OnValueChanged += FieldSubtergufeCardPlayerTwo_OnValueChanged;
        playerOneProgress.OnValueChanged += PlayerOneProgress_OnValueChanged;
        playerTwoProgress.OnValueChanged += PlayerTwoProgress_OnValueChanged;
        isPlayerOneReady.OnValueChanged += PlayersReady_OnValueChanged;
        isPlayerTwoReady.OnValueChanged += PlayersReady_OnValueChanged;

        playerOneBlackHole.OnValueChanged += DeadStarP1_OnValueChanged;
        playerTwoBlackHole.OnValueChanged += DeadStarP2_OnValueChanged;
        playerOneNeutronStar.OnValueChanged += DeadStarP1_OnValueChanged;
        playerTwoNeutronStar.OnValueChanged += DeadStarP2_OnValueChanged;
        playerOneWhiteDwarf.OnValueChanged += DeadStarP1_OnValueChanged;
        playerTwoWhiteDwarf.OnValueChanged += DeadStarP2_OnValueChanged;
        playerOneBlackDwarf.OnValueChanged += DeadStarP1_OnValueChanged;
        playerTwoBlackDwarf.OnValueChanged += DeadStarP2_OnValueChanged;

        //if this is the second player to spawn will want 
    }

    //may want to split this up into 8 so that don't update every value on every value change
    //but probably fine
    private void DeadStarP1_OnValueChanged(int previousValue, int newValue)
    {
        if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            DeadStarBankUI.Instance.UpdateDeadStarBankUI();
        }
        else if (Player.Instance.OpponentIs() == PlayerEnum.PlayerOne)
        {
            DeadStarServerRpc(PlayerEnum.PlayerOne);
        }
    }
    private void DeadStarP2_OnValueChanged(int previousValue, int newValue)
    {
        if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            DeadStarBankUI.Instance.UpdateDeadStarBankUI();
        }
        else if (Player.Instance.OpponentIs() == PlayerEnum.PlayerTwo)
        {
            DeadStarServerRpc(PlayerEnum.PlayerTwo);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeadStarServerRpc(PlayerEnum player)
    {
        DeadStarClientRpc(player);
    }
    [ClientRpc]
    private void DeadStarClientRpc(PlayerEnum player)
    {
        if (Player.Instance.OpponentIs() == player)
        {
            OpponentDeadStarBankUI.Instance.UpdateDeadStarBankUI();
        }
       
    }

    public void IncreaseBlackHoleCount()
    {
        if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            SetBlackHoleCountServerRpc(PlayerEnum.PlayerOne, playerOneBlackHole.Value + 1);
        }
        else if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            SetBlackHoleCountServerRpc(PlayerEnum.PlayerTwo, playerTwoBlackHole.Value + 1);
        }
    }

    [ServerRpc(RequireOwnership =false)]
    private void SetBlackHoleCountServerRpc(PlayerEnum player, int value)
    {
        if (player == PlayerEnum.PlayerOne)
        {
            playerOneBlackHole.Value = value;
        }
        else if (player == PlayerEnum.PlayerTwo)
        {
            playerTwoBlackHole.Value = value;
        }
    }
    public void IncreaseNeutronStarCount()
    {
        if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            SetNeutronStarCountServerRpc(PlayerEnum.PlayerOne, playerOneNeutronStar.Value + 1);
        }
        else if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            SetNeutronStarCountServerRpc(PlayerEnum.PlayerTwo, playerTwoNeutronStar.Value + 1);
        }
    }
    public void DecreaseNeutronStarCount()
    {
        if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            SetNeutronStarCountServerRpc(PlayerEnum.PlayerOne, playerOneNeutronStar.Value - 1);
        }
        else if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            SetNeutronStarCountServerRpc(PlayerEnum.PlayerTwo, playerTwoNeutronStar.Value - 1);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetNeutronStarCountServerRpc(PlayerEnum player, int value)
    {
        if (player == PlayerEnum.PlayerOne)
        {
            playerOneNeutronStar.Value = value;
        }
        else if (player == PlayerEnum.PlayerTwo)
        {
            playerTwoNeutronStar.Value = value;
        }
    }
    public void IncreaseWhiteDwarfCount()
    {
        if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            SetWhiteDwarfCountServerRpc(PlayerEnum.PlayerOne, playerOneWhiteDwarf.Value + 1);
        }
        else if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            SetWhiteDwarfCountServerRpc(PlayerEnum.PlayerTwo, playerTwoWhiteDwarf.Value + 1);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetWhiteDwarfCountServerRpc(PlayerEnum player, int value)
    {
        if (player == PlayerEnum.PlayerOne)
        {
            playerOneWhiteDwarf.Value = value;
        }
        else if (player == PlayerEnum.PlayerTwo)
        {
            playerTwoWhiteDwarf.Value = value;
        }
    }
    public void IncreaseBlackDwarfCount()
    {
        if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            SetBlackDwarfCountServerRpc(PlayerEnum.PlayerOne, playerOneBlackDwarf.Value + 1);
        }
        else if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            SetBlackDwarfCountServerRpc(PlayerEnum.PlayerTwo, playerTwoBlackDwarf.Value + 1);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetBlackDwarfCountServerRpc(PlayerEnum player, int value)
    {
        if (player == PlayerEnum.PlayerOne)
        {
            playerOneBlackDwarf.Value = value;
        }
        else if (player == PlayerEnum.PlayerTwo)
        {
            playerTwoBlackDwarf.Value = value;
        }
    }

    public int GetBlackHoleCount()
    {
        if(Player.Instance.IAm()== PlayerEnum.PlayerOne)
        {
            return playerOneBlackHole.Value;
        }else if(Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            return playerTwoBlackHole.Value;
        }
        return 0;

    }
    public int GetNeutronStarCount()
    {
        if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            return playerOneNeutronStar.Value;
        }
        else if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            return playerTwoNeutronStar.Value;
        }
        return 0;

    }
    public int GetWhiteDwarfCount()
    {
        if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            return playerOneWhiteDwarf.Value;
        }
        else if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            return playerTwoWhiteDwarf.Value;
        }
        return 0;

    }
    public int GetBlackDwarfCount()
    {
        if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            return playerOneBlackDwarf.Value;
        }
        else if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            return playerTwoBlackDwarf.Value;
        }
        return 0;

    }



    private void PlayerOneLight_OnValueChanged(int previousValue, int newValue)
    {
        //update player two of player one's light
        
        if (Player.Instance.IAm() ==  PlayerEnum.PlayerOne)
        {
            CardGameManager.Instance.UpdateLight();
        }else if(Player.Instance.OpponentIs() == PlayerEnum.PlayerOne)
        {
            LightServerRpc(PlayerEnum.PlayerOne);
        }

    }
    private void PlayerTwoLight_OnValueChanged(int previousValue, int newValue)
    {
        //update player one of player two's light
        
        if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            CardGameManager.Instance.UpdateLight();
        }else if(Player.Instance.OpponentIs() == PlayerEnum.PlayerTwo)
        {
            LightServerRpc(PlayerEnum.PlayerTwo);
        }
    }
    [ServerRpc(RequireOwnership =false)]
    private void LightServerRpc(PlayerEnum playerLightChanged)
    {
        LightClientRpc(playerLightChanged);
    }
    //ugly? isn't there a better way to do this like without server rpcss
    [ClientRpc]
    private void LightClientRpc(PlayerEnum playerLightChanged)
    {
        if (Player.Instance.IAm() == playerLightChanged) return;
        if(Player.Instance.OpponentIs() == PlayerEnum.PlayerOne)
        {
            CardGameManager.Instance.UpdateOpponentLight();
            //OpponentBankUI.Instance.SetLightText(playerOneLight.Value);
        }else if(Player.Instance.OpponentIs() == PlayerEnum.PlayerTwo)
        {
            CardGameManager.Instance.UpdateOpponentLight();
            //OpponentBankUI.Instance.SetLightText(playerTwoLight.Value);
        }

    }

    private void PlayerOneStardust_OnValueChanged(int previousValue, int newValue)
    {
        //update player two of player one's light
        
        if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            CardGameManager.Instance.UpdateStardust();
        }else if(Player.Instance.OpponentIs() == PlayerEnum.PlayerOne)
        {
            StardustServerRpc(PlayerEnum.PlayerOne);
        }

    }
    private void PlayerTwoStardust_OnValueChanged(int previousValue, int newValue)
    {
        //update player one of player two's light
        
        if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            CardGameManager.Instance.UpdateStardust();
        }else if(Player.Instance.OpponentIs() == PlayerEnum.PlayerTwo)
        {
            StardustServerRpc(PlayerEnum.PlayerTwo);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void StardustServerRpc(PlayerEnum playerStardustChanged)
    {
        StardustClientRpc(playerStardustChanged);
    }
    //ugly? isn't there a better way to do this like without server rpcss
    [ClientRpc]
    private void StardustClientRpc(PlayerEnum playerStardustChanged)
    {
        if (Player.Instance.IAm() == playerStardustChanged) return;
        if (Player.Instance.OpponentIs() == PlayerEnum.PlayerOne)
        {
            CardGameManager.Instance.UpdateOpponentStardust();
            //OpponentBankUI.Instance.SetStardustText(playerOneStardust.Value);
        }
        else if (Player.Instance.OpponentIs() == PlayerEnum.PlayerTwo)
        {
            CardGameManager.Instance.UpdateOpponentStardust();
            //OpponentBankUI.Instance.SetStardustText(playerTwoStardust.Value);
        }

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
    private void PlayerOneProgress_OnValueChanged(int previousValue, int newValue)
    {
        OnPlayerProgressChanged?.Invoke(this, new OnPlayerProgressChangedEventArgs
        {
            playerEnum = PlayerEnum.PlayerOne,
        });
    }
    private void PlayerTwoProgress_OnValueChanged(int previousValue, int newValue)
    {
        OnPlayerProgressChanged?.Invoke(this, new OnPlayerProgressChangedEventArgs
        {
            playerEnum = PlayerEnum.PlayerTwo,
        });
    }

    private void FieldSubtergufeCardPlayerTwo_OnValueChanged(FixedString64Bytes previousValue, FixedString64Bytes newValue)
    {
        OnFieldCardsChanged?.Invoke(this, new OnFieldCardsChangedEventArgs
        {
            playerEnum = PlayerEnum.PlayerTwo,
            cardType = CardTypeEnum.Subterfuge,
            cardName = newValue.ToString()

        });
    }

    private void FieldSubtergufeCardPlayerOne_OnValueChanged(FixedString64Bytes previousValue, FixedString64Bytes newValue)
    {
        OnFieldCardsChanged?.Invoke(this, new OnFieldCardsChangedEventArgs
        {
            playerEnum = PlayerEnum.PlayerOne,
            cardType = CardTypeEnum.Subterfuge,
            cardName = newValue.ToString()
        });
    }

    private void FieldExpertCardsPlayerTwo_OnListChanged(NetworkListEvent<FixedString64Bytes> changeEvent)
    {
        OnFieldCardsChanged?.Invoke(this, new OnFieldCardsChangedEventArgs
        {
            playerEnum = PlayerEnum.PlayerTwo,
            cardType = CardTypeEnum.Expert,
            cardName = changeEvent.Value.ToString()
        }) ;
    }

    private void FieldExpertCardsPlayerOne_OnListChanged(NetworkListEvent<FixedString64Bytes> changeEvent)
    {
        OnFieldCardsChanged?.Invoke(this, new OnFieldCardsChangedEventArgs
        {
            playerEnum = PlayerEnum.PlayerOne,
            cardType = CardTypeEnum.Expert,
            cardName = changeEvent.Value.ToString()
        });
    }
    private void FieldCivilizationCardsPlayerTwo_OnListChanged(NetworkListEvent<FixedString64Bytes> changeEvent)
    {
        OnFieldCardsChanged?.Invoke(this, new OnFieldCardsChangedEventArgs
        {
            playerEnum = PlayerEnum.PlayerTwo,
            cardType = CardTypeEnum.Civilization,
            cardName = changeEvent.Value.ToString()
        });
    }
    private void FieldCivilizationCardsPlayerOne_OnListChanged(NetworkListEvent<FixedString64Bytes> changeEvent)
    {
        OnFieldCardsChanged?.Invoke(this, new OnFieldCardsChangedEventArgs
        {
            playerEnum = PlayerEnum.PlayerOne,
            cardType = CardTypeEnum.Civilization,
            cardName = changeEvent.Value.ToString()
        });
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
    internal void HealthChangedServerRpc(int cardIndex, int damage, PlayerEnum owner)
    {
        HealthChangedClientRpc(cardIndex, damage, owner);
    }
    [ClientRpc]
    private void HealthChangedClientRpc(int cardIndex, int damage, PlayerEnum owner)
    {
        if(Player.Instance.IAm() == owner)
        {
            if(Player.Instance.IAm() == PlayerEnum.PlayerOne)
            {
                BaseCard targetCard = PlayerPlayingField.Instance.GetAllPlayerExpertCards()[cardIndex];
                ((ExpertCard)targetCard).TakeDamage(damage);
                
            }
            if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
            {
                BaseCard targetCard = PlayerPlayingField.Instance.GetAllPlayerExpertCards()[cardIndex];
                ((ExpertCard)targetCard).TakeDamage(damage);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    internal void LevelUpServerRpc(int cardIndex, PlayerEnum owner)
    {
        LevelUpClientRpc(cardIndex, owner);
    }
    [ClientRpc]
    private void LevelUpClientRpc(int cardIndex, PlayerEnum owner)
    {
        if (owner == Player.Instance.OpponentIs())
        {
                BaseCard targetCard = OpponentPlayingField.Instance.GetAllOpponentExpertCards()[cardIndex];
                //((ExpertCard)targetCard).LevelUp();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    internal void PacifyServerRpc(int cardIndex, PlayerEnum owner)
    {
        PacifyClientRpc(cardIndex, owner);
    }
    [ClientRpc]
    private void PacifyClientRpc(int cardIndex, PlayerEnum owner)
    {
        if (Player.Instance.IAm() == owner)
        {
            //if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
            //{
            //    BaseCard targetCard = PlayerPlayingField.Instance.GetAllPlayerExpertCards()[cardIndex];
            //    targetCard.Pacify();
               
            //    Status status = new Status();
            //    status.statusPower = 0;
            //    status.statusType = EffectTypeEnum.Pacify;
            //    targetCard.AddStatus(status);

            //}
            //if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
            //{
            //    BaseCard targetCard = PlayerPlayingField.Instance.GetAllPlayerExpertCards()[cardIndex];
            //    targetCard.Pacify();
                
            //    Status status = new Status();
            //    status.statusPower = 0;
            //    status.statusType = EffectTypeEnum.Pacify;
            //    targetCard.AddStatus(status);
            //}
        }
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

    public int GetStardust()
    {
        if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
           return  playerOneStardust.Value;
        }
        else if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            return playerTwoStardust.Value;
        }
        return 0;
    }
    public int GetOpponentStardust()
    {
        if (Player.Instance.OpponentIs() == PlayerEnum.PlayerOne)
        {
            return playerOneStardust.Value;
        }
        else if (Player.Instance.OpponentIs() == PlayerEnum.PlayerTwo)
        {
            return playerTwoStardust.Value;
        }
        return 0;
    }



    public int GetOpponentLight()
    {
        if (Player.Instance.OpponentIs() == PlayerEnum.PlayerOne)
        {
            return playerOneLight.Value;
        }
        else if (Player.Instance.OpponentIs() == PlayerEnum.PlayerTwo)
        {
            return playerTwoLight.Value;
        }
        return 0;
    }

    public void IncreaseLight(int value)
    {
        if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            SetLightServerRpc(PlayerEnum.PlayerOne, playerOneLight.Value + value);
        }
        else if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            SetLightServerRpc(PlayerEnum.PlayerTwo, playerTwoLight.Value + value);
        }
    }
    public void DecreaseLight(int value)
    {
        if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            SetLightServerRpc(PlayerEnum.PlayerOne, playerOneLight.Value - value);
        }
        else if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            SetLightServerRpc(PlayerEnum.PlayerTwo, playerTwoLight.Value - value);
        }
    }

    public void DecreaseStardust(int value)
    {
        if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            SetStardustServerRpc(PlayerEnum.PlayerOne, playerOneStardust.Value - value);
        }
        else if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            SetStardustServerRpc(PlayerEnum.PlayerTwo, playerTwoStardust.Value - value);
        }
    }
    public void IncreaseStardust(int value)
    {
        if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            SetStardustServerRpc(PlayerEnum.PlayerOne, playerOneStardust.Value + value);
        }
        else if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            SetStardustServerRpc(PlayerEnum.PlayerTwo, playerTwoStardust.Value + value);
        }
    }
    [ServerRpc(RequireOwnership =false)]
    public void SetStardustServerRpc(PlayerEnum player, int value)
    {
        if(player == PlayerEnum.PlayerOne)
        {
            playerOneStardust.Value = value;
        }else if(player == PlayerEnum.PlayerTwo)
        {
            playerTwoStardust.Value = value;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetLightServerRpc(PlayerEnum player, int value)
    {
        if (player == PlayerEnum.PlayerOne)
        {
            playerOneLight.Value = value;
        }
        else if (player == PlayerEnum.PlayerTwo)
        {
            playerTwoLight.Value = value;
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