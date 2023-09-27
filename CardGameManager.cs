using Amazon.S3.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardGameManager : NetworkBehaviour
{
    public static CardGameManager Instance { get; private set; }

    [SerializeField] private GameObject playerField;
    [SerializeField] private PlayerOne playerOne;
    [SerializeField] private PlayerTwo playerTwo;

    public const int PROGRESS_BAR_MAX = 50;
    
    
    

    private BaseCard initiator;
    //this is weird
    private int turn;
    private int expertCardsPlayedThisTurn;
    private bool isMyTurn = false;
    private bool isGameOver = false;
    private bool eurekaThisTurn = false;
    private bool isSelectingTarget = false;
    public int stardustFromSupernova = 0;
    private IPlayer player;
    private IPlayer opponent;


    
    
    public event EventHandler OnBeginTurn;
    public event EventHandler OnBeginTurnStep2;
    public event EventHandler OnEndTurn;
    public event EventHandler<OnWinEventArgs> OnWin;
    public event EventHandler OnBeginMatch;
    

    public class OnWinEventArgs : EventArgs
    {
        public PlayerEnum playerEnum;
    }
    public class OnSelectTargetEventArgs : EventArgs
    {
        public BaseCard card;
    }
    public class OnAttackEventArgs : EventArgs
    {
        public BaseCard card;
        public int damage;
    }

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            player = playerOne;
            opponent = playerTwo;
                //gameObject.AddComponent<PlayerOne>();
        }
        else
        {
            player = playerTwo;
            opponent = playerOne;
            //When the second player joins, begin the match
            BeginMatchServerRpc();
            
        }
    }


    
    [ServerRpc(RequireOwnership =false)]
    private void MatchEndServerRpc(int winner)
    {
        MatchEndClientRpc(winner);
    }
    [ClientRpc]
    private void MatchEndClientRpc(int winner)
    {
        if(winner == 1)
        {
            OnWin?.Invoke(this, new OnWinEventArgs
            {
                playerEnum = PlayerEnum.PlayerOne
            });
        }
        else
        {
            OnWin?.Invoke(this, new OnWinEventArgs
            {
                playerEnum = PlayerEnum.PlayerTwo
            });
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void BeginTurnServerRpc(PlayerEnum playerEnum)
    {
        BeginTurnClientRpc(playerEnum);
    }

    [ClientRpc]
    public void BeginTurnClientRpc(PlayerEnum playerEnum)
    {
        if(playerEnum == Player.Instance.IAm()) {
            return;
        }
        OnBeginTurn?.Invoke(this, EventArgs.Empty);
        OnBeginTurnStep2?.Invoke(this, EventArgs.Empty);
        int stardustPerTurn = 5;
        player.SetStardust(player.GetStardust() + stardustPerTurn + stardustFromSupernova);
        
        BankUI.Instance.GlowStardust(stardustPerTurn + stardustFromSupernova);
        stardustFromSupernova = 0;
        
        //OnUpdateBankedStardust?.Invoke(this, EventArgs.Empty);

        turn++;
        isMyTurn = true;

        

        List<BaseCard> playerCardsOnPlay = playerField.GetComponentsInChildren<BaseCard>().ToList();
        //List<BaseCard> expertCards = playerField.GetComponentsInChildren<BaseCard>()
        //    .Where(x => x.GetCardSO().cardType == CardTypeEnum.Expert)
        //    .ToList();
        //List<BaseCard> civilizationCards = playerField.GetComponentsInChildren<BaseCard>()
        //    .Where(x => x.GetCardSO().cardType == CardTypeEnum.Civilization)
        //    .ToList();
        int lightSumGeneration = playerCardsOnPlay.Select(x => x.GenerateLight()).Aggregate(0, (accumulatedLightSum, light) =>
        {
            accumulatedLightSum += light;
            return accumulatedLightSum;
        });
        //set all at once instead of individually inside GenerateLight so the glow is proportinal to the whole gain
        player.SetLight(player.GetLight() + lightSumGeneration);
        //update progress
        //if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
        //{
        //    SetPlayerOneProgressServerRpc(civilizationCards.Aggregate(0, (acc, x) => acc + (x.GetCardSO().progress ?? default(int))));
        //}
        //else if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        //{
        //    SetPlayerTwoProgressServerRpc(civilizationCards.Aggregate(0, (acc, x) => acc + (x.GetCardSO().progress ?? default(int))));
        //}

        //reset values for new turn
        expertCardsPlayedThisTurn = 0;
        //Resizable.Instance.MaxSize();





    }
    public void EndTurn()
    {
        if (!isMyTurn)
        {
            return;
        }
        isMyTurn = false;

        //Begin the next player's turn
        BeginTurnServerRpc(Player.Instance.IAm());
        OnEndTurn?.Invoke(this, EventArgs.Empty);
    }


    
        
        
    

    [ServerRpc(RequireOwnership =false)]
    public void BeginMatchServerRpc()
    {
        BeginMatchClientRpc();
        

    }
    [ClientRpc]
    public void BeginMatchClientRpc()
    {
        OnBeginMatch?.Invoke(this, EventArgs.Empty);
        PlayerDeck.Instance.GetActiveDeck();
    }
    [ServerRpc(RequireOwnership =false)]
    public void ChoosePlayerToStartServerRpc()
    {
        
        int randomInt = UnityEngine.Random.Range(0, 2);
        if (randomInt == 0)
        {
            //PlayerOne begins
            BeginTurnServerRpc(PlayerEnum.PlayerOne);
        }
        else
        {
            //PlayerTwo begins
            BeginTurnServerRpc(PlayerEnum.PlayerTwo);
        };
    }

    public bool IsMyTurn()
    {
        return isMyTurn;
    }
    public bool CanPlayExpertCardThisTurn()
    {
        return expertCardsPlayedThisTurn == 0;
    }
    public void PlayedExpertCardThisTurn()
    {
        this.expertCardsPlayedThisTurn = 1;
    }
    public bool IsGameOver()
    {
        return isGameOver;
    }

    public void Eureka()
    {
        eurekaThisTurn = true;
    }

    public void PlayEurekaCard()
    {
        eurekaThisTurn = false;
    }

    public bool HasEureka()
    {
        return eurekaThisTurn;
    }
    
    public void BeginSelectingTarget(BaseCard initiator)
    {
        //InspectCardUI.Instance.Hide();
        isSelectingTarget = true;
        this.initiator = initiator;
    }

    public bool IsSelectingTarget()
    {
        return isSelectingTarget;
    }

    public BaseCard GetSelectTargetInitiator()
    {
        return initiator;
    }

    internal void DoneSelectingTarget()
    {
        initiator = null;
        isSelectingTarget = false;
    }

    

    public void PlayCard(BaseCard card)
    {
        player.SetStardust(player.GetStardust() - card.GetStardust());
        player.SetLight(player.GetLight() + card.GetLight());
       
        card.GetCardUI().GlowLight();
    }
    public void GainStardust(int stardustGain)
    {
        player.SetStardust(player.GetStardust() + stardustGain);
        BankUI.Instance.GlowStardust(stardustGain);
        //OnUpdateBankedStardust?.Invoke(this, EventArgs.Empty);
    }

    

    public IPlayer GetPlayer()
    {
        return player;
    }

    public IPlayer GetOpponent()
    {
        return opponent;
    }
}
