using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class ExpertCard : BaseCard
{
    

    private int level = 1;
    private NetworkVariable<int> lifetime = new NetworkVariable<int>();
    private NetworkVariable<int> skillStardustCost = new NetworkVariable<int>();
    private NetworkVariable<bool> isRedGiant = new NetworkVariable<bool>();
    private NetworkVariable<int> skillStardustCostAlteration = new NetworkVariable<int>();
    private NetworkVariable<bool> willGenerateLightNextTurn = new NetworkVariable<bool>(true);

    private int experience;
    public event EventHandler OnLevelUp;
    
    private int health;
    private int cardStatDecrement = 0;
    private bool hasAbsorbed = false;
    private bool canUseSkill = false;
    private List<GameAreaEnum> locations;
    private List<StarClassEnum> ranks;
    private bool temporaryIsRedGiant = false;
    private bool willBecomeWhiteDwarf = false;
    private bool willBeSuckedByBlackHole = false;
    private bool isStardustDanger = false;
    private bool isLifetimeDanger = false;
    public event EventHandler OnDetermineIfCanUseSkill;
    private bool morph = false;
    private BlackHoleSuckAnimation blackHoleSuckAnimation;
    private bool willBeBlackHole = true;

    public event EventHandler OnShed;

    private ICard targetCard;
    public bool CanUseSkill()
    {
        return canUseSkill;
    }
    [ServerRpc(RequireOwnership = false)]
    public void WillNotGenerateLightNextTurnServerRpc()
    {
        willGenerateLightNextTurn.Value = false;
    }
    [ServerRpc(RequireOwnership = false)]
    public void ResetWillGenerateLightNextTurnServerRpc()
    {
        willGenerateLightNextTurn.Value = true;
    }
    public bool WillGenerateLightNextTurn()
    {
        return willGenerateLightNextTurn.Value;
    }
    public override void WillBeSuckedByBlackHole()
    {
        willBeSuckedByBlackHole = true;
    }

    override public void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        lifetime.OnValueChanged += Lifetime_OnValueChanged;
        isRedGiant.OnValueChanged += IsRedGiant_OnValueChanged;
        skillStardustCost.OnValueChanged += SkillStardustCost_OnValueChanged;
    }
    protected override void Start()
    {
        base.Start();
        ((ExpertCardUI)cardUI).OnEndAnimation += ExpertCard_OnEndAnimation;
        TurnPhaseStateMachine.Instance.OnTurnPhase_Age += TurnPhaseStateMachine_OnTurnPhaseAge;
        TurnPhaseStateMachine.Instance.OnTurnPhase_Shed += TurnPhaseStateMachine_OnTurnPhaseShed;
        TurnPhaseStateMachine.Instance.OnTurnPhase_BlackHoleSuck += OnTurnPhase_BlackHoleSuck;
        ((ExpertCardUI)cardUI).GetLifetimeAnimator().GetBehaviour<OnAnimationEnd>().OnAnimationEndEvent += LifetimeTextAnimator_OnAnimationEndEvent;
        OnUseSkill += ExpertCard_OnUseSkill;
    }

    private void ExpertCard_OnUseSkill(object sender, EventArgs e)
    {
        CheckIfCanUseSkill();
    }

    private void SkillStardustCost_OnValueChanged(int previousValue, int newValue)
    {
        CostContainer costContainer = ((ExpertCardUI)cardUI).GetCostContainer();
        costContainer.SetSkillStardustCost();

    }
    private void OnTurnPhase_BlackHoleSuck(object sender, EventArgs e)
    {
        if (GetCardOwner() == Player.Instance.IAm())
        {
            CheckIfCanUseSkill();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        ((ExpertCardUI)cardUI).OnEndAnimation -= ExpertCard_OnEndAnimation;
        TurnPhaseStateMachine.Instance.OnTurnPhase_Age -= TurnPhaseStateMachine_OnTurnPhaseAge;
        TurnPhaseStateMachine.Instance.OnTurnPhase_Shed -= TurnPhaseStateMachine_OnTurnPhaseShed;
        

    }
    private void LifetimeTextAnimator_OnAnimationEndEvent(object sender, EventArgs e)
    {
        if (GetCardOwner() == Player.Instance.OpponentIs()) return;
        PostAge();
    }

    private void TurnPhaseStateMachine_OnTurnPhaseShed(object sender, EventArgs e)
    {
        if (GetCardOwner() == Player.Instance.IAm()
            && IsShed()
            //not strange
            && !statusList.Select(x => x.statusType).Contains(EffectTypeEnum.Strange)
            )
        {
            string message = "";
            Sprite icon = null;
            icon = Resources.Load<Sprite>("stardustIcon");
            int stardustGain;

            ((ExpertCardUI)cardUI).ShedUI();
            int sheddingTreshold = 5;

            int stardustAfterShed = GetCardStardust() - cardSO.Shed;
            if (GetCardStardust() - cardSO.Shed >= sheddingTreshold)
            {
                stardustGain = cardSO.Shed;
                //CardGameManager.Instance.AddToTurnStardustIncrement(cardSO.Shed);
                cardStatDecrement += cardSO.Shed;
            }
            else
            {
                stardustGain = GetCardStardust() - sheddingTreshold;
                //CardGameManager.Instance.AddToTurnStardustIncrement(GetCardStardust() - sheddingTreshold);
                cardStatDecrement += GetCardStardust() - sheddingTreshold;
            }
            
            if (stardustAfterShed <= sheddingTreshold)
            {
                isStardustDanger = true;
                willBecomeWhiteDwarf = true;
                DeadStarBank.Instance.BirthWhiteDwarf();
                MakeDead();
                //This will prevent the flash if ability is usable

                //DeadStarBank.Instance.BirthWhiteDwarf();
                //Die();
            };

            DecrementStats();
            message = "+" + stardustGain;

            float xoffset = 0f;
            float yoffset = 1.35f;
            float delay = 0;
            if (stardustGain != 0)
            {
                StartCoroutine(AnimationManager.Instance.StatMessage(transform, message, icon, delay, xoffset, yoffset));
                StatMessageServerRpc(message, xoffset, yoffset);
            }
            CardGameManager.Instance.AddToTempStardustIncrement(stardustGain);
            CardGameManager.Instance.CardShed();
            

        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void StatMessageServerRpc(string message, float xoffset, float yoffset)
    {
        StatMessageClientRpc(message, xoffset, yoffset);
    }
    [ClientRpc]
    private void StatMessageClientRpc(string message, float xoffset, float yoffset)
    {
        if (GetCardOwner() == Player.Instance.IAm()) return;
        Sprite sprite = Resources.Load<Sprite>("stardustIcon");
        float delay = 0;
        StartCoroutine(AnimationManager.Instance.StatMessage(transform, message, sprite, delay, xoffset, yoffset));
    }
    private void TurnPhaseStateMachine_OnTurnPhaseAge(object sender, EventArgs e)
    {

        if (GetCardOwner() == Player.Instance.IAm()
            &&
            //don't age if it's a yellow or orange in red giant, they shed
            !IsShed()
            // dont' age if starnge
            && !statusList.Select(x => x.statusType).Contains(EffectTypeEnum.Strange)) {
            Age();
        }
    }

    private void ExpertCard_OnEndAnimation(object sender, EventArgs e)
    {
        CheckMorph();
    }

    public bool IsRedGiantLocal()
    {
        return temporaryIsRedGiant;
    }
    private void IsRedGiant_OnValueChanged(bool previousValue, bool newValue)
    {
        cardUI.BecomeRedGiant();
        //if the stardust is 5 or less here then get sucked by black hole

        if (GetCardOwner() == Player.Instance.IAm() && cardStardust.Value <= 5)
        {
            //((ExpertCardUI)cardUI).StardustDanger();
            CardSuckedByBlackHoleServerRpc();
            if (!isDestroyed)
            {
                //turn highlight off if it is on
                ((ExpertCardUI)cardUI).DoneFlash();
                Debug.Log("make card destoryed: " + GetCardSO().Title);
                isDestroyed = true;
                
                
            }
        }

    }

    private void Lifetime_OnValueChanged(int previousValue, int newValue)
    {
        //update the ui 
        if (GetCardOwner() == Player.Instance.IAm() && cardSO != null && (cardSO.Rank == StarClassEnum.WHITE || cardSO.Rank == StarClassEnum.BLUE) && (IsGiant() || IsRedGiantLocal()))
        {
            PostAge();
            TurnPhaseStateMachine.Instance.Aged();
        }
        else if (!IsGiant())
        {
            ((ExpertCardUI)cardUI).RefreshLifetime();
            //TurnPhaseStateMachine.Instance.Aged();
        }
        
    }
    protected override void DivineMultiplayer_OnSpawnCard(object sender, EventArgs e)
    {
        base.DivineMultiplayer_OnSpawnCard(sender, e);
        //Debug.Log("expert, DivineMultiplayer_OnSpawnCard");
        if (GetCardOwner() == Player.Instance.IAm()) {
            CheckIfCanUseSkill();
        }
    }
    protected override void CardGameManager_OnEndTurn(object sender, EventArgs e)
    {
        base.CardGameManager_OnEndTurn(sender, e);
        if (GetCardOwner() == Player.Instance.IAm())
        {
            CheckIfCanUseSkill();
            
        }

    }
    public override void CheckIfCanUseSkill()
    {
        
        //Debug.Log("Checking if acan use skill: " + cardSO.Title);
        //Debug.Log("CheckIfCanUseSkill");
        //Debug.Log("!hasAbsorbed && CardGameManager.Instance.IsMyTurn() && GetCardOwner() == Player.Instance.IAm() && !hasUsedSkillThisTurn: " + (!hasAbsorbed && CardGameManager.Instance.IsMyTurn() && GetCardOwner() == Player.Instance.IAm() && !hasUsedSkillThisTurn));
        //Debug.Log("hasAbsorbed: " + hasAbsorbed);
        //Debug.Log("hasUsedSkillThisTurn: " + hasUsedSkillThisTurn);
        //Debug.Log("GetCardOwner(): " + GetCardOwner());
        //Debug.Log("Player.Instance.IAm(): " + Player.Instance.IAm());
        canUseSkill = false;
        if (cardSO.GiantEffect != null
            && cardSO.GiantEffect.PassiveActive == PassiveActiveEnum.Active 
            && !hasAbsorbed 
            && CardGameManager.Instance.IsMyTurn() 
            && GetCardOwner() == Player.Instance.IAm() 
            && !hasUsedSkillThisTurn)
        {
            if (cardSO.GiantEffect.Effect == EffectTypeEnum.Strange)
            {
                if (CardGameManager.Instance.GetOpponent().GetNeutronStar() >= 2
                   && CardGameManager.Instance.GetPlayer().GetStardust() >= skillStardustCost.Value)
                {
                    if(cardSO.Rank == StarClassEnum.RED)
                    {
                        canUseSkill = true;
                    }
                    else if(IsRedGiantLocal() || isRedGiant.Value)
                    {
                        canUseSkill = true;
                    }
                }
            }
            else { 
                List<BaseCard> cardsInPlay = NetworkManager.Singleton.SpawnManager.SpawnedObjectsList
                    .Where(x => x.TryGetComponent(out BaseCard card))
                    .Select(x => x.GetComponent<BaseCard>())
                    .Where(x => !x.IsCardDestroyed())
                    .ToList();
                if (cardSO.GiantEffect.Effect == EffectTypeEnum.Uphold)
                {
                    cardsInPlay.Remove(this);
                }
                
                //Debug.Log("cardsInPlay coutn: " + cardsInPlay.Count);
                //Debug.Log("cardsinplay: " + cardsInPlay.Count);
                if (locations != null)
                {
                    string enumerable = String.Join(", ",locations.Select(x => x.ToString()));
                }
                if(ranks != null)
                {
                string enumerable2 = String.Join(", ", ranks.Select(x => x.ToString()));
                }
            
            
            
                //Debug.Log("isRedGiantLocal: " + IsRedGiantLocal());
                if (IsRedGiantLocal() || GetCardSO().Rank == StarClassEnum.RED)
                {
                    if(locations != null && ranks != null)
                    {
                    //Debug.Log("if: " + cardsInPlay
                    //.Where(x => locations.Contains(x.GetCardGameArea()))
                    //.Where(x => ranks.Contains(x.GetCardSO().Rank))
                    //.Any());
                    }
                    if(locations != null)
                    {
                    }
                    //Debug.Log("cardsInPlay.Any(): " + cardsInPlay.Any());
                
                    foreach (BaseCard card in cardsInPlay)
                    {
                    //Debug.Log("cardso: " + card.GetCardSO());
                    //Debug.Log("title: " + card.GetCardSO().Title);
                    //Debug.Log("card.GetCardSO().Rank: " + card.GetCardSO().Rank);
                    //Debug.Log("card.location: " + card.GetCardGameArea());
                    }
                    if (locations != null) {
                        foreach (BaseCard card in cardsInPlay.Where(x =>    locations.Contains(x.GetCardGameArea())).ToList())
                        {
                        }
                    }
                


                    foreach (BaseCard card in cardsInPlay)
                    {

                    //Debug.Log("cardso: " + card.GetCardSO());
                    //Debug.Log("title: " + card.GetCardSO().Title);
                    //Debug.Log("card.GetCardSO().Rank: " + card.GetCardSO().Rank);
                    }
                    if(ranks != null)
                    {
                    }
                
                    if (locations != null 
                        && ranks != null 
                        && CardGameManager.Instance.GetPlayer().GetStardust() >=        skillStardustCost.Value 
                        && (cardsInPlay
                            .Where(x => locations.Contains(x.GetCardGameArea()))
                            .Where(x => ranks.Contains(x.GetCardSO().Rank))
                            .Any()
                            || PlayerHand.Instance.GetHandCards()
                            .Where(x => locations.Contains(x.GetCardGameArea()))
                            .Where(x => ranks.Contains(x.GetCardSO().Rank))
                            .Any()
                            ))
                    {   
                        canUseSkill = true;
                    }else if(locations == null 
                            && ranks == null 
                            && CardGameManager.Instance.GetPlayer().GetStardust()           >= GetCardSO().GiantEffect.StardustCost)
                    {
                        canUseSkill = true;
                    };
                }
            }
        }
        OnDetermineIfCanUseSkill?.Invoke(this, EventArgs.Empty);
    }
    override protected void Awake()
    {
        base.Awake();
        experience = 0;
        blackHoleSuckAnimation = GetComponent<BlackHoleSuckAnimation>();
        level = 1;
        
        cardUI = GetComponentInChildren<CardUI>();
        //RefreshBaseEnergyGeneration();
    }

    override async public Task<bool> SetCardSO()
    {
       await base.SetCardSO();

        GiantEffect giantEffect = cardSO.GiantEffect;
        if (giantEffect != null)
        {
            ViableTarget viableTarget = giantEffect.ViableTarget;
            if (viableTarget != null)
            {
                locations = viableTarget.Locations;
                ranks = viableTarget.Ranks;
            }
        }
        return true;
    }

   
    
    [ServerRpc(RequireOwnership = false)]
    private void BecomeRedGiantServerRpc()
    {
        isRedGiant.Value = true;
    }
    override public bool IsShed()
    {
        return isRedGiant.Value && (cardSO.Rank == StarClassEnum.YELLOW || cardSO.Rank == StarClassEnum.ORANGE);
    }
    override public bool WillSupernova()
    {
        return isRedGiant.Value && (cardSO.Rank == StarClassEnum.WHITE || cardSO.Rank == StarClassEnum.BLUE);
    }
    
    override protected void CardGameManager_OnBeginTurn(object sender, EventArgs e)
    {
        base.CardGameManager_OnBeginTurn(sender, e);
        hasUsedSkillThisTurn = false;

        
        
        int index = 0;
        List<Status> statusesToRemove = new List<Status>();
        foreach (Status status in statusList)
        {
            switch (status.statusType)
            {
                //case EffectTypeEnum.Dim:

                //case EffectTypeEnum.Strange:
                //    StrangeMatterEffect();
                    
                //    //Remove this status
                //    //GetComponentInChildren<StatusIconsUI>().RemoveStatusIcon(index);
                //    statusesToRemove.Add(status);
                //    break;

            }
            index++;
        }
        
        ////need this as removing inside loop causes error
        //statusList.RemoveAll(x => statusesToRemove.Contains(x));
    }
    public void RemoveStatus(EffectTypeEnum statusType)
    {
        RemoveStatusServerRpc(statusType);
        
    }
    [ServerRpc(RequireOwnership = false)]
    private void RemoveStatusServerRpc(EffectTypeEnum statusType)
    {
        RemoveStatusClientRpc(statusType);
    }
    [ClientRpc]
    override protected void RemoveStatusClientRpc(EffectTypeEnum statusType)
    {
        int index = 0;
        List<Status> statusesToRemove = new List<Status>();
        foreach (Status status in statusList)
        {
            if (status.statusType == statusType)
                statusesToRemove.Add(status);

            index++;
        }

        ////need this as removing inside loop causes error
        statusList.RemoveAll(x => {
            if (statusesToRemove.Contains(x))
            {
                statusIconsUI.RemoveStatusIcon(x.statusType);
            }
            
            return statusesToRemove.Contains(x);
            });
    }
    override protected void CardGameManager_OnBeginTurnStep2(object sender, EventArgs e)
    {
        //need to do this here because by now hlack hole and neutron star will have actuated
        //what if I kill it right when the nv changes
        if (GetCardOwner() == Player.Instance.IAm() && cardStatDecrement != 0)
            DecrementStats();

        
    }
    override public void StrangeMatterEffect()
    {
        cardStatDecrement++;
    }
    override public void BlackHoleApproach()
    {
        if (GetCardOwner() == Player.Instance.IAm()) { 
            BlackHoleSuckAnimationServerRpc();
        }
        blackHoleSuckAnimation.Play();
    }

    [ServerRpc(RequireOwnership = false)]
    private void BlackHoleSuckAnimationServerRpc()
    {
        BlackHoleSuckAnimationClientRpc();
    }

    [ClientRpc]
    private void BlackHoleSuckAnimationClientRpc()
    {
        if(GetCardOwner() == Player.Instance.IAm()) { return; }

        blackHoleSuckAnimation.Play();
    }

    public void IncreaseStatDecrement(int decrement)
    {
        cardStatDecrement += decrement;
        DecrementStats();
    }

    public void DecrementStats()
    {
        SetCardStardust(cardStardust.Value - cardStatDecrement, true);
        SetCardLight(cardLight.Value - cardStatDecrement, true);
        
        cardStatDecrement = 0;
    }
    public void CheckMorphStardust()
    {
        if(isStardustDanger)
        {
            StardustDangerServerRpc();
        }
        if (isLifetimeDanger)
        {
            ((ExpertCardUI)cardUI).LifetimeDanger();
        }
    }

    [ServerRpc(RequireOwnership =false)]
    private void StardustDangerServerRpc()
    {
        StardustDangerClientRpc();
    }
    [ClientRpc]
    private void StardustDangerClientRpc()
    {
        ((ExpertCardUI)cardUI).StardustDanger();
    }
    public void CheckMorph()
    {
        if (morph) return;
        if (willBecomeWhiteDwarf)
        {
            MakeDead();
            morph = true;
            cardUI.BecomeWhiteDwarf();
        }
        if (willBeSuckedByBlackHole)
        {
            MakeDead();
            morph = true;
            cardUI.BeSuckedByBlackHole();
        }
    }
    public override void SetLifetime(int lifetime)
    {
        SetCardLifetimeServerRpc(lifetime);
    }
    private void PostAgeLocal()
    {
        if (lifetime.Value - 1 < 1)
        {
            if (cardSO.Rank == StarClassEnum.WHITE || cardSO.Rank == StarClassEnum.BLUE
                || cardSO.Rank == StarClassEnum.YELLOW || cardSO.Rank == StarClassEnum.ORANGE)
            {
                temporaryIsRedGiant = true;
            }
        }
    }
    private void PostAge()
    {
        if (lifetime.Value < 1)
        {
            if (cardSO.Rank == StarClassEnum.WHITE || cardSO.Rank == StarClassEnum.BLUE
                || cardSO.Rank == StarClassEnum.YELLOW || cardSO.Rank == StarClassEnum.ORANGE)
            {
                //change card image
                BecomeRedGiantServerRpc();
                
                
            }
            else
            {
                isLifetimeDanger = true;
                DeadStarBank.Instance.BirthWhiteDwarf();
                //kill card right here want it to be immediate don't want to wait for animation to have effect on field we already know if going to happen

                MakeDead();
                willBecomeWhiteDwarf = true;
                CheckMorphStardust();
                //DeadStarBank.Instance.BirthWhiteDwarf();
                //Die();
            }
        }

        if (lifetime.Value < 0)
        {
            if (cardSO.Rank == StarClassEnum.WHITE || cardSO.Rank == StarClassEnum.BLUE
                /* never triggered anymore || cardSO.Rank == StarClassEnum.YELLOW || cardSO.Rank == StarClassEnum.ORANGE*/
                )
            {
                MakeDead();
                //need to destory before running logic because logic will consider this card as alive if do it later
                //Die();
                //CardGameManager.Instance.GainStardust(stardust);

                //never triggered anymore
                //if (cardSO.Rank == StarClassEnum.ORANGE || cardSO.Rank == StarClassEnum.YELLOW)
                //{
                //    DeadStarBank.Instance.BirthWhiteDwarf();
                //}
                //else 
                //if (cardSO.Rank == StarClassEnum.WHITE
                //    || cardSO.Rank == StarClassEnum.BLUE)
                //{
                //will only gain stardust that was eaten otherwise OP I think

                //okay, will need to implement various supernova effects here

                if(cardSO.GiantEffect.Effect == EffectTypeEnum.WipeoutBothFields)
                {
                    NetworkManager.Singleton.SpawnManager.SpawnedObjectsList
                        .Where(x => x.TryGetComponent(out BaseCard card))
                        .Select(x => x.GetComponent<BaseCard>())
                        .ToList()
                        .ForEach(x => {
                            //x.MakeDeadIrregardlessOwner();
                            x.MakeDeadIrregardlessOwner();
                        });
                    CardGameManager.Instance.WipeOutBothFields();
                    
                }


                int stardustGain = 0;
                if (GetCardStardust() - cardSO.Stardust > 0)
                {
                    stardustGain = GetCardStardust() - cardSO.Stardust;
                }

                
                if (cardSO.Rank == StarClassEnum.WHITE)
                {
                    //Trigger neutron star animation
                    DeadStarBank.Instance.BirthNeutronStar();
                    cardUI.BecomeNeutronStar(stardustGain);
                    //DeadStarBank.Instance.BirthNeutronStar();
                }
                else if (cardSO.Rank == StarClassEnum.BLUE)
                {
                    if (willBeBlackHole) {
                        DeadStarBank.Instance.BirthBlackHole();
                        cardUI.BecomeBlackHole(stardustGain);
                    }
                    else
                    {
                        //kill card right here want it to be immediate don't want to wait for animation to have effect on field we already know if going to happen

                        cardUI.SupernovaNoBlackHole(stardustGain);
                    }
                    
                }
                //}

            }
        }
    }
    public void Age()
    {
        PostAgeLocal();
        int lifetimeDecrement = 1;
        if (gameArea != GameAreaEnum.Hand && GetCardOwner() == Player.Instance.IAm())
        {
            if (!IsCardDestroyed())
            SetCardLifetimeServerRpc(lifetime.Value - lifetimeDecrement);
            }
    }
    [ServerRpc(RequireOwnership =false)]
    private void SetCardLifetimeServerRpc(int newValue)
    {
        lifetime.Value = newValue;
    }
    public int GetLevel()
    {
        return level;
    }
    //public void LevelUp()
    //{
    //    //pay the cost
    //    if (GetCardOwner() == Player.Instance.IAm()) { 
    //    //TalentValues levelUpCost = cardSO.expertStatsObjectFromJson.levels[level - 1].levelUpCost;
    //    CardGameManager.Instance.bankedArt -= levelUpCost.art;
    //    CardGameManager.Instance.bankedBrawn -= levelUpCost.brawn;
    //    CardGameManager.Instance.bankedPhilosophy -= levelUpCost.philosophy;
    //    CardGameManager.Instance.bankedScience -= levelUpCost.science;
    //    CardGameManager.Instance.UpdateBankedEnergy();
    //    }
    //    //reset experience
    //    experience = 0;

    //    //update health
    //    health += cardSO.expertStatsObjectFromJson.levels[level].maxHealth - cardSO.expertStatsObjectFromJson.levels[level - 1].maxHealth;
    //    //increase level
    //    level++;
        
    //    //update level up menu
    //    levelUpMenuUI.UpdateLevelUpMenuUI();

    //    RefreshBaseEnergyGeneration();
    //    if (GetCardOwner() == Player.Instance.IAm()) {
    //        int index = PlayerPlayingField.Instance
    //            .GetAllPlayerExpertCards()
    //            .FindIndex(x => x == this);
    //        if(index == -1)
    //        {
               
    //        }
            
    //        DivineMultiplayer.Instance.LevelUpServerRpc(index, GetCardOwner());
    //    }
    //    OnLevelUp?.Invoke(this, EventArgs.Empty);
    //}
    //public bool CanLevelUp()
    //{
    //    if(!(cardSO.expertStatsObjectFromJson.levels.Count >= level + 1))
    //    {
    //        return false;
    //    }
    //    bool canBuyLevelUp = true;
    //    TalentValues levelUpCost = cardSO.expertStatsObjectFromJson.levels[level - 1].levelUpCost;
    //    if (levelUpCost.art != 0)
    //    {
    //        if (levelUpCost.art > CardGameManager.Instance.bankedArt)
    //        {
    //            canBuyLevelUp = false;
    //        }
    //    }
    //    else if (levelUpCost.brawn != 0)
    //    {
    //        if (levelUpCost.brawn > CardGameManager.Instance.bankedBrawn)
    //        {
    //            canBuyLevelUp = false;
    //        }
    //    }
    //    else if (levelUpCost.philosophy != 0)
    //    {
    //        if (levelUpCost.philosophy > CardGameManager.Instance.bankedPhilosophy)
    //        {
    //            canBuyLevelUp = false;
    //        }
    //    }
    //    else if (levelUpCost.science != 0)
    //    {
    //        if (levelUpCost.science > CardGameManager.Instance.bankedScience)
    //        {
    //            canBuyLevelUp = false;
    //        }
    //    }
    //    return gameObject.activeSelf == true
    //        && experience >= cardSO.expertStatsObjectFromJson.levels[level - 1].maxExperience
    //        && canBuyLevelUp;
    //}
    

    public void IncreaseExperience(int experience)
    {
        this.experience += experience;
    }
    public int GetExperience()
    {
        return experience;
    }
    public int GetHealth()
    {
        return health;
    }
    private void CheckAlive()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void TakeDamage(int damage)
    {
        this.health -= damage;
        CheckAlive();
    }

    override public ViableTarget GetViableTarget()
    {

        return cardSO.GiantEffect.ViableTarget;
    }
    override public void TargetSelected(ICard targetCard)
    {
        this.targetCard = targetCard;
        EffectTypeEnum effect = cardSO.GiantEffect.Effect;
        switch (effect)
        {
            case EffectTypeEnum.Absorb:
                ((ExpertCard)targetCard).GetComponentInChildren<DesintegrateAnimation>().OnStartAbsorbingStardust += ExpertCard_OnStartAbsorbingStardust;

                //do this for card in opponent field too
                ((ExpertCard)targetCard).AbsorbTargetSelectedServerRpc(NetworkObject.NetworkObjectId);
                //don't want the card to highlight as it's dieing
                //for me
                ((ExpertCard)targetCard).MakeDead();
                Destroy(((ExpertCard)targetCard).GetHighlight().gameObject);

                //CardGameManager.Instance.GainStardust(((ExpertCard)targetCard).stardust);
                //ugly don't need two things to do the same thing

                float directionX = transform.position.x - ((ExpertCard)targetCard).transform.position.x;
                ((ExpertCard)targetCard).Disintegrate(directionX);
                
                //targetCard.Die();
                hasAbsorbed = true;
               
                break;
            case EffectTypeEnum.NoBlackHole:

                ((ExpertCard)targetCard).willBeBlackHole = false;
                
                //somehow make a status on the target
                Status status = new Status();
                status.statusType = EffectTypeEnum.NoBlackHole;
                ((ExpertCard)targetCard).AddStatus(status);
                break;
            case EffectTypeEnum.Dim:

                ((ExpertCard)targetCard).WillNotGenerateLightNextTurnServerRpc();

                //somehow make a status on the target
                Status status2 = new Status();
                status2.statusType = EffectTypeEnum.Dim;
                ((ExpertCard)targetCard).AddStatus(status2);
                break;
            case EffectTypeEnum.Uphold:
                ((ExpertCard)targetCard).SetCardStardust(((ExpertCard)targetCard).GetCardStardust() + cardSO.GiantEffect.Power);
                break;
            case EffectTypeEnum.Injure:
                ((ExpertCard)targetCard).SetCardStardust(targetCard.GetCardStardust() - cardSO.GiantEffect.Power);
                ((ExpertCard)targetCard).CheckCardIsDead(targetCard.GetCardStardust() - cardSO.GiantEffect.Power);
                break;
            case EffectTypeEnum.Discount:
                ((BaseCardLocal)targetCard).Discount(cardSO.GiantEffect.Power);
                break;
        }
        UsedSkill();

    }

    [ServerRpc(RequireOwnership = false)]
    private void AbsorbTargetSelectedServerRpc(ulong prefabIdHash)
    {
        AbsorbTargetSelectedClientRpc(prefabIdHash);
    }
    [ClientRpc]
    private void AbsorbTargetSelectedClientRpc(ulong cardThatIsAbsorbing)
    {
        if (GetCardOwner() == Player.Instance.IAm()) { 
        NetworkObject cardThatIsAbsorbingCard = NetworkManager.Singleton.SpawnManager.SpawnedObjectsList.Where(x => x.TryGetComponent(out ExpertCard card)).Where(x => x.NetworkObjectId == cardThatIsAbsorbing).First();
            float directionX = cardThatIsAbsorbingCard.gameObject.transform.position.x - transform.position.x;

        Disintegrate(directionX);

        if (GetCardOwner() == Player.Instance.IAm())
        {
            MakeDead();
            Destroy(GetHighlight().gameObject);
        }
        }
    }

    private void ExpertCard_OnStartAbsorbingStardust(object sender, EventArgs e)
    {
         
        SetCardStardust(GetCardStardust() + targetCard.GetCardStardust(), true);
        SetCardLight(GetCardLight() + targetCard.GetCardLight(), true);
        
    }



    //public ExpertLevel GetExpertLevel()
    //{
    //return cardSO.expertStatsObjectFromJson.levels[level - 1];
    //}
    override public void DoneInspectingCard()
    {
        base.DoneInspectingCard();
        //levelUpMenuUI.Hide();
        //((ExpertCardUI)cardUI).HideSkills();
    }
    //ugly I am doing this kind of thing in expertcardui too
    override public void InspectCard()
    {
        base.InspectCard();
        //((ExpertCardUI)cardUI).ShowSkills();
    }
    override public bool IsGiant()
    {
        return isRedGiant.Value;
    }

   
    public bool HasAbsorbed()
    {
        return hasAbsorbed;
    }
    public void UseSkill()
    {
        
        
        //Apply skill effect
        switch (cardSO.GiantEffect.Effect)
        {
            
            case EffectTypeEnum.Absorb:
                if(cardSO.GiantEffect.Preference == PreferenceEnum.Choose)
                    CardGameManager.Instance.BeginSelectingTarget(this);
                break;
            case EffectTypeEnum.NoBlackHole:
                if (cardSO.GiantEffect.Preference == PreferenceEnum.Choose)
                    CardGameManager.Instance.BeginSelectingTarget(this, false);
                break;
            case EffectTypeEnum.Dim:
                if (cardSO.GiantEffect.Preference == PreferenceEnum.Choose)
                    CardGameManager.Instance.BeginSelectingTarget(this, false);
                break;
            case EffectTypeEnum.Uphold:
                Debug.Log("uphold use skill");
                if (cardSO.GiantEffect.Preference == PreferenceEnum.Choose)
                    CardGameManager.Instance.BeginSelectingTarget(this, false, transform.parent.GetSiblingIndex());
                break;
            case EffectTypeEnum.GenerateLight:
                
                int light = GenerateLight();
                CardGameManager.Instance.GetPlayer().SetLight(CardGameManager.Instance.GetPlayer().GetLight() + light);
                UsedSkill();
                break;
            case EffectTypeEnum.Discount:
                if (cardSO.GiantEffect.Preference == PreferenceEnum.Choose)
                    CardGameManager.Instance.BeginSelectingTarget(this, false, transform.parent.GetSiblingIndex());
                break;
            case EffectTypeEnum.Untargetable:

                BecomeUntargetableServerRpc();
                hasAbsorbed = true;
                UsedSkill();
                break;
            case EffectTypeEnum.Injure:
                if (cardSO.GiantEffect.Preference == PreferenceEnum.Choose) { 
                    CardGameManager.Instance.BeginSelectingTarget(this, false);
                }
                else
                {
                    UsedSkill();
                    List<BaseCard> opponentCards = NetworkManager.Singleton.SpawnManager.SpawnedObjectsList
                .Where(x => x.TryGetComponent(out BaseCard card))
                .Select(x => x.GetComponent<BaseCard>())
                .Where(x => !x.IsCardDestroyed() && x.GetCardOwner() == Player.Instance.OpponentIs())
                .ToList();
                    BaseCard targetCard = ListUtility.GetRandomItemFromList(opponentCards);
                    targetCard.SetCardStardust(targetCard.GetCardStardust() - cardSO.GiantEffect.Power);
                    targetCard.CheckCardIsDead(targetCard.GetCardStardust() - cardSO.GiantEffect.Power);
                }
                
                break;
            case EffectTypeEnum.Strange:
                UsedSkill();
                CardGameManager.Instance.GetOpponent().SetNeutronStar(CardGameManager.Instance.GetOpponent().GetNeutronStar() - 2);
                CardGameManager.Instance.GetOpponent().SetBlackHole(CardGameManager.Instance.GetOpponent().GetBlackHole() + 1);
                List<BaseCard> opponentCards2 = NetworkManager.Singleton.SpawnManager.SpawnedObjectsList
                .Where(x => x.TryGetComponent(out BaseCard card))
                .Select(x => x.GetComponent<BaseCard>())
                .Where(x => !x.IsCardDestroyed() && x.GetCardOwner() == Player.Instance.OpponentIs())
                .ToList();
                if(opponentCards2.Count > 0)
                {
                    BaseCard targetCard = ListUtility.GetRandomItemFromList(opponentCards2);
                    Status status = new Status();
                    status.statusType = EffectTypeEnum.Strange;
                    targetCard.AddStatus(status);
                }
                break;

        }
        

        
    }
    public int GetSkillStardustCost()
    {
        return skillStardustCost.Value;
    }
    override public int GetLifetime()
    {
        return lifetime.Value;
    }
    [ClientRpc]
    private void UpdateSkillStardustCostClientRpc()
    {
        CostContainer costContainer = ((ExpertCardUI)cardUI).GetCostContainer();
        costContainer.SetSkillStardustCost();
    }
    override public List<ICard> GetViableTargets()
    {
        //make all viable targets highlighted
        List<BaseCard> cardsInPlay = NetworkManager.Singleton.SpawnManager.SpawnedObjectsList.Where(x => x.TryGetComponent(out BaseCard card)).Select(x => x.GetComponent<BaseCard>()).ToList();
        List<BaseCardLocal> cardsInHand = PlayerHand.Instance.GetHandCards();
        ViableTarget viableTarget = cardSO.GiantEffect.ViableTarget;
        List<GameAreaEnum> locations = viableTarget.Locations;
        List<StarClassEnum> ranks = viableTarget.Ranks;

        if(cardSO.GiantEffect.Effect == EffectTypeEnum.Uphold)
        {
            cardsInPlay.Remove(this);
        }
        List<BaseCard> filteredCardsInPlay = cardsInPlay
            .Where(x => locations.Contains(x.GetCardGameArea()))
            .Where(x => ranks.Contains(x.GetCardSO().Rank))
            .Where(x => x.IsTargetable())
            .ToList();
        List<BaseCardLocal> filteredCardsInHand = cardsInHand.Where(x => locations.Contains(x.GetCardGameArea()))
            .Where(x => ranks.Contains(x.GetCardSO().Rank))
            .ToList();
        List<ICard> filteredCards = new List<ICard>(filteredCardsInPlay.Cast<ICard>());
        filteredCards.AddRange(new List<ICard>(filteredCardsInHand.Cast<ICard>()));


        return filteredCards;
            
    }

    public void IncreaseStatDecrementForBlackHole()
    {
        ((ExpertCardUI)cardUI).LightDust();
        ((ExpertCardUI)cardUI).StardustDust();
    }

    
}


