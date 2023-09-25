using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoreSkillInfoUI : MonoBehaviour
{
    [SerializeField] private BaseCard card;

    [SerializeField] private TextMeshProUGUI skillTitleText;
    [SerializeField] private TextMeshProUGUI skillDescriptionText;
    [SerializeField] private TextMeshProUGUI expGainText;
    [SerializeField] private TextMeshProUGUI viableTargetsText;
    [SerializeField] private TextMeshProUGUI targetText;

    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private Image powerBackground;
    [SerializeField] private Image powerIcon;

    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image costBackground;
    [SerializeField] private Image costIcon;

    [SerializeField] private Button retractButton;
    [SerializeField] private Button useSkillButton;
    [SerializeField] private SkillButton retractedSkillUI;

    private void Awake()
    {
        retractButton.onClick.AddListener(() =>
        {
            Hide();
            retractedSkillUI.Show();
        });
        useSkillButton.onClick.AddListener(() => { 
            retractedSkillUI.UseSkill(); 
        });
        ((ExpertCard)card).OnLevelUp += ExpertCard_OnLevelUp;

    }

    private void ExpertCard_OnLevelUp(object sender, System.EventArgs e)
    {
        Refresh();
    }

    //private void Update()
    //{
       
    //    if (useSkillButton.interactable == false && retractedSkillUI.CanUseSkill())
    //    {
    //        useSkillButton.interactable = true;
    //    }
    //    else if (useSkillButton.interactable == true && !retractedSkillUI.CanUseSkill())
    //    {
    //        useSkillButton.interactable = false;
    //    }

    //}
    private void Start()
    {
        Refresh();
    }
    private void Refresh()
    {
        //ExpertAction skill = card.GetCardSO().expertStatsObjectFromJson.levels[((ExpertCard)card).GetLevel() - 1].expertActions[0];
        //skillTitleText.text = skill.actionName.ToString();

        //switch (skill.effect.effectType)
        //{
        //    case EffectTypeEnum.BonusEnergy:
        //        skillDescriptionText.text = UniversalConstants.BONUS_ENERGY_DESCRIPTION;
        //        break;
        //    case EffectTypeEnum.GainExperience:
        //        skillDescriptionText.text = UniversalConstants.GAIN_EXPERIENCE_DESCRIPTION;
        //        break;
        //    case EffectTypeEnum.Attack:
        //        skillDescriptionText.text = UniversalConstants.ATTACK_DESCRIPTION;
        //        break;
        //    case EffectTypeEnum.Eureka:
        //        skillDescriptionText.text = UniversalConstants.EUREKA_DESCRIPTION;
        //        break;
        //    default:
        //        //ugly use of string
        //        skillDescriptionText.text = "\nNo effect other than to gain experience.";
        //        break;
        //}

        //expGainText.text = "+" + skill.experienceGain.ToString();

        //if (skill.viableTarget.selfOther == SelfOtherEnum.Self)
        //{
        //    viableTargetsText.text = "Self";
        //}
        //else
        //{

        //    //Define string portion that denotes viable target classes
        //    bool firstAddition = true;
        //    string talentClasses = "";

        //    if (skill.viableTarget.cardTalentClasses.Count == 4)
        //    {
        //        //has all classes
        //        talentClasses = "All ";
        //    }
        //    else
        //    {
        //        foreach (TalentEnum talentClass in skill.viableTarget.cardTalentClasses)
        //        {
        //            if (firstAddition)
        //            {
        //                talentClasses += talentClass;
        //                firstAddition = false;
        //            }
        //            else
        //            {
        //                talentClasses += " or " + talentClass;
        //            }
        //        }
        //        if (skill.viableTarget.cardTalentClasses.Count > 0)
        //        {
        //            talentClasses += ", ";
        //        }
        //    }

        //    //Define string portion that denotes viables card types
        //    firstAddition = true;
        //    string cardTypes = "";
        //    if (skill.viableTarget.cardTypes.Count == 3)
        //    {
        //        //has all classes
        //        cardTypes = "";
        //    }
        //    else
        //    {
        //        foreach (CardTypeEnum cardType in skill.viableTarget.cardTypes)
        //        {
        //            if (firstAddition)
        //            {
        //                cardTypes += cardType;
        //                firstAddition = false;
        //            }
        //            else
        //            {
        //                cardTypes += " or " + cardType;
        //            }
        //        }
        //        if (skill.viableTarget.cardTypes.Count > 0)
        //        {
        //            cardTypes += " ";
        //        }
        //    }

        //    //Define string portion that denotes viable areas
        //    firstAddition = true;
        //    string cardAreas = "";
        //    if (skill.viableTarget.locations.Count == 4)
        //    {
        //        //has all classes
        //        cardAreas = "Anywhere";
        //    }
        //    else
        //    {
        //        foreach (GameAreaEnum location in skill.viableTarget.locations)
        //        {
        //            if (firstAddition)
        //            {
        //                cardAreas += LocationToWord(location);
        //            }
        //            else
        //            {
        //                cardAreas += " or " + LocationToWord(location);
        //            }
        //        }
        //    }

        //    //Exclude self or not
        //    string excludeSelf = "";

        //    if (skill.viableTarget.selfOther == SelfOtherEnum.Other
        //            && skill.viableTarget.locations.Contains(GameAreaEnum.Field))
        //    {
        //        excludeSelf = " But Self";
        //    }
        //    string gainExperience = "";
        //    if (skill.effect.effectType == EffectTypeEnum.GainExperience)
        //    {
        //        gainExperience += "unfilled EXP ";
        //    }

        //    viableTargetsText.text = talentClasses + gainExperience + cardTypes + cardAreas + excludeSelf;
        //}
        //if (skill.viableTarget.selfOther == SelfOtherEnum.Self)
        //{
        //    targetText.text = "Self";
        //}
        //else
        //{
        //    targetText.text = "Single\n" + skill.effect.preference.ToString();
        //}
        //if (skill.effect.power != 0)
        //{
        //    string powerString = skill.effect.power.ToString();
        //    if (powerString.Length == 2)
        //    {
        //        powerText.fontSize = 11;
        //        powerText.characterSpacing = -3.7f;
        //    }
        //    else if (powerString.Length == 1)
        //    {
        //        powerText.fontSize = 13;
        //    }
        //    powerText.text = powerString;
        //    //ugly (magi number)


        //}
        //else
        //{
        //    powerText.text = "-";
        //    powerBackground.gameObject.SetActive(false);
        //}


        //powerIcon.sprite = Image2SpriteUtility.Instance.GetSkillEffectIconSprite(skill.effect.effectType);

        //if (skill.cost.art != 0)
        //{
        //    costText.text = skill.cost.art.ToString();
        //    costBackground.color = UniversalConstants.GetYellow();
        //    costIcon.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Art);
        //}
        //else if (skill.cost.brawn != 0)
        //{
        //    costText.text = skill.cost.brawn.ToString();
        //    costBackground.color = UniversalConstants.GetRed();
        //    costIcon.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Brawn);
        //}
        //else if (skill.cost.philosophy != 0)
        //{
        //    costText.text = skill.cost.philosophy.ToString();
        //    costBackground.color = UniversalConstants.GetBlue();
        //    costIcon.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Philosophy);
        //}
        //else if (skill.cost.science != 0)
        //{
        //    costText.text = skill.cost.science.ToString();
        //    costBackground.color = UniversalConstants.GetGreen();
        //    costIcon.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Science);
        //}
    }
    private  string LocationToWord(GameAreaEnum location)
    {
        switch (location)
        {
            case GameAreaEnum.Field:
                return "Ally";
            case GameAreaEnum.OpponentField:
                return "Enemy";
            case GameAreaEnum.Hand:
                return "In My Hand";
            default: return "";
        }
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
