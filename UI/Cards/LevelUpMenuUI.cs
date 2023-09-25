using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpMenuUI : MonoBehaviour
{
    [SerializeField] private ExpertCard card;
    [SerializeField] private Button levelUpButton;
    [SerializeField] private TextMeshProUGUI levelDownButtonText;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private TextMeshProUGUI nextLevelText;
    [SerializeField] private TextMeshProUGUI currentLevelHealth;
    [SerializeField] private TextMeshProUGUI nextLevelHealth;
    [SerializeField] private TextMeshProUGUI currentTolerance;
    [SerializeField] private TextMeshProUGUI nextTolerance;
    [SerializeField] private TextMeshProUGUI currentSkillExperienceGain;
    [SerializeField] private TextMeshProUGUI nextSkillExperienceGain;
    [SerializeField] private TextMeshProUGUI currentSkillEffectPower;
    [SerializeField] private TextMeshProUGUI nextSkillEffectPower;
    [SerializeField] private TextMeshProUGUI currentSkillCostText;
    [SerializeField] private TextMeshProUGUI nextSkillCostText;
    [SerializeField] private TextMeshProUGUI levelUpCostText;
    [SerializeField] private TextMeshProUGUI currentLevelEnergyGenerationText;
    [SerializeField] private TextMeshProUGUI nextLevelEnergyGenerationText;
    [SerializeField] private Image currentLevelEnergyBackground;
    [SerializeField] private Image currentLevelEnergySymbol;
    [SerializeField] private Image nextLevelEnergyBackground;
    [SerializeField] private Image nextLevelEnergySymbol;
    [SerializeField] private Image currentSkillEffectBackground;
    [SerializeField] private Image currentSkillEffectSymbol;
    [SerializeField] private Image currentSkillCostBackground;
    [SerializeField] private Image currentSkillCostSymbol;
    [SerializeField] private Image nextSkillEffectBackground;
    [SerializeField] private Image nextSkillEffectSymbol;
    [SerializeField] private Image nextSkillCostBackground;
    [SerializeField] private Image nextSkillCostSymbol;
    [SerializeField] private Image levelUpCostBackground;
    [SerializeField] private Image levelUpCostSymbol;

    private void Start()
    {
        CardUI.OnAnyPlayCard += CardUI_OnAnyPlayCard;
        //levelUpButton.onClick.AddListener(() => { card.LevelUp(); });
        //UpdateLevelUpMenuUI();
        DisableLevelUpButton();
        Hide();

    }

    //private void Update()
    //{
    //    if (card.CanLevelUp())
    //    {
    //        EnableLevelUpButton();
    //    }
    //    else
    //    {
    //        if (gameObject.activeSelf == true && levelUpButton.isActiveAndEnabled == true)
    //        {
    //            DisableLevelUpButton();
    //        }
    //    }
    //}
   
    

    private void CardUI_OnAnyPlayCard(object sender, CardUI.OnPlayCardEventArgs e)
    {
       
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    //public void UpdateLevelUpMenuUI()
    //{
    //    bool hasNextLevel = card.GetCardSO().expertStatsObjectFromJson.levels.Count >= card.GetLevel() + 1;

    //    currentLevelText.text = card.GetLevel().ToString();
    //    nextLevelText.text = hasNextLevel ? (card.GetLevel() + 1).ToString() : "-";
    //    currentLevelHealth.text = card.GetHealth() + "/" + card.GetCardSO().expertStatsObjectFromJson.levels[card.GetLevel() - 1].maxHealth;
    //    nextLevelHealth.text = hasNextLevel ? (card.GetHealth() + card.GetCardSO().expertStatsObjectFromJson.levels[card.GetLevel()].maxHealth - card.GetCardSO().expertStatsObjectFromJson.levels[card.GetLevel() - 1].maxHealth) + "/" + card.GetCardSO().expertStatsObjectFromJson.levels[card.GetLevel()].maxHealth : "-";
    //    currentTolerance.text = card.GetCardSO().expertStatsObjectFromJson.levels[card.GetLevel() - 1].tolerance.ToString();
    //    nextTolerance.text = hasNextLevel ? card.GetCardSO().expertStatsObjectFromJson.levels[card.GetLevel()].tolerance.ToString() : "-";
    //    currentSkillExperienceGain.text = card.GetCardSO().expertStatsObjectFromJson.levels[card.GetLevel() - 1].expertActions[0].experienceGain.ToString();
    //    nextSkillExperienceGain.text = hasNextLevel ? card.GetCardSO().expertStatsObjectFromJson.levels[card.GetLevel()].expertActions[0].experienceGain.ToString() : "-";
        
        

    //    ActionEffect currentEffect = card.GetCardSO().expertStatsObjectFromJson.levels[((ExpertCard)card).GetLevel() - 1].expertActions[0].effect;
        
    //    if(currentEffect.effectType != EffectTypeEnum.Null)
    //    {
    //        currentSkillEffectSymbol.sprite = Image2SpriteUtility.Instance.GetSkillEffectIconSprite(currentEffect.effectType);
    //        currentSkillEffectPower.text = card.GetCardSO().expertStatsObjectFromJson.levels[card.GetLevel() - 1].expertActions[0].effect.power.ToString();
    //    }
    //    else
    //    {
    //        currentSkillEffectPower.text = "-";
    //        currentSkillEffectSymbol.sprite = null;
    //        currentSkillEffectBackground.color = Color.clear;
    //    }

        
    //    if (hasNextLevel) {
    //        ActionEffect nextEffect = card.GetCardSO().expertStatsObjectFromJson.levels[((ExpertCard)card).GetLevel()].expertActions[0].effect;
    //        if (nextEffect.effectType != EffectTypeEnum.Null)
    //        {
    //            nextSkillEffectSymbol.sprite = hasNextLevel ? Image2SpriteUtility.Instance.GetSkillEffectIconSprite(nextEffect.effectType) : null;
    //            nextSkillEffectPower.text = hasNextLevel ? card.GetCardSO().expertStatsObjectFromJson.levels[card.GetLevel()].expertActions[0].effect.power.ToString() : "-";
    //        }
    //        else
    //        {
    //            nextSkillEffectSymbol.sprite = null;
    //            nextSkillEffectPower.text = "-";
    //            nextSkillEffectBackground.color = Color.clear;
    //        }
    //    }
    //    else
    //    {
    //        nextSkillEffectSymbol.sprite = null;
    //        nextSkillEffectPower.text = "-";
    //        nextSkillEffectBackground.color = Color.clear;
    //    }
        
        


    //    TalentValues currentLevelEnergyGeneration = card.GetCardSO().expertStatsObjectFromJson.levels[card.GetLevel() - 1].energyGeneration;
    //    TalentValues nextLevelEnergyGeneration = hasNextLevel ? card.GetCardSO().expertStatsObjectFromJson.levels[card.GetLevel()].energyGeneration : null;
    //    TalentValues currentLevelActionCost = card.GetCardSO().expertStatsObjectFromJson.levels[card.GetLevel() - 1].expertActions[0].cost;
    //    TalentValues nextLevelActionCost = hasNextLevel ? card.GetCardSO().expertStatsObjectFromJson.levels[card.GetLevel()].expertActions[0].cost : null;
    //    TalentValues levelUpCost = hasNextLevel ? card.GetCardSO().expertStatsObjectFromJson.levels[card.GetLevel() - 1].levelUpCost : null;

    //    CardUI.SetCircleVisual(currentLevelEnergyGeneration, currentLevelEnergyGenerationText, currentLevelEnergySymbol, currentLevelEnergyBackground);
    //    //if (currentLevelEnergyGeneration.art != 0)
    //    //{
    //    //    currentLevelEnergyBackground.color = UniversalConstants.GetYellow();
    //    //    currentLevelEnergySymbol.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Art);
    //    //    currentLevelEnergyGenerationText.text = currentLevelEnergyGeneration.art.ToString();
    //    //}
    //    //else if (currentLevelEnergyGeneration.brawn != 0)
    //    //{
    //    //    currentLevelEnergyBackground.color = UniversalConstants.GetRed();
    //    //    currentLevelEnergySymbol.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Brawn);
    //    //    currentLevelEnergyGenerationText.text = currentLevelEnergyGeneration.brawn.ToString();
    //    //}
    //    //else if (currentLevelEnergyGeneration.philosophy != 0)
    //    //{
    //    //    currentLevelEnergyBackground.color = UniversalConstants.GetBlue();
    //    //    currentLevelEnergySymbol.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Philosophy);
    //    //    currentLevelEnergyGenerationText.text = currentLevelEnergyGeneration.philosophy.ToString();
    //    //}
    //    //else if (currentLevelEnergyGeneration.science != 0)
    //    //{
    //    //    currentLevelEnergyBackground.color = UniversalConstants.GetGreen();
    //    //    currentLevelEnergySymbol.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Science);
    //    //    currentLevelEnergyGenerationText.text = currentLevelEnergyGeneration.science.ToString();
    //    //}
    //    //////////////////////

    //    if (hasNextLevel)
    //    {
    //        CardUI.SetCircleVisual(nextLevelEnergyGeneration, nextLevelEnergyGenerationText, nextLevelEnergySymbol, nextLevelEnergyBackground);
    //        //if (nextLevelEnergyGeneration.art != 0)
    //        //{
    //        //    nextLevelEnergyBackground.color = UniversalConstants.GetYellow();
    //        //    nextLevelEnergySymbol.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Art);
    //        //    nextLevelEnergyGenerationText.text = nextLevelEnergyGeneration.art.ToString();
    //        //}
    //        //else if (nextLevelEnergyGeneration.brawn != 0)
    //        //{
    //        //    nextLevelEnergyBackground.color = UniversalConstants.GetRed();
    //        //    nextLevelEnergySymbol.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Brawn);
    //        //    nextLevelEnergyGenerationText.text = nextLevelEnergyGeneration.brawn.ToString();
    //        //}
    //        //else if (nextLevelEnergyGeneration.philosophy != 0)
    //        //{
    //        //    nextLevelEnergyBackground.color = UniversalConstants.GetBlue();
    //        //    nextLevelEnergySymbol.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Philosophy);
    //        //    nextLevelEnergyGenerationText.text = nextLevelEnergyGeneration.philosophy.ToString();
    //        //}
    //        //else if (nextLevelEnergyGeneration.science != 0)
    //        //{
    //        //    nextLevelEnergyBackground.color = UniversalConstants.GetGreen();
    //        //    nextLevelEnergySymbol.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Science);
    //        //    nextLevelEnergyGenerationText.text = nextLevelEnergyGeneration.science.ToString();
    //        //}
    //    }
    //    else
    //    {
    //        nextLevelEnergyBackground.color = Color.clear;
    //        nextLevelEnergySymbol.sprite = null;
    //        nextLevelEnergyGenerationText.text = "-";
    //        nextLevelEnergyGenerationText.color = Color.white;
    //    }
    //    //////////////////////////
    //    TalentValues currentSkillCost = card.GetCardSO().expertStatsObjectFromJson.levels[card.GetLevel() - 1].expertActions[0].cost;
    //    CardUI.SetCircleVisual(currentSkillCost, currentSkillCostText, currentSkillCostSymbol, currentSkillCostBackground);
    //    //if (currentLevelActionCost.art != 0)
    //    //{
    //    //    currentSkillCostBackground.color = UniversalConstants.GetYellow();
    //    //    currentSkillCostSymbol.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Art);
    //    //    currentSkillCostText.text = currentSkillCost.art.ToString();

    //    //}
    //    //else if (currentLevelActionCost.brawn != 0)
    //    //{
    //    //    currentSkillCostBackground.color = UniversalConstants.GetRed();
    //    //    currentSkillCostSymbol.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Brawn);
    //    //    currentSkillCostText.text =  currentSkillCost.brawn.ToString();
    //    //}
    //    //else if (currentLevelActionCost.philosophy != 0)
    //    //{
    //    //    currentSkillCostBackground.color = UniversalConstants.GetBlue();
    //    //    currentSkillCostSymbol.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Philosophy);
    //    //    currentSkillCostText.text =  currentSkillCost.philosophy.ToString();
    //    //}
    //    //else if (currentLevelActionCost.science != 0)
    //    //{
    //    //    currentSkillCostBackground.color = UniversalConstants.GetGreen();
    //    //    currentSkillCostSymbol.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Science);
    //    //    currentSkillCostText.text =  currentSkillCost.science.ToString();
    //    //}
    //    ///////////////
    //    if (hasNextLevel)
    //    {
    //        CardUI.SetCircleVisual(card.GetCardSO().expertStatsObjectFromJson.levels[card.GetLevel()].expertActions[0].cost, nextSkillCostText, nextSkillCostSymbol, nextSkillCostBackground);
    //        //TalentValues nextSkillCost = card.GetCardSO().expertStatsObjectFromJson.levels[card.GetLevel()].expertActions[0].cost;
    //        //if (nextLevelActionCost.art != 0)
    //        //{
    //        //    nextSkillCostBackground.color = UniversalConstants.GetYellow();
    //        //    nextSkillCostSymbol.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Art);
    //        //    nextSkillCostText.text =  nextSkillCost.art.ToString();
    //        //}
    //        //else if (nextLevelActionCost.brawn != 0)
    //        //{
    //        //    nextSkillCostBackground.color = UniversalConstants.GetRed();
    //        //    nextSkillCostSymbol.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Brawn);
    //        //    nextSkillCostText.text =  nextSkillCost.brawn.ToString();
    //        //}
    //        //else if (nextLevelActionCost.philosophy != 0)
    //        //{
    //        //    nextSkillCostBackground.color = UniversalConstants.GetBlue();
    //        //    nextSkillCostSymbol.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Philosophy);
    //        //    nextSkillCostText.text =  nextSkillCost.philosophy.ToString();
    //        //}
    //        //else if (nextLevelActionCost.science != 0)
    //        //{
    //        //    nextSkillCostBackground.color = UniversalConstants.GetGreen();
    //        //    nextSkillCostSymbol.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Science);
    //        //    nextSkillCostText.text =  nextSkillCost.science.ToString();
    //        //}
    //    }
    //    else
    //    {
    //        nextSkillCostBackground.color = Color.clear;
    //        nextSkillCostSymbol.sprite = null;
    //        nextSkillCostText.text = "-";
    //        nextSkillCostText.color = Color.white;
    //    }
    //    //////////////
    //    if (hasNextLevel)
    //    {
            
    //        TalentValues levelUpClassValuesCost = card.GetCardSO().expertStatsObjectFromJson.levels[card.GetLevel() - 1].levelUpCost;
    //        CardUI.SetCircleVisual(levelUpClassValuesCost, levelUpCostText, levelUpCostSymbol, levelUpCostBackground);
    //        //if (levelUpCost.art != 0)
    //        //{
    //        //    levelUpCostBackground.color = UniversalConstants.GetYellow();
    //        //    levelUpCostSymbol.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Art);
    //        //    levelUpCostText.text = levelUpClassValuesCost.art.ToString();
    //        //}
    //        //else if (levelUpCost.brawn != 0)
    //        //{
    //        //    levelUpCostBackground.color = UniversalConstants.GetRed();
    //        //    levelUpCostSymbol.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Brawn);
    //        //    levelUpCostText.text = levelUpClassValuesCost.brawn.ToString();
    //        //}
    //        //else if (levelUpCost.philosophy != 0)
    //        //{
    //        //    levelUpCostBackground.color = UniversalConstants.GetBlue();
    //        //    levelUpCostSymbol.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Philosophy);
    //        //    levelUpCostText.text = levelUpClassValuesCost.philosophy.ToString();
    //        //}
    //        //else if (levelUpCost.science != 0)
    //        //{
    //        //    levelUpCostBackground.color = UniversalConstants.GetGreen();
    //        //    levelUpCostSymbol.sprite = Image2SpriteUtility.Instance.GetStatIconSprite(TalentEnum.Science);
    //        //    levelUpCostText.text = levelUpClassValuesCost.science.ToString();
    //        //}
    //    }
    //    else
    //    {
    //        levelUpCostBackground.color = Color.clear;
    //        levelUpCostSymbol.sprite = null;
    //        levelUpCostText.text = "-";
    //        levelUpCostText.color = Color.white;
    //    }
    //    ////////////
    //    ///
    //    if (hasNextLevel)
    //    {

    //    }
    //    else
    //    {
    //        nextSkillEffectBackground.color = Color.clear;
    //    }
    //}
    private void DisableLevelUpButton()
    {
        levelUpButton.interactable = false;
        levelDownButtonText.alpha = .5f;
        
    }
    private void EnableLevelUpButton()
    {
        levelUpButton.interactable = true;
        levelDownButtonText.alpha = 1.0f;
    }
}
