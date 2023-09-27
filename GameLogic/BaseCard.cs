using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.LookDev;

public class BaseCard : NetworkBehaviour, ICard
{
    //how do I keep track of this card's stardust and light with a network variable in case a player disconnects 
    //if I make is a network Behaviour it will make two of it, can I have two of it but in different places in the two different players??
    //public event EventHandler<OnCardHoverEventArgs> OnCardHover;
    public event EventHandler OnCardHoverEnter;
    public event EventHandler OnCardHoverExit;
    protected CardSO cardSO;
    protected GameAreaEnum gameArea;
    protected CardUI cardUI;
    protected List<Status> statusList = new List<Status>();
    private bool hasUsedSkillThisTurn = false;
    protected bool isDestroyed = false;
    protected TalentValues baseEnergyGeneration = new TalentValues();
    protected TalentValues energyGeneration = new TalentValues();
    private bool isPacified = false;
    protected Transform previousParent;
    protected RectTransform rectTransform;
    protected NetworkVariable<int> cardStardust = new NetworkVariable<int>();
    protected NetworkVariable<int> cardLight = new NetworkVariable<int>();

    public event EventHandler OnCardSOAssigned;

    private IPlayer player;

    private NetworkVariable<FixedString32Bytes> cardTitle = new NetworkVariable<FixedString32Bytes>("");
    private NetworkVariable<PlayerEnum> cardOwner = new NetworkVariable<PlayerEnum>();

    [SerializeField] private SelectedCardVisual selectedCardVisual;
    [SerializeField] private StatusIconsUI statusIconsUI;
    virtual protected void Awake()
    {
        
        player = CardGameManager.Instance.GetPlayer();
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
            cardUI.GlowLight();
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
            cardUI.GlowLight();
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
    
    protected void Start()
    {
        
        CardGameManager.Instance.OnBeginTurn += CardGameManager_OnBeginTurn;
        CardGameManager.Instance.OnBeginTurnStep2 += CardGameManager_OnBeginTurnStep2;
        CardGameManager.Instance.OnEndTurn += CardGameManager_OnEndTurn;
        
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
        cardSO = await CardGenerator.Instance.CardNameToCardSO(cardTitle.Value.ToString());
        OnCardSOAssigned?.Invoke(this, EventArgs.Empty);
        PlaceCardInField();
        Show();
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
        return isDestroyed;
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
        return cardLight.Value;
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
    public int GetCardStardust()
    {
        return cardStardust.Value;
    }
    
    //ugly warning if use this ineaset of increment/decrement opponent won't be updated
    

    public void SetCardLight(int newValue)
    {
        if(!IsCardDestroyed())
        SetCardLightServerRpc(newValue);
        //cardUI.RefreshLightText();
    }
    [ServerRpc(RequireOwnership =false)]
    private void SetCardLightServerRpc(int newValue)
    {
        cardLight.Value = newValue;
        CheckDestroy(newValue);
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

    public void SetCardStardust(int newValue)
    {
        if(!IsCardDestroyed())
        SetCardStardustServerRpc(newValue);
    }

    [ServerRpc(RequireOwnership =false)]
    private void SetCardStardustServerRpc(int newValue)
    {
        cardStardust.Value = newValue;
       
        CheckDestroy(newValue);
    }
    private void CheckDestroy(int value)
    {
        if (value <= 0 && !isDestroyed)
        {
           DeadStarBank.Instance.BirthBlackDwarf();
        //DeadStarBankUI.Instance.UpdateDeadStarBankUI();
        DestroyCardServerRpc();
        isDestroyed = true; 
        }
        
    }
    public void Die()
    {
        //DeadStarBankUI.Instance.UpdateDeadStarBankUI();
        DestroyCardServerRpc();
        isDestroyed = true;
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
}
