using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;


public class ExpertCard : BaseCard
{
    

    private int level = 1;
    
    private LevelUpMenuUI levelUpMenuUI;
    private int lifetime;
    private int experience;
    public event EventHandler OnLevelUp;
    private int health;
    private bool isGiant = false;



    override protected void Awake()
    {
        base.Awake();
        experience = 0;
        
        level = 1;
        
        cardUI = GetComponentInChildren<CardUI>();
        levelUpMenuUI = GetComponentInChildren<LevelUpMenuUI>();
        //RefreshBaseEnergyGeneration();
    }

    override  public void SetCardSO(CardSO cardSO)
    {
       base.SetCardSO(cardSO);
        
        lifetime = cardSO.Lifetime;
    }

    override public bool IsCardDestroyed(){

        return isDestroyed;
    }

    private void RefreshBaseEnergyGeneration()
    {
        //TalentValues baseEnergyGeneration = cardSO.expertStatsObjectFromJson.levels[level - 1].energyGeneration;
        if (baseEnergyGeneration.art != 0) { base.baseEnergyGeneration.art = baseEnergyGeneration.art; }
        if (baseEnergyGeneration.brawn != 0) { base.baseEnergyGeneration.brawn = baseEnergyGeneration.brawn; }
        if (baseEnergyGeneration.philosophy != 0) { base.baseEnergyGeneration.philosophy = baseEnergyGeneration.philosophy; }
        if (baseEnergyGeneration.science != 0) { base.baseEnergyGeneration.science = baseEnergyGeneration.science; }
    }
    override protected void CardGameManager_OnEndTurn(object sender, EventArgs e)
    {
        base.CardGameManager_OnEndTurn(sender, e);
        if (GetCardOwner() == Player.Instance.OpponentIs())
        {
            Age();
            if (!isDestroyed) { 
            cardUI.RefreshUI();
            }
            if (lifetime < 0)
            {
                if (!isDestroyed)
                {
                    Destroy(gameObject);
                    isDestroyed = true;
                };
            }
            else if (lifetime < 1)
            {
                if (!(cardSO.Rank == StarClassEnum.WHITE || cardSO.Rank == StarClassEnum.BLUE
                    || cardSO.Rank == StarClassEnum.YELLOW || cardSO.Rank == StarClassEnum.ORANGE))
                {
                    if (!isDestroyed)
                    {
                        Destroy(gameObject);
                        isDestroyed = true;
                    };
                }
                else if(cardSO.Rank == StarClassEnum.WHITE || cardSO.Rank == StarClassEnum.BLUE
                    || cardSO.Rank == StarClassEnum.YELLOW || cardSO.Rank == StarClassEnum.ORANGE) { cardUI.GetRedGiantImage(); }
            }
        }
    }
    override protected void CardGameManager_OnBeginTurn(object sender, EventArgs e)
    {
        base.CardGameManager_OnBeginTurn(sender, e);

        if (gameArea != GameAreaEnum.Hand && GetCardOwner() == Player.Instance.IAm()) {
            Age();
            if (lifetime < 1)
            {
                if(cardSO.Rank == StarClassEnum.WHITE || cardSO.Rank == StarClassEnum.BLUE
                    || cardSO.Rank == StarClassEnum.YELLOW || cardSO.Rank == StarClassEnum.ORANGE)
                {
                    //change card image
                    isGiant = true;
                    cardUI.GetRedGiantImage();
                }
                else {
                    DeadStarBank.Instance.BirthWhiteDwarf();
                    
                Destroy(gameObject);
                isDestroyed = true;
                }
            }
            if(lifetime < 0)
            {
                if (cardSO.Rank == StarClassEnum.WHITE || cardSO.Rank == StarClassEnum.BLUE
                    || cardSO.Rank == StarClassEnum.YELLOW || cardSO.Rank == StarClassEnum.ORANGE)
                {
                    //CardGameManager.Instance.GainStardust(stardust);
                    if(cardSO.Rank == StarClassEnum.ORANGE || cardSO.Rank == StarClassEnum.YELLOW)
                    {
                        DeadStarBank.Instance.BirthWhiteDwarf();
                    }else if (cardSO.Rank == StarClassEnum.WHITE 
                        || cardSO.Rank == StarClassEnum.BLUE)
                    {
                        CardGameManager.Instance.stardustFromSupernova += GetStardust();
                        if (cardSO.Rank == StarClassEnum.WHITE)
                        {
                            DeadStarBank.Instance.BirthNeutronStar();
                        }
                        else if (cardSO.Rank == StarClassEnum.BLUE)
                        {
                            DeadStarBank.Instance.BirthBlackHole();
                        }
                    }
                    
                    
                    
                    
                    //int cardIndex = OpponentPlayingField.Instance.GetAllOpponentExpertCards().FindIndex(x => x == targetCard);
                    //DivineMultiplayer.Instance.AbsorbServerRpc(cardIndex, targetCard.GetCardOwner());

                }
                Destroy(gameObject);
                isDestroyed = true;
            }
            cardUI.RefreshUI();
            
        }

        //reset
        energyGeneration.art = baseEnergyGeneration.art;
        energyGeneration.brawn = baseEnergyGeneration.brawn;
        energyGeneration.philosophy = baseEnergyGeneration.philosophy;
        energyGeneration.science = baseEnergyGeneration.science;

        int index = 0;
        List<Status> statusesToRemove = new List<Status>();
        foreach (Status status in statusList)
        {
            switch (status.statusType)
            {
                //        case EffectTypeEnum.BonusEnergy:
                //            //select all energy types that are generated and pick random one to boost
                //            List<RankEnum> energyTypesGenerated = new List<RankEnum>();

                //            if(energyGeneration.art != 0) 
                //            { 
                //                energyTypesGenerated.Add(RankEnum.Art); 
                //            }
                //            if (energyGeneration.brawn != 0)
                //            {
                //                energyTypesGenerated.Add(RankEnum.Brawn);
                //            }
                //            if (energyGeneration.philosophy != 0)
                //            {
                //                energyTypesGenerated.Add(RankEnum.Philosophy);
                //            }
                //            if (energyGeneration.science != 0)
                //            {
                //                energyTypesGenerated.Add(RankEnum.Science);
                //            }
                //            RankEnum energyTypeToBoost = ListUtility.GetRandomItemFromList(energyTypesGenerated);
                //            //Generate energy boost
                //            switch (energyTypeToBoost)
                //            {
                //                case RankEnum.Art:
                //                    energyGeneration.art += status.statusPower;
                //                    break;
                //                case RankEnum.Brawn:
                //                    energyGeneration.brawn += status.statusPower;
                //                    break; 
                //                case RankEnum.Philosophy:
                //                    energyGeneration.philosophy += status.statusPower;
                //                    break;
                //                case RankEnum.Science:
                //                    energyGeneration.science += status.statusPower;
                //                    break;
                //            }
                //            //Remove this status
                //            GetComponentInChildren<StatusIconsUI>().RemoveStatusIcon(index);
                //            statusesToRemove.Add(status);
                //            break;
                case EffectTypeEnum.Strange:
                    DecreaseCardLight();
                    DecreaseCardStardust();
                    //lifetime -= 1;
                    cardUI.RefreshUI();
                    if(GetCardLight() <= 0 || GetStardust() <= 0 || lifetime <= 0)
                    {
                        DeadStarBank.Instance.BirthBlackDwarf();
                        
                        Destroy(gameObject);
                        
                    }
                    //((ExpertCardUI)cardUI).RefreshExperience();
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

    public int GetLifetime()
    {
        return lifetime;
    }
    public void Age()
    {
        lifetime--;
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
                IncrementCardStardust(((ExpertCard)targetCard).GetStardust());
                
                //CardGameManager.Instance.GainStardust(((ExpertCard)targetCard).stardust);
                Destroy(targetCard.gameObject);
                int cardIndex = OpponentPlayingField.Instance.GetAllOpponentExpertCards().FindIndex(x => x == targetCard);
                DivineMultiplayer.Instance.AbsorbServerRpc(cardIndex, targetCard.GetCardOwner());
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
    public override bool IsGiant()
    {
        return isGiant;  
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
}


