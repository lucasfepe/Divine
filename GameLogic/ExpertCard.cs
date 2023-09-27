using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;


public class ExpertCard : BaseCard
{
    

    private int level = 1;
    private NetworkVariable<int> lifetime = new NetworkVariable<int>();
    private NetworkVariable<bool> isRedGiant = new NetworkVariable<bool>();
    private int experience;
    public event EventHandler OnLevelUp;
    private int health;
    private int cardStatDecrement = 0;
    

    override public void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
      
        lifetime.OnValueChanged += Lifetime_OnValueChanged;
        isRedGiant.OnValueChanged += IsRedGiant_OnValueChanged;
      
    }

    private void IsRedGiant_OnValueChanged(bool previousValue, bool newValue)
    {
        cardUI.BecomeRedGiant();
        
    }

    private void Lifetime_OnValueChanged(int previousValue, int newValue)
    {
        //update the ui 
        ((ExpertCardUI)cardUI).RefreshLifetime();
    }

    override protected void Awake()
    {
        base.Awake();
        experience = 0;
        
        level = 1;
        
        cardUI = GetComponentInChildren<CardUI>();
        //RefreshBaseEnergyGeneration();
    }

    override async public Task<bool> SetCardSO()
    {
       await base.SetCardSO();
        return true;
    }

    

   
    override protected void CardGameManager_OnEndTurn(object sender, EventArgs e)
    {
        base.CardGameManager_OnEndTurn(sender, e);
        cardStatDecrement = 0;
        if (GetCardOwner() == Player.Instance.OpponentIs())
        {
            
            if (lifetime.Value < 1)
            {
               if(cardSO.Rank == StarClassEnum.WHITE || cardSO.Rank == StarClassEnum.BLUE
                    || cardSO.Rank == StarClassEnum.YELLOW || cardSO.Rank == StarClassEnum.ORANGE) { cardUI.GetRedGiantImage(); }
            }
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void BecomeRedGiantServerRpc()
    {
        isRedGiant.Value = true;
    }
    override protected void CardGameManager_OnBeginTurn(object sender, EventArgs e)
    {
        base.CardGameManager_OnBeginTurn(sender, e);
        if(GetCardOwner() == Player.Instance.IAm())
            Age();
        
        int index = 0;
        List<Status> statusesToRemove = new List<Status>();
        foreach (Status status in statusList)
        {
            switch (status.statusType)
            {
               
                case EffectTypeEnum.Strange:
                    StrangeMatterEffect();
                    
                    //Remove this status
                    //GetComponentInChildren<StatusIconsUI>().RemoveStatusIcon(index);
                    statusesToRemove.Add(status);
                    break;

            }
            index++;
        }
        ////need this as removing inside loop causes error
        //statusList.RemoveAll(x => statusesToRemove.Contains(x));
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
        cardStatDecrement++;
    }
    public void DecrementStats()
    {
        
        
        SetCardStardust(cardLight.Value - cardStatDecrement);
        SetCardLight(cardLight.Value - cardStatDecrement);
            
    }
    public override void SetLifetime(int lifetime)
    {
        SetCardLifetimeServerRpc(lifetime);
    }
    public void Age()
    {
        int lifetimeDecrement = 1;
        if (gameArea != GameAreaEnum.Hand && GetCardOwner() == Player.Instance.IAm())
        {
            
            int nextAge = lifetime.Value - lifetimeDecrement;
            if (nextAge < 1)
            {

                if (cardSO.Rank == StarClassEnum.WHITE || cardSO.Rank == StarClassEnum.BLUE
                    || cardSO.Rank == StarClassEnum.YELLOW || cardSO.Rank == StarClassEnum.ORANGE)
                {
                    //change card image
                    BecomeRedGiantServerRpc();

                }
                else
                {
                    DeadStarBank.Instance.BirthWhiteDwarf();
                    Die();
                }
            }

            if (nextAge < 0)
            {
                if (cardSO.Rank == StarClassEnum.WHITE || cardSO.Rank == StarClassEnum.BLUE
                    || cardSO.Rank == StarClassEnum.YELLOW || cardSO.Rank == StarClassEnum.ORANGE)
                {
                    //need to destory before running logic because logic will consider this card as alive if do it later
                    Die();
                    //CardGameManager.Instance.GainStardust(stardust);
                    if (cardSO.Rank == StarClassEnum.ORANGE || cardSO.Rank == StarClassEnum.YELLOW)
                    {
                        DeadStarBank.Instance.BirthWhiteDwarf();
                    }
                    else if (cardSO.Rank == StarClassEnum.WHITE
                        || cardSO.Rank == StarClassEnum.BLUE)
                    {
                        CardGameManager.Instance.stardustFromSupernova += GetCardStardust();
                        if (cardSO.Rank == StarClassEnum.WHITE)
                        {
                            DeadStarBank.Instance.BirthNeutronStar();
                        }
                        else if (cardSO.Rank == StarClassEnum.BLUE)
                        {
                            DeadStarBank.Instance.BirthBlackHole();
                        }
                    }

                }

            }}
        if (!IsCardDestroyed())
            SetCardLifetimeServerRpc(lifetime.Value - lifetimeDecrement);
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
    override public void TargetSelected(BaseCard targetCard)
    {
        EffectTypeEnum effect = cardSO.GiantEffect.Effect;
        switch (effect)
        {
            case EffectTypeEnum.Absorb:
                //Reduce the health in my own game
                SetCardStardust(GetCardStardust() + targetCard.GetCardStardust());
                
                //CardGameManager.Instance.GainStardust(((ExpertCard)targetCard).stardust);
                
                targetCard.Die();
                
                break;
        }

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
    public void UseSkill()
    {




        Status newStatus = new Status();
        List<BaseCard> expertCards = PlayerPlayingField.Instance.GetAllPlayerExpertCards();
        List<BaseCard> filteredbaseCards;
        System.Random random = new System.Random();
        BaseCard randomExpertTargetCard;
        //Apply skill effect
        switch (cardSO.GiantEffect.Effect)
        {
            case EffectTypeEnum.Absorb:
                if(cardSO.GiantEffect.Preference == PreferenceEnum.Choose)
                    CardGameManager.Instance.BeginSelectingTarget(this);
                break;
        }

        //OnUseSkill?.Invoke(this, EventArgs.Empty);
    }

     override public int GetLifetime()
    {
        return lifetime.Value;
    }
}


