using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BaseCard : MonoBehaviour
{
    //how do I keep track of this card's stardust and light with a network variable in case a player disconnects 
    //if I make is a network Behaviour it will make two of it, can I have two of it but in different places in the two different players??
    //public event EventHandler<OnCardHoverEventArgs> OnCardHover;
    public event EventHandler OnCardHoverEnter;
    public event EventHandler OnCardHoverExit;
    protected CardSO cardSO;
    protected GameAreaEnum gameArea;
    protected CardUI cardUI;
    private PlayerEnum cardOwner;
    protected List<Status> statusList = new List<Status>();
    private bool hasUsedSkillThisTurn = false;
    protected bool isDestroyed = false;
    protected TalentValues baseEnergyGeneration = new TalentValues();
    protected TalentValues energyGeneration = new TalentValues();
    private bool isPacified = false;
    protected Transform previousParent;
    protected RectTransform rectTransform;
    private int cardStardust;
    private int cardLight;

    
    [SerializeField] private SelectedCardVisual selectedCardVisual;
    [SerializeField] private StatusIconsUI statusIconsUI;
    

    //public class OnCardHoverEventArgs : EventArgs
    //{
    //    public Card hoveredCard;
    //}
    virtual public void BlackHoleApproach()
    {

    }
    virtual public void StrangeMatterEffect()
    {

    }
    virtual protected void Awake()
    {
        
        gameArea = GameAreaEnum.Hand;
        cardUI = GetComponentInChildren<CardUI>();
        rectTransform = GetComponent<RectTransform>();
    }
    protected void Start()
    {
        CardGameManager.Instance.OnBeginTurn += CardGameManager_OnBeginTurn;
        CardGameManager.Instance.OnEndTurn += CardGameManager_OnEndTurn;
        
    }

    

    virtual protected void CardGameManager_OnEndTurn(object sender, EventArgs e)
    {
        
    }

    virtual protected void CardGameManager_OnBeginTurn(object sender, EventArgs e)
    {
        hasUsedSkillThisTurn = false;
        
    }
    virtual public void InspectCard()
    {
        InspectCardUI inspectCardUI1 = GameObject.Find("InspectCardUI").GetComponent<InspectCardUI>();
        
        inspectCardUI1.Show(this);
        previousParent = transform.parent;
        transform.SetParent(inspectCardUI1.transform);
        rectTransform.anchorMax = new Vector2(.5f, .5f);
        rectTransform.anchorMin = new Vector2(.5f, .5f);
        rectTransform.position = Vector2.zero;
        rectTransform.localScale = new Vector2(3f, 3f);
        SetCardGameArea(GameAreaEnum.Inspect);
        //turn off highlight
        selectedCardVisual.Hide();
    }
    virtual public void DoneInspectingCard()
    {
        if (GetCardGameArea() == GameAreaEnum.Inspect)
        {
            transform.SetParent(previousParent);
            rectTransform.anchorMax = new Vector2(.5f, .5f);
            rectTransform.anchorMin = new Vector2(.5f, .5f);

            rectTransform.localPosition = Vector2.zero;
            float scale;
            if (previousParent.TryGetComponent(out PlayerHand _))
            {
                scale = UniversalConstants.HAND_CARD_SCALE;
                SetCardGameArea(GameAreaEnum.Hand);
            }
            else
            {
                scale = UniversalConstants.FIELD_CARD_SCALE;
                if(GetCardOwner() == Player.Instance.OpponentIs())
                {
                    SetCardGameArea(GameAreaEnum.OpponentField);
                }
                else
                {
                    SetCardGameArea(GameAreaEnum.Field);
                }
            }
            
            rectTransform.localScale = new Vector2(scale, scale);

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
        cardStardust = cardSO.Stardust;
        cardLight = cardSO.Light;
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
    
    
    public void SetCardOwner(PlayerEnum playerEnum)
    {
        cardOwner = playerEnum;
    }
    public PlayerEnum GetCardOwner()
    {
        return cardOwner;
    }
    public bool IsCardMine()
    {
        return cardOwner == Player.Instance.IAm();
    }
    
    
    public void AddStatus(Status status)
    {
        this.statusList.Add(status);
        statusIconsUI.AddStatusIcon(status);
        
    }
    public List<Status> GetStatusList()
    {
        return statusList;
    }
    public bool HasUsedSkillThisTurn()
    {
        return hasUsedSkillThisTurn;
    }
    public void UsedSkill()
    {
        hasUsedSkillThisTurn = true;
    }
    virtual public bool IsCardDestroyed()
    {
        return false;
    }
    public int GenerateLight()
    {
        //as destroy only effects on the next frame this will still be called for destoryed cards this turn so check
        if (isDestroyed) return 0;
        //ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>();
        //ParticleSystem.Burst burst = particleSystem.emission.GetBurst(0);
        
        
            //burst.cycleCount = light;
            //particleSystem.emission.SetBurst(0, burst);
            //particleSystem.GetComponent<ParticleSystemRenderer>().material = Materials.Instance.yellowEnergyMaterial;


        cardUI.GlowLight();
        //particleSystem.Play();
        return cardLight;
    }



    public void Pacify()
    {
        isPacified = true;
    }

    private void OnDestroy()
    {
        CardGameManager.Instance.OnBeginTurn -= CardGameManager_OnBeginTurn;
        CardGameManager.Instance.OnEndTurn -= CardGameManager_OnEndTurn;
    }
    public bool IsPacified()
    {
        return isPacified;
    }
    virtual public bool IsGiant()
    {
        return false;
    }

    virtual public void TargetSelected(BaseCard targetCard) { }
    virtual public ViableTarget GetViableTarget() {  return null; }

    public Transform GetPreviousParent()
    {
        return previousParent;
    }
    public int GetStardust()
    {
        return cardStardust;
    }
    public void IncrementCardStardust(int value)
    {
         cardStardust += value;
        //update the uiSetStardustText
        cardUI.RefreshStardustText();
        if (cardOwner == Player.Instance.IAm())
        {
            //update other player
            DivineMultiplayer.Instance.UpdateCardStardustInOpponentFieldServerRpc(GetIndex(), cardStardust, Player.Instance.OpponentIs());
        }
        
    }
    //ugly warning if use this ineaset of increment/decrement opponent won't be updated
    public void SetCardStardust(int value)
    {
        cardStardust = value;
        cardUI.RefreshStardustText();
    }

    public void SetLight(int value)
    {
        cardLight = value;
        cardUI.RefreshLightText();
    }

    public void DecreaseCardStardust()
    {
        cardStardust--;
        cardUI.RefreshStardustText();
        if (cardOwner == Player.Instance.IAm())
        {
            //update other player
            DivineMultiplayer.Instance.UpdateCardStardustInOpponentFieldServerRpc(GetIndex(), cardStardust, Player.Instance.OpponentIs());
        }
        //update other player
    }
    public int GetCardLight()
    {
        return cardLight;
    }

    public CardUI GetCardUI()
    {
        return cardUI;
    }

    public void DecreaseCardLight()
    {
        cardLight--;
        cardUI.RefreshLightText();
        if (cardOwner == Player.Instance.IAm())
        {
            //update other player
            DivineMultiplayer.Instance.UpdateCardLightInOpponentFieldServerRpc(GetIndex(), cardLight, Player.Instance.OpponentIs());
        }
    }
    private int GetIndex()
    {
        return PlayerPlayingField.Instance
                .GetAllPlayerExpertCards()
                .FindIndex(x => x == this);
    }

    public int GetLight()
    {
        return cardLight;
    }

}
