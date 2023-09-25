using Mono.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillButton : MonoBehaviour
{

    private ExpertAction action;
    private BaseCard parentCard;
    public event EventHandler OnUseSkill;

    //public bool CanUseSkill()
    //{
    //    if (CardGameManager.Instance.IsMyTurn() == false) { return false; }
    //    if (parentCard.IsPacified()) return false;
    //    //Can only use skills on my own cards duh
    //    if (parentCard.GetCardOwner() != Player.Instance.IAm()) { return false; }
    //    //Can't use skill if already used a skill this turn
    //    if (parentCard.HasUsedSkillThisTurn()) return false;

    //    ExpertAction action = parentCard.GetCardSO().expertStatsObjectFromJson
    //        .levels[((ExpertCard)parentCard).GetLevel() - 1].expertActions[0];
    //    TalentValues actionCost = action.cost;
    //    ActionEffect actionEffect = action.effect;
    //    bool canUseSkill = true;

    //    //Can Afford Skill
    //    if (actionCost.art != 0)
    //    {
    //        canUseSkill = CardGameManager.Instance.bankedArt >= actionCost.art;
    //    }
    //    if (actionCost.brawn != 0)
    //    {
    //        canUseSkill = CardGameManager.Instance.bankedBrawn >= actionCost.brawn;
    //    }
    //    if (actionCost.philosophy != 0)
    //    {
    //        canUseSkill = CardGameManager.Instance.bankedPhilosophy >= actionCost.philosophy;
    //    }
    //    if (actionCost.science != 0)
    //    {
    //        canUseSkill = CardGameManager.Instance.bankedScience >= actionCost.science;
    //    }

    //    if (canUseSkill) { 
    //    switch (actionEffect.effectType)
    //    {
    //        case EffectTypeEnum.BonusEnergy:
    //                //check if viable targets
    //                canUseSkill = PlayerPlayingField.Instance.GetAllPlayerExpertCards()
    //                .Where(x => x != parentCard)
    //                .Where(x => action.viableTarget.cardTalentClasses.Contains(x.GetCardSO().cardTalentClass)).Any();
    //            break;
    //        case EffectTypeEnum.GainExperience:
    //                //check if viable targetsDebug.Log("@1");
    //                canUseSkill = PlayerPlayingField.Instance.GetAllPlayerExpertCards()
    //                .Where(x => x != parentCard)
    //                .Where(x => action.viableTarget.cardTalentClasses.Contains(x.GetCardSO().cardTalentClass))
    //                .Where(x => ((ExpertCard)x).GetExperience() < x.GetCardSO().expertStatsObjectFromJson.levels[((ExpertCard)x).GetLevel() - 1].maxExperience).Any();
    //            break;
    //            case EffectTypeEnum.Attack:
    //                canUseSkill = OpponentPlayingField.Instance.GetAllOpponentExpertCards().Count > 0;
    //                break;

    //        }
    //    }
        
    //    return canUseSkill;
    //}

    public void UseSkill()
    {
        parentCard.UsedSkill();
        //close the inspect
        //InspectCardUI.Instance.Hide();
        if(parentCard.GetType() == typeof(ExpertCard))
        {
            //((ExpertCard)parentCard).IncreaseExperience(action.experienceGain);
            //TODO put all these buying methods centralized in cardGameManger or interface or something
            //if (action.cost.art != 0)
            //{
            //    CardGameManager.Instance.bankedArt -= action.cost.art;
            //}
            //if (action.cost.brawn != 0)
            //{
            //    CardGameManager.Instance.bankedBrawn -= action.cost.brawn;
            //}
            //if (action.cost.philosophy != 0)
            //{
            //    CardGameManager.Instance.bankedPhilosophy -= action.cost.philosophy;
            //}
            //if (action.cost.science != 0)
            //{
            //    CardGameManager.Instance.bankedScience -= action.cost.science;
            //}
            //CardGameManager.Instance.UpdateBankedEnergy();

            Status newStatus = new Status();
            List<BaseCard> expertCards = PlayerPlayingField.Instance.GetAllPlayerExpertCards();
            List<BaseCard> filteredbaseCards;
            System.Random random = new System.Random();
            BaseCard randomExpertTargetCard;
            //Apply skill effect
            //switch (action.effect.effectType)
            //{
            //    case EffectTypeEnum.BonusEnergy:
                    
            //        newStatus.statusType = EffectTypeEnum.BonusEnergy;
            //        newStatus.statusPower = action.effect.power;
            //        filteredbaseCards = expertCards
            //            .Where(x => action.viableTarget.cardTalentClasses.Contains(x.GetCardSO().cardTalentClass))
            //            .Where(x => x != parentCard)
            //            .ToList();
            //        randomExpertTargetCard = ListUtility.GetRandomItemFromList(filteredbaseCards); 
            //        randomExpertTargetCard.AddStatus(newStatus);
            //        break;

            //    case EffectTypeEnum.GainExperience:

            //        newStatus.statusType = EffectTypeEnum.GainExperience;
            //        newStatus.statusPower = action.effect.power;
            //        filteredbaseCards = expertCards
            //        .Where(x => action.viableTarget.cardTalentClasses.Contains(x.GetCardSO().cardTalentClass))
            //        .Where(x => x != parentCard)
            //        .Where(x => ((ExpertCard)x).GetExperience() < x.GetCardSO().expertStatsObjectFromJson.levels[((ExpertCard)x).GetLevel() - 1].maxExperience)
            //        .ToList();

                 
            //        randomExpertTargetCard = ListUtility.GetRandomItemFromList(filteredbaseCards);

            //        randomExpertTargetCard.AddStatus(newStatus);
            //        break;
            //    case EffectTypeEnum.Eureka:
            //        CardGameManager.Instance.Eureka();
            //        break;
            //    case EffectTypeEnum.Attack:
            //        CardGameManager.Instance.BeginSelectingTarget(parentCard);
            //        break;
            //}

            OnUseSkill?.Invoke(this, EventArgs.Empty);
        }
    }
    public void SetSkillButtonAction(ExpertAction action)
    {
        this.action = action;
    }
    public void SetParentCard(BaseCard card)
    {
        parentCard = card;
    }
    public void Show()
    {
        //gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
