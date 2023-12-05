using Amazon.Lambda.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardGameManager : NetworkBehaviour
{
    public static CardGameManager Instance { get; private set; }

    [SerializeField] private GameObject playerField;
    [SerializeField] private PlayerOne playerOne;
    [SerializeField] private PlayerTwo playerTwo;

    public const int PROGRESS_BAR_MAX = 50;
    private bool willWipeOutBothFields = false;


    private int initiatorIndex;
    private BaseCard initiator;
    //this is weird
    private int turn;
    private int expertCardsPlayedThisTurn;
    private bool isMyTurn = false;
    private bool isGameOver = false;
    private bool eurekaThisTurn = false;
    private bool isSelectingTarget = false;
    private int tempStardustIncrement = 0;
    private IPlayer player;
    private IPlayer opponent;
    private int cardsShedThisTurn;
    private int cardsSupernovaThisTurn;
    

    public event EventHandler OnDoneSelectingTarget;
    public event EventHandler OnBeginSelectingTarget;
    public event EventHandler<OnBeginTurnEventArgs> OnBeginTurn;
    public class OnBeginTurnEventArgs: EventArgs
    {
        public bool firstTurn;
    }
    public event EventHandler OnBeginTurnStep2;
    public event EventHandler OnEndTurn;
    public event EventHandler<OnWinEventArgs> OnWin;
    public event EventHandler OnBeginMatch;
    private bool playerTwoReady = false;

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
    public bool isFirstTurn = true;
    private void Awake()
    {
        Instance = this;

    }
    private void Start()
    {
        AssignPlayer();



    }

    
    public int GetInitiatorIndex()
    {
        return initiatorIndex;
    }
    





    public override void OnNetworkSpawn()
    {
    }
    public void AssignPlayerOne()
    {
        player = playerOne;
        opponent = playerTwo;
        Player.Instance.SetPlayerEnum(PlayerEnum.PlayerOne);
    }
    public void AssignPlayerTwo()
    {
        player = playerTwo;
        opponent = playerOne;
        Player.Instance.SetPlayerEnum(PlayerEnum.PlayerTwo);


    }

    
    private void AssignPlayer()
    {
        if (MainMenuToGameStatic.playerNo == 1)
        {
            AssignPlayerOne();
        }
        else if (MainMenuToGameStatic.playerNo == 2)
        {
            AssignPlayerTwo();
        }
        Debug.Log("PAYERASSIGNED");
        BeginMatch();
    }



    [ServerRpc(RequireOwnership =false)]
    public void MatchEndServerRpc(PlayerEnum winner)
    {
        NetworkManager.Singleton.Shutdown();
        MatchEndClientRpc(winner);
    }
    [ClientRpc]
    private void MatchEndClientRpc(PlayerEnum winner)
    {
        Debug.Log("winner: " + winner);
        FindObjectsOfType<BoxCollider2D>().ToList().ForEach(x => x.enabled = false);
        if(winner == PlayerEnum.PlayerOne)
        {
            OnWin?.Invoke(this, new OnWinEventArgs
            {
                playerEnum = PlayerEnum.PlayerOne
            });
        }
        else if(winner == PlayerEnum.PlayerTwo)
        {
            OnWin?.Invoke(this, new OnWinEventArgs
            {
                playerEnum = PlayerEnum.PlayerTwo
            });
        }
        FindObjectOfType<KillNetMan>().KillNetManager();
    }

    [ServerRpc(RequireOwnership = false)]
    private void BeginTurnServerRpc(PlayerEnum playerEnum)
    {
        BeginTurnClientRpc(playerEnum);
    }

    [ClientRpc]
    public void BeginTurnClientRpc(PlayerEnum playerEnum, bool firstTurn = false)
    {
        isFirstTurn = firstTurn;
        if (playerEnum == Player.Instance.IAm()) {
            return;
        }
        isMyTurn = true;


        OnBeginTurn?.Invoke(this, new OnBeginTurnEventArgs
        {
            firstTurn = firstTurn
        });
        //OnBeginTurnStep2?.Invoke(this, EventArgs.Empty);


        
        
        //OnUpdateBankedStardust?.Invoke(this, EventArgs.Empty);

        turn++;
        

        

       
        //List<BaseCard> expertCards = playerField.GetComponentsInChildren<BaseCard>()
        //    .Where(x => x.GetCardSO().cardType == CardTypeEnum.Expert)
        //    .ToList();
        //List<BaseCard> civilizationCards = playerField.GetComponentsInChildren<BaseCard>()
        //    .Where(x => x.GetCardSO().cardType == CardTypeEnum.Civilization)
        //    .ToList();
        
        //set all at once instead of individually inside GenerateLight so the glow is proportinal to the whole gain
        
        
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
    public void AddToTurnLightIncrement(int value)
    {
        //turnLightIncrement += value;
    }
    public void AddToTempStardustIncrement(int value)
    {
        tempStardustIncrement += value;
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


    
        
        
    

    
   
    public void BeginMatch()
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
        //ugly do I need this?
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
    private bool shake = true;
    public void BeginSelectingTarget(BaseCard initiator, bool shake = true, int initiatorIndex = 0)
    {
        Debug.Log("baginselectintarget shake: " + shake);
        //InspectCardUI.Instance.Hide();
        this.shake = shake;
        isSelectingTarget = true;
        this.initiator = initiator;
        this.initiatorIndex = initiatorIndex;
        if (shake)
        {
            Debug.Log("Done flash, begin selecting target;");
            ((ExpertCardUI)initiator.GetCardUI()).DoneFlash();
            ((ExpertCardUI)initiator.GetCardUI()).GetCardGraphicAnimator().BecomeAttacking();
            initiator.BecomeAttackingServerRpc();
            initiator.GetViableTargets()
            .ForEach(x => {
                //for me
                //ugly can't I make this more unified...
                ((ExpertCardUI)x.GetCardUI()).Flash();
                ((ExpertCardUI)x.GetCardUI()).GetCardGraphicAnimator().BecomeTarget();

                //for my opponent
                x.BecomeTargetServerRpc();
            });
        }
        else
        {
            Debug.Log("baginselectintarget not shke");
            ((ExpertCardUI)initiator.GetCardUI()).LastingHighlight();

            Debug.Log("viable target count: " + initiator.GetViableTargets().Count);
            initiator.GetViableTargets()
            .ForEach(x => {
                //for me
                //ugly can't I make this more unified...
                Debug.Log("flash from not shaek");
                ((ExpertCardUI)x.GetCardUI()).Flash();


                //for my opponent
                //x.BecomeTargetServerRpc();
            });
        }
        //for my opponent
        
        //make all viable targets highlighted
        
        OnBeginSelectingTarget?.Invoke(this, EventArgs.Empty);


    }

    public bool IsSelectingTarget()
    {
        return isSelectingTarget;
    }

    public BaseCard GetSelectTargetInitiator()
    {
        return initiator;
    }
    public void CancelSkill()
    {

        DoneSelectingTarget();
    }
    internal void DoneSelectingTarget()
    {

        //ugly feels dirty um too many calls to other objects and methods... can't this be done in the class itself?
        ((ExpertCardUI)initiator.GetCardUI()).GetCardGraphicAnimator().StopAttacking();
        //for my opponent
        initiator.DoneAttackingServerRpc();

        initiator.GetViableTargets()
            .ForEach(x => {
                //for me
                if (!((ExpertCard)x).CanUseSkill())
                {
                    ((ExpertCardUI)x.GetCardUI()).DoneFlash();
                    ((ExpertCardUI)x.GetCardUI()).GetCardGraphicAnimator().StopBeingTarget();
                }
                
                

                //for my opponent
                x.StopBeingTargetServerRpc(Player.Instance.IAm());
            });
        if (!shake)
        {
            ((ExpertCardUI)initiator.GetCardUI()).EndLastingHighlight();
        }
        initiator = null;
        isSelectingTarget = false;
        OnDoneSelectingTarget?.Invoke(this, EventArgs.Empty);
    }

    

    

    public IPlayer GetPlayer()
    {
        return player;
    }

    public IPlayer GetOpponent()
    {
        return opponent;
    }

    public void CardShed()
    {
        cardsShedThisTurn++;
        if(cardsShedThisTurn == TurnPhaseStateMachine.Instance.GetCardsThatCanShedCount())
        {
            player.SetStardust(player.GetStardust() + tempStardustIncrement);
            cardsShedThisTurn = 0;
            tempStardustIncrement = 0;
        }
    }
    public void CardSupernova()
    {
        cardsSupernovaThisTurn++;
        if (cardsSupernovaThisTurn == TurnPhaseStateMachine.Instance.GetCardsThatWillSupernovaCount())
        {
            player.SetStardust(player.GetStardust() + tempStardustIncrement);
            cardsSupernovaThisTurn = 0;
            tempStardustIncrement = 0;
        }
    }
    public void WipeOutBothFields()
    {
        willWipeOutBothFields = true;
    }
    public bool WillWipeOutBothField()
    {
        return willWipeOutBothFields;
    }
    public void ResetWipeOutBothFields()
    {
        willWipeOutBothFields = false;
    }
}
