using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.LookDev;

public class BaseCard : NetworkBehaviour, ICard
{
    //how do I keep track of this card's stardust and light with a network variable in case a player disconnects 
    //if I make is a network Behaviour it will make two of it, can I have two of it but in different places in the two different players??
    //public event EventHandler<OnCardHoverEventArgs> OnCardHover;
    public static event EventHandler OnUseSkill;
    public event EventHandler OnCardHoverEnter;
    public event EventHandler OnCardHoverExit;
    protected CardSO cardSO;
    protected GameAreaEnum gameArea;
    protected CardUI cardUI;
    protected List<Status> statusList = new List<Status>();
    protected bool hasUsedSkillThisTurn = false;
    protected bool isDestroyed = false;
    protected TalentValues baseEnergyGeneration = new TalentValues();
    protected TalentValues energyGeneration = new TalentValues();
    private bool isPacified = false;
    protected Transform previousParent;
    protected RectTransform rectTransform;
    protected NetworkVariable<int> cardStardust = new NetworkVariable<int>();
    protected NetworkVariable<int> cardLight = new NetworkVariable<int>();
    protected NetworkVariable<bool> isTwinOn = new NetworkVariable<bool>(false);
    protected NetworkVariable<bool> targetable = new NetworkVariable<bool>(true);
    private Canvas canvas;

    public event EventHandler OnCardSOAssigned;
    public event EventHandler OnCancelSkill;
    public event EventHandler OnCardDieStardust;
    public event EventHandler OnCardDieLight;
    private IPlayer player;

    private NetworkVariable<FixedString32Bytes> cardTitle = new NetworkVariable<FixedString32Bytes>("");
    private NetworkVariable<PlayerEnum> cardOwner = new NetworkVariable<PlayerEnum>();

    [SerializeField] private SelectedCardVisual selectedCardVisual;
    [SerializeField] protected StatusIconsUI statusIconsUI;
    [SerializeField] private GlowShaderController lightGlower;

    public SelectedCardVisual GetHighlight()
    {
        return selectedCardVisual;
    }
    virtual protected void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        //player = CardGameManager.Instance.GetPlayer();
        gameArea = GameAreaEnum.Hand;
        cardUI = GetComponentInChildren<CardUI>();
        rectTransform = GetComponent<RectTransform>();
    }

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
    //public void PlayCard()
    //{
    //    PlayCardServerRpc();
    //}
    //[ServerRpc(RequireOwnership =false)]
    //private void PlayCardServerRpc()
    //{
    //    GetComponent<NetworkObject>().Spawn();
    //}
    override public void OnNetworkSpawn()
    {
        Hide();

        cardTitle.OnValueChanged += cardTitle_OnValueChanged;
        cardStardust.OnValueChanged += CardStardust_OnValueChanged;
        cardLight.OnValueChanged += CardLight_OnValueChanged;
        //cardOwner.OnValueChanged += cardOwner_OnValueChanged;

        //assign card so
    }

    private void CardLight_OnValueChanged(int previousValue, int newValue)
    {
        if (gameObject.activeSelf)
        {
            cardUI.RefreshLightText();
        }
        
    }

    private void CardStardust_OnValueChanged(int previousValue, int newValue)
    {
        if (gameObject.activeSelf)
        {
            cardUI.RefreshStardustText();
        }
        if (GetCardOwner() == Player.Instance.IAm()) { 
            CheckDestroyStardust(newValue);
        }
    }

    private void cardTitle_OnValueChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        SetCardSO();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
    

    
    public void PlaceCardInField()
    {
        //want to do this if card is mine
        if (cardOwner.Value == Player.Instance.IAm()) {
            Transform emptyExpertCardSlot = PlayerPlayingField.Instance.GetFirstOpenExpertCardPosition();
            transform.SetParent(emptyExpertCardSlot);
            SetCardGameArea(GameAreaEnum.Field);
            //Instead of adding it to a list and having the other play client make the card there, make the card as a network object here (in a server rpc) and have a client rpc to reparent the card based on whether it is I who played this card or my opponent
            //AddToFieldCardList(card.GetCardSO().Title);
            RectTransform rectTransform = transform.gameObject.GetComponent<RectTransform>();
            rectTransform.position = emptyExpertCardSlot.position;
        }
        else if (cardOwner.Value == Player.Instance.OpponentIs())
        {
            Transform emptyExpertCardSlot = OpponentPlayingField.Instance.GetFirstOpenExpertCardPosition();
            transform.SetParent(emptyExpertCardSlot);
            SetCardGameArea(GameAreaEnum.OpponentField);
            
            //Instead of adding it to a list and having the other play client make the card there, make the card as a network object here (in a server rpc) and have a client rpc to reparent the card based on whether it is I who played this card or my opponent
            //AddToFieldCardList(card.GetCardSO().Title);
            RectTransform rectTransform = transform.gameObject.GetComponent<RectTransform>();
            rectTransform.position = emptyExpertCardSlot.position;
           
        }
    }
    
    private void Show()
    {
        gameObject.SetActive(true);
    }
    
    virtual protected void Start()
    {
        
        CardGameManager.Instance.OnBeginTurn += CardGameManager_OnBeginTurn;
        CardGameManager.Instance.OnBeginTurnStep2 += CardGameManager_OnBeginTurnStep2;
        CardGameManager.Instance.OnEndTurn += CardGameManager_OnEndTurn;
        DivineMultiplayer.Instance.OnSpawnCard += DivineMultiplayer_OnSpawnCard;
       

    }

   

    virtual protected void DivineMultiplayer_OnSpawnCard(object sender, EventArgs e)
    {
    }

    virtual protected void CardGameManager_OnBeginTurnStep2(object sender, EventArgs e)
    {
        
    }

    virtual protected void CardGameManager_OnEndTurn(object sender, EventArgs e)
    {
        
    }

    virtual protected void CardGameManager_OnBeginTurn(object sender, EventArgs e)
    {
        hasUsedSkillThisTurn = false;
        if(GetCardOwner() == Player.Instance.IAm())
        ResetUntargetableServerRpc();


    }
    virtual public void InspectCard()
    {
        if (IsCardDestroyed()) return;
        InspectCardUI inspectCardUI1 = GameObject.Find("InspectCardUI").GetComponent<InspectCardUI>();
        
        inspectCardUI1.Show(this);
        previousParent = transform.parent;
        transform.SetParent(inspectCardUI1.transform);
        rectTransform.anchorMax = new Vector2(.5f, .5f);
        rectTransform.anchorMin = new Vector2(.5f, .5f);
        rectTransform.position = Vector2.zero;
        StartCoroutine(ChangeSorting());
        SetCardGameArea(GameAreaEnum.Inspect);
        //turn off highlight
        //selectedCardVisual.Hide();
    }

    private IEnumerator ChangeSorting()
    {
        yield return null;
        canvas.overrideSorting = true;
        int topSorting = 80;
        canvas.sortingOrder = topSorting;
    }
    private IEnumerator ChangeSortingBack()
    {
        yield return null;
        canvas.overrideSorting = false;
        int normalSorting = 0;
        canvas.sortingOrder = normalSorting;
    }

    virtual public void DoneInspectingCard()
    {
        
        if (GetCardGameArea() == GameAreaEnum.Inspect)
        {
            if(this is ExpertCard)
            {
                ((ExpertCardUI)cardUI).HideSkillDescUI();
            }
            
            transform.SetParent(previousParent);
            rectTransform.anchorMax = new Vector2(.5f, .5f);
            rectTransform.anchorMin = new Vector2(.5f, .5f);

            rectTransform.localPosition = Vector2.zero;
            canvas.sortingOrder = 0;
            canvas.overrideSorting = false;

            StartCoroutine(ChangeSortingBack());

            if (previousParent.TryGetComponent(out PlayerHand _))
            {
                
                SetCardGameArea(GameAreaEnum.Hand);
            }
            else
            {
                
                if(GetCardOwner() == Player.Instance.OpponentIs())
                {
                    SetCardGameArea(GameAreaEnum.OpponentField);
                }
                else
                {
                    SetCardGameArea(GameAreaEnum.Field);
                }
            }
            
            

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
    virtual async public Task<bool> SetCardSO()
    {
        if (cardSO == null) { 
        cardSO = await CardGenerator.Instance.CardNameToCardSO(cardTitle.Value.ToString());
        }
        OnCardSOAssigned?.Invoke(this, EventArgs.Empty);
        PlaceCardInField();
        DivineMultiplayer.Instance.CallOnSpawnCard();
        
        Show();
        SetRarityVisual();
        return true;
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
        cardOwner.Value = playerEnum;
    }
    public PlayerEnum GetCardOwner()
    {
        return cardOwner.Value;
    }
    public bool IsCardMine()
    {
        return cardOwner.Value == Player.Instance.IAm();
    }
    
    
    public void AddStatus(Status status)
    {
        AddStatusServerRpc(status.statusType,status.statusPower);
        
        
    }
    [ServerRpc(RequireOwnership =false)]
    private void AddStatusServerRpc(EffectTypeEnum statusType, int statusPower)
    {
        AddStatusClientRpc(statusType, statusPower);
    }
    
    
    [ClientRpc]
    private void AddStatusClientRpc(EffectTypeEnum statusType, int statusPower = 0)
    {
        Status status = new Status();
        status.statusType = statusType;
        status.statusPower = statusPower;
        this.statusList.Add(status);
        statusIconsUI.AddStatusIcon(status);
        if(statusType == EffectTypeEnum.Strange)
        {
            cardUI.Strangify();
        }
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
        Debug.Log("used skill@@@@@");
        if (!this is ExpertCard) return;
        Debug.Log("used skill, check");
        CardGameManager.Instance.GetPlayer().SetStardust(
            CardGameManager.Instance.GetPlayer().GetStardust() - ((ExpertCard)this).GetSkillStardustCost());
        hasUsedSkillThisTurn = true;
        
        OnUseSkill?.Invoke(this, EventArgs.Empty);
    }
    public virtual void CheckIfCanUseSkill()
    {

    }
    virtual public bool IsCardDestroyed()
    {
        return isDestroyed;
    }
    public int GenerateLight()
    {
        Debug.Log("GenLight");
        //as destroy only effects on the next frame this will still be called for destoryed cards this turn so check
        if (isDestroyed) return 0;
        //ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>();
        //ParticleSystem.Burst burst = particleSystem.emission.GetBurst(0);
        if(this is ExpertCard)
        {
            if (!((ExpertCard)this).WillGenerateLightNextTurn())
            {
                Debug.Log("will not gen light next turn");
                ((ExpertCard)this).ResetWillGenerateLightNextTurnServerRpc();
                lightGlower.CallOnDoneGlowing();

                ((ExpertCard)this).RemoveStatus(EffectTypeEnum.Dim);

                return 0;
            }else if(statusList.Select(x => x.statusType).Contains(EffectTypeEnum.Strange))
            {
                lightGlower.CallOnDoneGlowing();
                return 0;
            }

        }

        //burst.cycleCount = light;
        //particleSystem.emission.SetBurst(0, burst);
        //particleSystem.GetComponent<ParticleSystemRenderer>().material = Materials.Instance.yellowEnergyMaterial;

        cardUI.GlowLight();
        //particleSystem.Play();
        return cardLight.Value;
    }



    public void Pacify()
    {
        isPacified = true;
    }

     protected virtual void OnDestroy()
    {
        CardGameManager.Instance.OnBeginTurn -= CardGameManager_OnBeginTurn;
        CardGameManager.Instance.OnEndTurn -= CardGameManager_OnEndTurn;

        CardGameManager.Instance.OnBeginTurnStep2 -= CardGameManager_OnBeginTurnStep2;
        DivineMultiplayer.Instance.OnSpawnCard -= DivineMultiplayer_OnSpawnCard;
        

       

    }
    public bool IsPacified()
    {
        return isPacified;
    }
    virtual public bool IsGiant()
    {
        return false;
    }

    virtual public void TargetSelected(ICard targetCard) { }
    virtual public ViableTarget GetViableTarget() {  return null; }
    virtual public List<ICard> GetViableTargets() { return null; }
    public Transform GetPreviousParent()
    {
        return previousParent;
    }
    public int GetCardStardust()
    {
        return cardStardust.Value;
    }
    
    //ugly warning if use this ineaset of increment/decrement opponent won't be updated
    

    public void SetCardLight(int newValue, bool showChange = false)
    {
       

        //if(!IsCardDestroyed())
        
        if (showChange) {
            
            ((ExpertCardUI)cardUI).showStatChangeMessage(CardStats.Light,newValue - cardLight.Value);
        }
        SetCardLightServerRpc(newValue, showChange);
        //cardUI.RefreshLightText();
    }
    [ServerRpc(RequireOwnership =false)]
    private void SetCardLightServerRpc(int newValue, bool showChange = false)
    {
        if (showChange) {
            SetCardLightClientRpc(newValue - cardLight.Value);
        }
        cardLight.Value = newValue;
        CheckDestroyLight(newValue);
    }

    [ClientRpc]
    private void SetCardLightClientRpc(int change)
    {
        if (GetCardOwner() == Player.Instance.OpponentIs()) { 
            ((ExpertCardUI)cardUI).showStatChangeMessage(CardStats.Light, change);
        }
    }
   
    public int GetCardLight()
    {
        return cardLight.Value;
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

    
    
    
    public void SetCardTitle(string title)
    {
        cardTitle.Value = title;
        
    }

    public void PlayCard()
    {
        //will never be called
    }

    virtual public int GetLifetime()
    {
        return 0;
    }

    public void SetCardStardust(int newValue, bool showChange = false)
    {
        if (cardSO != null) { 
        }
        if (showChange)
        {
            ((ExpertCardUI)cardUI).showStatChangeMessage(CardStats.Stardust, newValue - cardStardust.Value);
        }
        //if(!IsCardDestroyed())
        SetCardStardustServerRpc(newValue, showChange);
        
    }

    [ServerRpc(RequireOwnership =false)]
    private void SetCardStardustServerRpc(int newValue, bool showChange = false)
    {
        if (showChange)
        {
            SetCardStardustClientRpc(newValue - cardStardust.Value);
        }

        cardStardust.Value = newValue;
        CheckDestroyStardust(newValue);
        
    }
    [ServerRpc(RequireOwnership = false)]
    protected void CardSuckedByBlackHoleServerRpc()
    {
        OnCardDieStardustClientRpc();
        if (!isDestroyed)
        {
            isDestroyed = true;
        }
    }
    protected void CardSuckedByBlackHoleServerRpc_SERVER()
    {
        OnCardDieStardustClientRpc();
        if (!isDestroyed)
        {
            isDestroyed = true;
        }
    }
    [ClientRpc]
    private void SetCardStardustClientRpc(int change)
    {
        if (GetCardOwner() == Player.Instance.OpponentIs())
        {
            ((ExpertCardUI)cardUI).showStatChangeMessage(CardStats.Stardust, change);
        }
    }
    private void CheckDestroyStardust(int value)
    {
        if (value <= 0 )
        {
            //DeadStarBankUI.Instance.UpdateDeadStarBankUI();

            OnCardDieStardustClientRpc();
            if (!isDestroyed) { 
                isDestroyed = true;
            }
        }else if(value <= 5 && (IsGiant() || ((ExpertCard)this).IsRedGiantLocal()) && !IsCardDestroyed())
        {
            CardSuckedByBlackHoleServerRpc();
            if (!isDestroyed)
            {
                isDestroyed = true;
            }
        }
        
    }
    public void CheckCardIsDead(int value)
    {
        if (value <= 0)
        {
            //DeadStarBankUI.Instance.UpdateDeadStarBankUI();

            if (!isDestroyed)
            {
                isDestroyed = true;
            }
        }
        else if (value <= 5 && (IsGiant() || ((ExpertCard)this).IsRedGiantLocal()) && !IsCardDestroyed())
        {
            if (!isDestroyed)
            {
                isDestroyed = true;
            }
        }
    }

   
    public virtual void WillBeSuckedByBlackHole()
    {

    }
    [ClientRpc]
    private void OnCardDieStardustClientRpc()
    {
        //if (GetCardOwner() == Player.Instance.OpponentIs()) return;
        WillBeSuckedByBlackHole();
        OnCardDieStardust?.Invoke(this, EventArgs.Empty);
    }

    private void CheckDestroyLight(int value)
    {
        if (value <= 0 )
        {
            OnCardDieLightClientRpc();
            
            if (!isDestroyed)
            {
                isDestroyed = true;
            }
        }

    }
    [ClientRpc]
    private void OnCardDieLightClientRpc()
    {
        WillBeSuckedByBlackHole();
        OnCardDieLight?.Invoke(this, EventArgs.Empty);
    }


    //ugly passing value too many times
    public void Disintegrate(float directionX)
    {
        isDestroyed = true;
        cardUI.Disintegrate(directionX);
    }
    public void Die()
    {
        //DeadStarBankUI.Instance.UpdateDeadStarBankUI();
        DestroyCardServerRpc();
        isDestroyed = true;
    }
   
    public void MakeDead()
    {
        if (isDestroyed) return;
        Debug.Log("MakeDead@@@");
        isDestroyed = true;
        if (GetCardOwner() == Player.Instance.IAm())
        {
            Debug.Log("IAM make dead");
            MakeDeadServerRpc();
        }
        if (InspectCardUI.Instance.GetActiveCard() == this)
        {
            InspectCardUI.Instance.Hide();
        }
    }
    public void MakeDeadIrregardlessOwner()
    {
        if (isDestroyed) return;
        Debug.Log("MakeDead@@@");
        isDestroyed = true;
        
            Debug.Log("IAM make dead");
            MakeDeadIrregardlessOwnerServerRpc();
        
        if (InspectCardUI.Instance.GetActiveCard() == this)
        {
            InspectCardUI.Instance.Hide();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void MakeDeadIrregardlessOwnerServerRpc()
    {
        MakeDeadIrregardlessOwnerClientRpc();
    }
    [ClientRpc]
    private void MakeDeadIrregardlessOwnerClientRpc()
    {

        if (isDestroyed) return;
        isDestroyed = true;


        if (InspectCardUI.Instance.GetActiveCard() == this)
        {
            InspectCardUI.Instance.Hide();
        }

    }
    [ServerRpc(RequireOwnership =false)]
    private void MakeDeadServerRpc()
    {
        MakeDeadClientRpc();
    }
    [ClientRpc]
    private void MakeDeadClientRpc()
    {
        if(GetCardOwner() == Player.Instance.OpponentIs())
        {
            MakeDead();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void DestroyCardServerRpc()
    {
        GetComponent<NetworkObject>().Despawn(true);
    }
    virtual public void SetLifetime(int lifetime)
    {
        
    }

    [ServerRpc(RequireOwnership =false)]
    public void GlowLightServerRpc()
    {
        GlowLightClientRpc();
    }
    [ClientRpc]
    public void GlowLightClientRpc()
    {

        if(cardOwner.Value == Player.Instance.OpponentIs())
        {
            cardUI.GlowLight();
        }
    }
    public string GetCardTitle()
    {
        return cardTitle.Value.ToString();
    }
    [ServerRpc(RequireOwnership = false)]
    public void ShedServerRpc()
    {
        ShedClientRpc();
    }
    [ClientRpc]
    private void ShedClientRpc()
    {
        if(cardOwner.Value == Player.Instance.OpponentIs())
        {
            ((ExpertCardUI)cardUI).ShedUI();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void BecomeAttackingServerRpc()
    {
        BecomeAttackingClientRpc();
    }
    [ClientRpc]
    private void BecomeAttackingClientRpc()
    {
        if (cardOwner.Value == Player.Instance.OpponentIs())
        {
            ((ExpertCardUI)cardUI).GetCardGraphicAnimator().BecomeAttacking();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DoneAttackingServerRpc()
    {
        DoneAttackingClientRpc();
    }
    [ClientRpc]
    private void DoneAttackingClientRpc()
    {
        if (cardOwner.Value == Player.Instance.OpponentIs())
        {
            ((ExpertCardUI)cardUI).GetCardGraphicAnimator().StopAttacking();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void BecomeTargetServerRpc()
    {
        BecomeTargetClientRpc();
    }
    [ClientRpc]
    private void BecomeTargetClientRpc()
    {
        if (cardOwner.Value == Player.Instance.IAm())
        {
            ((ExpertCardUI)cardUI).Flash();
            ((ExpertCardUI)cardUI).GetCardGraphicAnimator().BecomeTarget();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void StopBeingTargetServerRpc(PlayerEnum initiator)
    {
        StopBeingTargetClientRpc(initiator);
    }
    [ClientRpc]
    private void StopBeingTargetClientRpc(PlayerEnum initiator)
    {
        if (Player.Instance.IAm() == initiator) return;
        if (IsCardDestroyed()) return;
        if (cardOwner.Value == Player.Instance.IAm())
        {
            Debug.Log("done flash stopbneingatargetlcientrpc");
            ((ExpertCardUI)cardUI).DoneFlash();
            ((ExpertCardUI)cardUI).GetCardGraphicAnimator().StopBeingTarget();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void BecomeWhiteDwarfServerRpc()
    {
        BecomeWhiteDwarfClientRpc();
    }

    [ClientRpc]
    private void BecomeWhiteDwarfClientRpc()
    {
        if (GetCardOwner() == Player.Instance.IAm())
        {
            return;
        }
        else if (GetCardOwner() == Player.Instance.OpponentIs())
        {
            GetHighlight().Hide();
            MakeDead();
            cardUI.BecomeWhiteDwarf();
        }


    }

    [ServerRpc(RequireOwnership = false)]
    public void BecomeNeutronStarServerRpc(int stardustGain)
    {
        BecomeNeutronStarClientRpc(stardustGain);
    }

    [ClientRpc]
    private void BecomeNeutronStarClientRpc(int stardustGain)
    {
        if (GetCardOwner() == Player.Instance.IAm())
        {
            return;
        }
        else if (GetCardOwner() == Player.Instance.OpponentIs())
        {
            GetHighlight().Hide();
            MakeDead();
            cardUI.BecomeNeutronStar(stardustGain);
        }


    }

    [ServerRpc(RequireOwnership = false)]
    public void BecomeBlackHoleServerRpc(int stardustGain, bool meganova = false)
    {
        BecomeBlackHoleClientRpc(stardustGain, meganova);
    }
    [ServerRpc(RequireOwnership = false)]
    public void SupernovaNoBlackHoleServerRpc(int stardustGain = 0, bool meganova = false)
    {
        SupernovaNoBlackHoleClientRpc(stardustGain, meganova);
    }

    [ClientRpc]
    private void BecomeBlackHoleClientRpc(int stardustGain, bool meganova = false)
    {
        if (GetCardOwner() == Player.Instance.IAm())
        {
            return;
        }
        else if (GetCardOwner() == Player.Instance.OpponentIs())
        {
            GetHighlight().Hide();
            MakeDead();
            Debug.Log("BecomeBlackHoleClientRpc: meganova: " + meganova);
            cardUI.BecomeBlackHole(stardustGain, meganova);
        }


    }
    [ClientRpc]
    private void SupernovaNoBlackHoleClientRpc(int stardustGain, bool meganova = false)
    {
        if (GetCardOwner() == Player.Instance.IAm())
        {
            return;
        }
        else if (GetCardOwner() == Player.Instance.OpponentIs())
        {
            GetHighlight().Hide();
            MakeDead();
            cardUI.SupernovaNoBlackHole(stardustGain, meganova);
        }


    }

    internal void SetBlackHole(BlackHoleAnimator blackHoleAnimator)
    {
        blackHoleAnimator.OnBlackHoleSuck += BlackHoleAnimator_OnBlackHoleSuck;
    }

    private void BlackHoleAnimator_OnBlackHoleSuck(object sender, EventArgs e)
    {
        ((ExpertCard)this).IncreaseStatDecrementForBlackHole();
    }

    public virtual bool IsShed()
    {
        return false;
    }
    public virtual bool WillSupernova()
    {
        return false;
    }

    public bool IsTargetable()
    {
        return targetable.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    protected void BecomeUntargetableServerRpc()
    {
        targetable.Value = false;
        AddStatusClientRpc(EffectTypeEnum.Untargetable);
    }
    [ServerRpc(RequireOwnership = false)]
    private void ResetUntargetableServerRpc()
    {
        targetable.Value = true;
        RemoveStatusClientRpc(EffectTypeEnum.Untargetable);
    }
    [ClientRpc]
    virtual protected void RemoveStatusClientRpc(EffectTypeEnum effectType)
    {
        
    }

    public void SetRarityVisual()
    {
        switch (cardSO.Rarity)
        {
            case RarityEnum.Uncommon:
                cardUI.ShowEmerald();
                break;
            case RarityEnum.Rare:
                cardUI.ShowAmethyst();
                break;
            case RarityEnum.Unique:
                cardUI.ShowJade();
                break;

        }
    }
}
