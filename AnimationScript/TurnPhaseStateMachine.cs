using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnPhaseStateMachine : MonoBehaviour
{
    [SerializeField] private PlayerPlayingField playerField;

    public static TurnPhaseStateMachine Instance { get; private set; }

    private State state;

    private int turnLightIncrement;
    private int cardsGlowed;
    private int playerCardsCount;
    private int agedCount;
    private int cardsThatCanAgeCount;
    private int shedCount;
    private int cardsThatCanShedCount;
    private int cardsThatWillSupernovaCount;
    private bool hasStardustGlowed;

    public event EventHandler OnTurnPhase_GenerateLightAndStardust;
    public event EventHandler OnTurnPhase_Age;
    public event EventHandler OnTurnPhase_Shed;
    public event EventHandler OnTurnPhase_BlackHoleSuck;

    private void Awake()
    {
        Instance = this;
    }

    public enum State
    {
        Null,
        GenerateLightAndStardust,
        Age,
        Shed,
        BlackHoleSuck
    }

    private void Start()
    {
        CardGameManager.Instance.OnBeginTurn += Instance_OnBeginTurn1;
        BankUI.Instance.GetStardustGlowShaderController().OnDoneGlowing += StardustBank_OnDoneGlowing;
    }

    private void Instance_OnBeginTurn1(object sender, CardGameManager.OnBeginTurnEventArgs e)
    {
        BeginTurnAnimations(e.firstTurn);
    }

    private void Instance_OnBeginTurn(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void BeginTurnAnimations(bool firstTurn)
    {
        if (firstTurn)
        {
            SetStateFirstTurn(State.GenerateLightAndStardust);
        }
        else
        {
            SetState(State.GenerateLightAndStardust);
        }
        
    }

    private void SetState(State state)
    {
        this.state = state;

        switch (state)
        {
            case State.GenerateLightAndStardust:
                playerCardsCount = playerField.GetAllPlayerExpertCards().ToList().Where(x => !x.IsCardDestroyed()).Count();
                hasStardustGlowed = false;
                GenerateStardustAnimations();
                GenerateLightAnimations();
                break;
            
            case State.Shed:
                shedCount = 0;
                //The IsCardDestroyed is necessary in case they finish the turn too fast before the animation is over and the card still exists
                cardsThatCanShedCount = playerField
                    .GetAllPlayerExpertCards()
                    .Where(x => x.IsShed() 
                        && !x.IsCardDestroyed() 
                        && !x.GetStatusList()
                            .Select(y => y.statusType)
                            .Contains(EffectTypeEnum.Strange))
                    .Count();
                if (cardsThatCanShedCount == 0)
                {
                    SetState(State.Age);
                }
                else
                {
                    ShedAnimations();
                }
                break;
            case State.Age:
                
                cardsThatCanAgeCount = playerField
                    .GetAllPlayerExpertCards()
                    .Where(x => !x.IsShed() 
                        && !x.IsCardDestroyed()
                        && !x.GetStatusList()
                            .Select(y => y.statusType)
                            .Contains(EffectTypeEnum.Strange))
                    .Count();
                cardsThatWillSupernovaCount = playerField
                    .GetAllPlayerExpertCards()
                    .Where(x => x.WillSupernova() 
                        && !x.IsCardDestroyed()
                        && !x.GetStatusList()
                            .Select(y => y.statusType)
                            .Contains(EffectTypeEnum.Strange))
                    .Count();
                agedCount = 0;
                if (cardsThatCanAgeCount == 0){
                    SetState(State.BlackHoleSuck);
                }else {
                OnTurnPhase_Age?.Invoke(this, EventArgs.Empty);
                }
                //AgeAnimations();
                break;
            
            
            case State.BlackHoleSuck:
                BlackHoleSuckAnimations();
                break;
        }
    }
    private void SetStateFirstTurn(State state)
    {
        this.state = state;

        switch (state)
        {
            case State.GenerateLightAndStardust:
                playerCardsCount = playerField.GetAllPlayerExpertCards().ToList().Where(x => !x.IsCardDestroyed()).Count();
                hasStardustGlowed = false;
                GenerateStardustAnimationsFirstTurn();
                GenerateLightAnimations();
                break;

            case State.Shed:
                shedCount = 0;
                //The IsCardDestroyed is necessary in case they finish the turn too fast before the animation is over and the card still exists
                cardsThatCanShedCount = playerField.GetAllPlayerExpertCards()
                    .Where(x => x.IsShed() && !x.IsCardDestroyed()).Count();
                if (cardsThatCanShedCount == 0)
                {
                    SetState(State.Age);
                }
                else
                {
                    ShedAnimations();
                }
                break;
            case State.Age:

                cardsThatCanAgeCount = playerField.GetAllPlayerExpertCards().Where(x => !x.IsShed() && !x.IsCardDestroyed()).Count();
                cardsThatWillSupernovaCount = playerField.GetAllPlayerExpertCards().Where(x => x.WillSupernova() && !x.IsCardDestroyed()).Count();
                agedCount = 0;
                if (cardsThatCanAgeCount == 0)
                {
                    SetState(State.BlackHoleSuck);
                }
                else
                {
                    OnTurnPhase_Age?.Invoke(this, EventArgs.Empty);
                }
                //AgeAnimations();
                break;


            case State.BlackHoleSuck:
                BlackHoleSuckAnimations();
                break;
        }
    }

    public void Aged()
    {
        agedCount++;
        if (agedCount == cardsThatCanAgeCount)
        {
            SetState(State.BlackHoleSuck);
        }
    }
    private bool HasCardsOnField()
    {
        return playerCardsCount > 0;
    }
    
    //ugly spaghetti code but no idea how to untangle
    private void GenerateLightAnimations()
    {
        cardsGlowed = 0;
        turnLightIncrement = 0;

        IPlayer player = CardGameManager.Instance.GetPlayer();
        List<BaseCard> playerCardsOnPlay = playerField.GetAllPlayerExpertCards().ToList();

        

        foreach(BaseCard card in playerCardsOnPlay)
        {
            card.GetCardUI().GetLightGlowShaderController().OnDoneGlowing += CardLight_OnDoneGlowing; 
        }

        int lightSumGeneration = playerCardsOnPlay.Select(x => x.GenerateLight()).Aggregate(0, (accumulatedLightSum, light) =>
        {
            accumulatedLightSum += light;
            return accumulatedLightSum;
        });

        turnLightIncrement += lightSumGeneration;
        turnLightIncrement += player.GetNeutronStar() * 10 + player.GetWhiteDwarf() * 5 - player.GetBlackHole() * 10;
        player.SetLight(player.GetLight() + turnLightIncrement);


        
    }
    private void StardustBank_OnDoneGlowing(object sender, EventArgs e)
    {
        hasStardustGlowed = true;
        if (!HasCardsOnField())
        {
            SetState(State.Shed);
        } else if (HaveCardsGlowed())
        {
            SetState(State.Shed);
        }
    }
    private void CardLight_OnDoneGlowing(object sender, EventArgs e)
    {
        Debug.Log("CardLight_OnDoneGlowing");
        cardsGlowed++;
        //ugly perhaps this is better done right in the card... so that don't have to do it every turn
        GlowShaderController glowShader = sender as GlowShaderController;
        glowShader.OnDoneGlowing -= CardLight_OnDoneGlowing;

        if (HaveCardsGlowed() && hasStardustGlowed)
        {
            SetState(State.Shed);
        }

    }
  
    private bool HaveCardsGlowed()
    {
        if(playerCardsCount > 0 && cardsGlowed == playerCardsCount)
        {
            return true;
        }
        else { return false; }
    }

    public void OnShedDone()
    {
        shedCount++;
        if(shedCount == cardsThatCanShedCount)
        {
            SetState(State.Age);
        }
    }

    private void GenerateStardustAnimationsFirstTurn()
    {
        IPlayer player = CardGameManager.Instance.GetPlayer();

        int stardustPerTurn = 5;


        BankUI.Instance.GlowStardust(stardustPerTurn);
    }
    private void GenerateStardustAnimations()
    {
        IPlayer player = CardGameManager.Instance.GetPlayer();

        int stardustPerTurn = 5;

        player.SetStardust(player.GetStardust() + stardustPerTurn);

        BankUI.Instance.GlowStardust(stardustPerTurn);
    }



    private void ShedAnimations()
    {
        OnTurnPhase_Shed?.Invoke(this, EventArgs.Empty);
    }

    private void BlackHoleSuckAnimations()
    {
        OnTurnPhase_BlackHoleSuck?.Invoke(this, EventArgs.Empty);
    }

    public State GetTurnPhase()
    {
        return state;
    }
    public int GetCardsThatCanShedCount()
    {
        return cardsThatCanShedCount;
    }
    public int GetCardsThatWillSupernovaCount()
    {
        return cardsThatWillSupernovaCount;
    }


}
