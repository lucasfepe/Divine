using Amazon.DynamoDBv2.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ExpertCardUI : CardUI
{
    [SerializeField] private Image lifeTimeBar;
    [SerializeField] private Image redGiantIconImage;
    [SerializeField] private TextMeshProUGUI lightText;
    [SerializeField] private TextMeshProUGUI lifetimeBarText;
    [SerializeField] private RectTransform lightRectTransform;
    [SerializeField] private RectTransform lifetime;
    [SerializeField] private Animator lifetimeAnimator;
    [SerializeField] private Animator stardustAnimator;
    [SerializeField] private Animator lightAnimator;
    [SerializeField] private RectTransform stardustRectTransform;
    [SerializeField] private TextMeshProUGUI stardustText;
    [SerializeField] private Image shedIcon;
    [SerializeField] private CostContainer costContainer;
    [SerializeField] private PowerContainer powerContainer;

    [SerializeField] private TextMeshProUGUI shedText;
    [SerializeField] private Image skillIcon;
    [SerializeField] private BoxCollider2D cardButtonCollider;
    [SerializeField] private CardBorderAnimation shedEffect;
    [SerializeField] private CardGraphicAnimator graphicAnimator;   
    [SerializeField] private TextMeshProUGUI skillDescText;
    [SerializeField] private Animator highlightAnimator;
    [SerializeField] private DesintegrateAnimation disintegrateAnimation;
    [SerializeField] private GameObject dust;

    public CardGraphicAnimator GetCardGraphicAnimator()
    {
        return graphicAnimator;
    }

    private const string OVERLAY_IMAGE_NAME = "overlayWhiteBorderRoom";
    private const string OVERLAY_NO_PANEL_IMAGE_NAME = "overlayWhiteNoPanelBorderRoom";
    private const string CARD_FRONT_IMAGE_NAME = "frontBorderRoom";
    private const string CARD_FRONT_NO_PANEL_IMAGE_NAME = "frontNoPanelBorderRoom";
    private const float HAND_METRIC_SCALE = 1.5f;
    private const float HAND_FONT_SIZE = 60;
    private const float HAND_TOLERANCE_FONT_SIZE = 35;
    private const float HAND_HEALTH_FONT_SIZE = 45;
    private const float HAND_LONG_LIFETIME_FONT_SIZE = 50;
    private const float FIELD_METRIC_SCALE = 1.3f;
    private const float FIELD_FONT_SIZE = 30;
    private const float INSPECT_METRIC_SCALE = 1f;
    private const float INSPECT_FONT_SIZE = 50;

    private RectTransform skillBtnRectTransform;
    private MoreSkillInfoButtonUI moreSkillInfoButtonUI;
    private bool refreshingLifetime = false;
    private float targetLifetimeFillAmount = 0f;
    private float lifetimeFillSpeed = 2f;
    private SkillDescScriptUI skillDescUI;
    private ChangeTextGradually stardustTextGradualChange;
    private ChangeTextGradually lightTextGradualChange;
    private const string ROTATE_TRIGGER = "RotateTrigger";
    private const string STARDUST_TEXT_GROW_TRIGGER = "StardustTextIncreaseTrigger";
    private const string STARDUST_TEXT_SHRINK_TRIGGER = "StardustTextShrinkTrigger";
    private const string LIGHT_TEXT_SHRINK_TRIGGER = "LightTextShrinkTrigger";
    private const string LIGHT_TEXT_GROW_TRIGGER = "LightTextGrowTrigger";
    private const string SHED_ICON_IMAGE_NAME = "RedGiantIcon";
    private const string ABSORB_ICON_IMAGE_NAME = "AbsorbIcon";
    private const string NOBLACKHOLE_ICON_IMAGE_NAME = "NoBlackHoleIcon";
    private const string EXPENSIVE_ICON_IMAGE_NAME = "expensive";
    private const string LIGHT_ICON_IMAGE_NAME = "light";
    private const string GEMINI_ICON_IMAGE_NAME = "GeminiIcon";
    private const string INJURE_ICON_IMAGE_NAME = "explosion";
    private const string STRANGE_ICON_IMAGE_NAME = "StrangeMatterIcon";
    private const string UPHOLD_ICON_IMAGE_NAME = "Uphold";
    private const string DIM_ICON_IMAGE_NAME = "Dim";
    private const string IMMUNE_ICON_IMAGE_NAME = "Immune";
    private const string DISCOUNT_ICON_IMAGE_NAME = "Cheapen";
    private const string END_ICON_IMAGE_NAME = "END";
    
    private bool hasSkillDesc = false;
    private CardBorderAnimation cardBorderAnimation;
    public event EventHandler OnEndAnimation;
    private Image costContainerImage;
    private TextMeshProUGUI costContainerText;
    private Image powerContainerImage;
    private TextMeshProUGUI powerContainerText;
    override protected void Awake()
    {
        base.Awake();
        if (redGiantIconImage != null) { 
        redGiantIconImage.gameObject.SetActive(false);
        }
        stardustTextGradualChange = stardustText.GetComponent<ChangeTextGradually>();
        stardustTextGradualChange.OnUpdateStatTextGradually += StardustTextGradualChange_OnUpdateStatTextGradually;
        lightTextGradualChange = lightText.GetComponent<ChangeTextGradually>();
         skillDescUI = skillIcon.GetComponent<SkillDescScriptUI>();
        HideSkillDescUI();
        cardBorderAnimation = background.GetComponent<CardBorderAnimation>();
        if(card is BaseCard)
        {
            ((BaseCard)card).OnCardDieLight += card_OnCardDieLight;
            ((BaseCard)card).OnCardDieStardust += card_OnCardDieStardust;
        }
        costContainerImage = costContainer.GetComponentInChildren<Image>();
        costContainerText = costContainer.GetComponentInChildren <TextMeshProUGUI>();
        powerContainerImage
            = powerContainer.GetComponentInChildren<Image>();
        powerContainerText = powerContainer.GetComponentInChildren<TextMeshProUGUI>();
        
    }

    private void ShedEffect_OnCardShedAnimationDone(object sender, EventArgs e)
    {
        TurnPhaseStateMachine.Instance.OnShedDone();
    }

    private void card_OnCardDieStardust(object sender, EventArgs e)
    {
        StardustDanger();
    }

    private void card_OnCardDieLight(object sender, EventArgs e)
    {
        LightDanger();
    }

    private void StardustTextGradualChange_OnUpdateStatTextGradually(object sender, EventArgs e)
    {
        //start whitedwarf animation
        if(card is ExpertCard)
        ((ExpertCard)card).CheckMorphStardust();
    }

    //ugly when refactoring try to not use if to check a type of object the object type itself should be used based on inheritance/ interface
    private void Start()
    {
        if (card is ExpertCard) {
            ((ExpertCard)card).OnDetermineIfCanUseSkill += ExpertCard_OnDetermineIfCanUseSkill;
            shedEffect.OnCardShedAnimationDone += ShedEffect_OnCardShedAnimationDone;
            //ugly but only way I can do this
            
            lifetimeAnimator.GetBehaviour<OnAnimationEnd>().OnAnimationEndEvent += LifetimeTextFlipAnimation_OnAnimationEndEvent;
            ((BaseCard)card).CheckIfCanUseSkill();
        }
        
    }

    private void OverallAnimator_OnCardDisintegrated(object sender, EventArgs e)
    {
        //ugly shouldn't be here
        ((ExpertCard)card).Die();
    }

    public override void Disintegrate(float d)
    {
        disintegrateAnimation.Disintegrate(d);
    }
    private void OnDestroy()
    {
        stardustTextGradualChange.OnUpdateStatTextGradually -= StardustTextGradualChange_OnUpdateStatTextGradually;
        if (card is ExpertCard)
        {
            ((ExpertCard)card).OnDetermineIfCanUseSkill -= ExpertCard_OnDetermineIfCanUseSkill;
        }
    }
    private void ExpertCard_OnDetermineIfCanUseSkill(object sender, EventArgs e)
    {
        if (((ExpertCard)card).CanUseSkill() && !((BaseCard)card).IsCardDestroyed())
        {
            Flash();


        }
        else
        {
            DoneFlash();


        }
    }
    public void Flash()
    {
        if (!card.IsCardDestroyed()) {
            highlightAnimator.ResetTrigger("Hide");
        highlightAnimator.SetBool("canUseSkill", true);
        }
    }
    public void DoneFlash()
    {
        if (!card.IsCardDestroyed()) { 
            highlightAnimator.SetBool("canUseSkill", false);
        }
    }
    
    public void LastingHighlight()
    {
        ((BaseCard)card).GetHighlight().LastingHighlight();
    }
    public void EndLastingHighlight()
    {
        ((BaseCard)card).GetHighlight().EndLastingHighlight();
    }
    public void HideSkillDescUI()
    {
        if(card is ExpertCard && !((ExpertCard)card).IsCardDestroyed()) {
            skillDescUI.Hide();
        }
        else if(!(card is ExpertCard))
        {
            skillDescUI.Hide();
        }
        
    }

    override protected void Card_OnCardSOAssigned(object sender, EventArgs e)
    {
        if (card.IsCardDestroyed()) return;
        base.Card_OnCardSOAssigned(sender,e);
        SetText(lifetimeBarText, card.GetLifetime().ToString());
        SetText(lightText, card.GetCardLight().ToString());
        SetText(stardustText, card.GetCardStardust().ToString());
        
        if(card.GetCardSO().Shed != 0)
        {
            shedText.text = card.GetCardSO().Shed.ToString();
            shedIcon.sprite = Resources.Load<Sprite>(SHED_ICON_IMAGE_NAME);
            shedIcon.color = Color.white;
        }
        
        if (card.GetCardSO().GiantEffect!= null
            &&
            card.GetCardSO().GiantEffect.Effect != EffectTypeEnum.Null)
        {
            
            hasSkillDesc = true;
            switch (card.GetCardSO().GiantEffect.Effect)
            {
                case EffectTypeEnum.Absorb:
                    skillIcon.sprite = Resources.Load<Sprite>(ABSORB_ICON_IMAGE_NAME);break;
                case EffectTypeEnum.NoBlackHole:
                    skillIcon.sprite = Resources.Load<Sprite>(NOBLACKHOLE_ICON_IMAGE_NAME);break;
                case EffectTypeEnum.Expensive:
                    skillIcon.sprite = Resources.Load<Sprite>(EXPENSIVE_ICON_IMAGE_NAME); break;
                case EffectTypeEnum.GenerateLight:
                    skillIcon.sprite = Resources.Load<Sprite>(LIGHT_ICON_IMAGE_NAME); break;
                case EffectTypeEnum.Gemini:
                    skillIcon.sprite = Resources.Load<Sprite>(GEMINI_ICON_IMAGE_NAME); break;
                case EffectTypeEnum.Injure:
                    skillIcon.sprite = Resources.Load<Sprite>(INJURE_ICON_IMAGE_NAME); break;
                case EffectTypeEnum.Strange:
                    skillIcon.sprite = Resources.Load<Sprite>(STRANGE_ICON_IMAGE_NAME); break;
                case EffectTypeEnum.Uphold:
                    skillIcon.sprite = Resources.Load<Sprite>(UPHOLD_ICON_IMAGE_NAME); break;
                case EffectTypeEnum.Dim:
                    skillIcon.sprite = Resources.Load<Sprite>(DIM_ICON_IMAGE_NAME); break;
                case EffectTypeEnum.Untargetable:
                    skillIcon.sprite = Resources.Load<Sprite>(IMMUNE_ICON_IMAGE_NAME); break;
                case EffectTypeEnum.Discount:
                    skillIcon.sprite = Resources.Load<Sprite>(DISCOUNT_ICON_IMAGE_NAME); break;
                case EffectTypeEnum.WipeoutBothFields:
                    skillIcon.sprite = Resources.Load<Sprite>(END_ICON_IMAGE_NAME); break;

            }
            
            skillIcon.color = Color.white;
            skillDescText.text = card.GetCardSO().GiantEffect.Description;
        }
        
    }
    public bool HasSkill()
    {
        return hasSkillDesc;
    }
    public void SetHasSkill(bool hasSkill)
    {
        hasSkillDesc = hasSkill;
    }
    public void SetText(TextMeshProUGUI textObject, string text)
    {
        
        switch (card.GetCardGameArea())
        {
            case GameAreaEnum.Hand:
                textObject.text = text;
                if (text.Length > 2)
                {
                    textObject.fontSize = 45;
                    textObject.characterSpacing = -10;
                }
                else if (text.Length > 1)
                {
                    textObject.fontSize = 35;
                    textObject.characterSpacing = -10;
                }
                else if (text.Length == 1)
                {
                    textObject.fontSize = 50;
                    textObject.characterSpacing = 0;
                }
                break;
            case GameAreaEnum.OpponentField:
            case GameAreaEnum.Field:
                textObject.text = text;
                if (text.Length > 2)
                {
                    textObject.fontSize = 40;
                    textObject.characterSpacing = -10;
                }
                else if (text.Length > 1)
                {
                    textObject.fontSize = 45;
                    textObject.characterSpacing = -10;
                }
                else if (text.Length == 1)
                {
                    textObject.fontSize = 60;
                    textObject.characterSpacing = 0;
                }
                break;
            case GameAreaEnum.Inspect:
                textObject.text = text;
                if (text.Contains("\n"))
                {
                    textObject.fontSize = 18;
                    textObject.characterSpacing = -10;
                }
                else if (text.Length > 2)
                {
                    textObject.fontSize = 20;
                    textObject.characterSpacing = -10;
                }
                else if (text.Length > 1)
                {
                    textObject.fontSize = 25;
                    textObject.characterSpacing = -10;
                }
                else if (text.Length == 1)
                {
                    textObject.fontSize = 35;
                    textObject.characterSpacing = 0;
                }
                break;
        }

    }

    private void Update()
    {
        //if (refreshingLifetime)
        //{
        //    lifeTimeBar.fillAmount = Mathf.Lerp(lifeTimeBar.fillAmount, targetLifetimeFillAmount, Time.deltaTime * lifetimeFillSpeed);
        //    if(Math.Abs(lifeTimeBar.fillAmount - targetLifetimeFillAmount) < .001)
        //    {
        //        refreshingLifetime = false;
        //    }
        //}
    }

    private void SkillButton_OnUseSkill(object sender, System.EventArgs e)
    {
        //RefreshExperience();
    }

    //private void Update()
    //{
    //    if (((ExpertCard)card).CanLevelUp() && quickLevelUpButton.activeSelf == false)
    //    {
    //        EnablQuickLevelUpButton();
    //    }else if (!((ExpertCard)card).CanLevelUp() && quickLevelUpButton.activeSelf == true)
    //    {
    //        DisableQuickLevelUpButton();
    //    }
    //    if (skillButton.GetComponentInChildren<Button>().interactable == false && skillButton.CanUseSkill())
    //    {
    //        //ugly
    //        EnableSkill(skillButton.GetComponentInChildren<Button>());
    //    }
    //    else if (skillButton.GetComponentInChildren<Button>().interactable == true && !skillButton.CanUseSkill())
    //    {
    //        DisableSkill(skillButton.GetComponentInChildren<Button>());
    //    }

    //}
    private void ExpertCard_OnLevelUp(object sender, System.EventArgs e)
    {
        //TalentValues energyGeneration = GetExpertLevel().energyGeneration;
        
        //if (energyGeneration.art != 0)
        //{
        //    energyText.text = energyGeneration.art.ToString();
        //}
        //else if (energyGeneration.brawn != 0)
        //{
        //    energyText.text = energyGeneration.brawn.ToString();
        //}
        //else if (energyGeneration.philosophy != 0)
        //{
        //    energyText.text = energyGeneration.philosophy.ToString();
        //}
        //else if (energyGeneration.science != 0)
        //{
        //    energyText.text = energyGeneration.science.ToString();
        //}

        //RefreshExperience();
        //RefreshHealth();
        ////SetText(toleranceText,GetExpertLevel()
        ////    .tolerance
        ////    .ToString());
        
        //levelText.text = ((ExpertCard)card).GetLevel().ToString();
        
        ////update skill ui
        //RefreshSkills();
    }

    //private ExpertLevel GetExpertLevel()
    //{
    //return ((ExpertCard)card).GetExpertLevel();
    //}


    override public void BecomeRedGiant()
    {
        base.BecomeRedGiant();
        lifetimeBarText.text = string.Empty;
        // ugly who could have know that this would have cause such a problem
        //a method fires when the animation is over but if the active is false 
        //then the animation will never be over how could I have coded it to avoid this
        //lifetimeBarText.gameObject.SetActive(false);
        lifeTimeBar.gameObject.SetActive(false);
        redGiantIconImage.gameObject.SetActive(true);
        cardBorderAnimation.BecomeRedGiant();
        
        
    }
    public void RefreshLifetime()
    {
        int lifetime = ((ExpertCard)card).GetLifetime();

        //targetLifetimeFillAmount = (float)((ExpertCard)card).GetLifetime() / card.GetCardSO().Lifetime;
        
        SetText(lifetimeBarText, lifetime.ToString());
        
        lifetimeAnimator.SetTrigger(ROTATE_TRIGGER);
        //refreshingLifetime = true;



    }

    private void LifetimeTextFlipAnimation_OnAnimationEndEvent(object sender, EventArgs e)
    {
        TurnPhaseStateMachine.Instance.Aged();
    }

    public void RefreshLight()
    {
        int light = ((ExpertCard)card).GetCardLight();
        SetText(lightText, light.ToString());
    }
    public void RefreshStardust()
    {
        int stardust = ((ExpertCard)card).GetCardStardust();
        SetText(stardustText, stardust.ToString());
    }

    
    
    //Don't know if inheritance here is the best way to go
    

    override public void RefreshStardustText()
    {
        float stardustTextGrowAnimationDuration = 1f;
        stardustTextGradualChange.SetText(stardustTextGrowAnimationDuration, card.GetCardStardust());
        bool valueIncreased = (Int32.Parse(stardustText.text) - card.GetCardStardust()) < 0;
        bool valueDecreased = (Int32.Parse(stardustText.text) - card.GetCardStardust()) > 0;
        if (valueIncreased)
        {
            //only do text grow animation when value grows
            stardustAnimator.SetTrigger(STARDUST_TEXT_GROW_TRIGGER);
        }else if (valueDecreased)
        {
            stardustAnimator.SetTrigger(STARDUST_TEXT_SHRINK_TRIGGER);
        }
        
    }
    public void ShedUI()
    {
        if (((BaseCard)card).IsCardDestroyed()) return;
        shedEffect.Shed();
        if(((BaseCard)card).GetCardOwner() == Player.Instance.IAm())
        ((BaseCard)card).ShedServerRpc();
    }

    override public void RefreshLightText()
    {
        float lightTextRefreshAnimationDuration = 1f;
        lightTextGradualChange.SetText(lightTextRefreshAnimationDuration, card.GetCardLight());
        //lightAnimator.SetTrigger(STARDUST_TEXT_INCREASE_TRIGGER);
        bool valueIncreased = (Int32.Parse(lightText.text) - card.GetCardLight()) < 0;
        bool valueDecreased = (Int32.Parse(lightText.text) - card.GetCardLight()) > 0;
        if (valueIncreased)
        {
            //only do text grow animation when value grows
            lightAnimator.SetTrigger(LIGHT_TEXT_GROW_TRIGGER);
        }
        else if (valueDecreased)
        {
            lightAnimator.SetTrigger(LIGHT_TEXT_SHRINK_TRIGGER);
        }
    }

    override protected void CardUI_OnAnyPlayCard(object sender, OnPlayCardEventArgs e)
    {
        base.CardUI_OnAnyPlayCard(sender,e);
        if (e.card != card) return;
       
    }

   
   
    private void EnableSkill(Button skillButton)
    {
        skillButton.interactable = true;
    }
    private void DisableSkill(Button skillButton)
    {
        skillButton.interactable = false;
    }

    override public void AdaptLook()
    {
        base.AdaptLook();
        switch (card.GetCardGameArea())
        {
            case GameAreaEnum.Hand:
                AdaptLookForHand();
                break;
            case GameAreaEnum.OpponentField:
                AdaptLookForOpponentField();
                break;
            case GameAreaEnum.Field:
                AdaptLookForField();
                break;
            case GameAreaEnum.Inspect:
                AdaptLookForInspect();
                break;
            case GameAreaEnum.CardPreview:
                AdaptLookForInspect();
                break;
            case GameAreaEnum.Catalogue:
                AdaptLookForHand();
                break;
        }
    }
    
    private void AdaptLookForHand()
    {
        cardButtonCollider.enabled = true;
        background.sprite = Resources.Load<Sprite>(CARD_FRONT_IMAGE_NAME);
        overlay.sprite = Resources.Load<Sprite>(OVERLAY_IMAGE_NAME);
        lightRectTransform.anchorMin = new Vector2(0, 1);
        lightRectTransform.anchorMax = new Vector2(0, 1);
        lightRectTransform.anchoredPosition = new Vector2(0,-27);
        lightRectTransform.sizeDelta = new Vector2(50, 50);
        lightRectTransform.pivot = new Vector2(0, 1);
        SetText(lightText,lightText.text);
        shedIcon.gameObject.SetActive(false);
        shedText.gameObject.SetActive(false);
        skillIcon.gameObject.SetActive(false);
        costContainer.Hide();
        powerContainer.Hide();
        
        HideSkillDescUI();

        stardustRectTransform.anchorMin = new Vector2(0, 1);
        stardustRectTransform.anchorMax = new Vector2(0, 1);
        stardustRectTransform.anchoredPosition = new Vector2(0, -177);
        stardustRectTransform.sizeDelta = new Vector2(50, 50);
        stardustRectTransform.pivot = new Vector2(0, 1);
        SetText(stardustText, stardustText.text);

        lifetime.anchorMin = new Vector2(1, 1);
        lifetime.anchorMax = new Vector2(1, 1);
        lifetime.anchoredPosition = new Vector2(0, -25);
        lifetime.sizeDelta = new Vector2(50, 50);
        lifetime.pivot = new Vector2(1, 1);
        SetText(lifetimeBarText, lifetimeBarText.text);

        //health.anchorMin = new Vector2(0, 1);
        //health.anchorMax = new Vector2(0, 1);
        //health.anchoredPosition = new Vector2(0, -175);
        //health.sizeDelta = new Vector2(50, 50);
        //health.pivot = new Vector2(0, 1);
        //SetText(healthBarText, ((ExpertCard)card).GetHealth().ToString());

        //tolerance.anchorMin = new Vector2(1, 1);
        //tolerance.anchorMax = new Vector2(1, 1);
        //tolerance.anchoredPosition = new Vector2(0, -175);
        //tolerance.sizeDelta = new Vector2(50, 50);
        ////toleranceText.fontSize = 30;
        //tolerance.pivot = new Vector2(1, 1);
        //SetText(toleranceText, toleranceText.text);

        //showLevelUpMenu.gameObject.SetActive(false);
        
        //healthBarLine.gameObject.SetActive(false);

        //experience.gameObject.SetActive(false);
       
    }
    //private void AdaptLifetimeLookForHand()
    //{
    //    if (lifetimeBarText.text.Length > 1)
    //    {
    //        lifetimeBarText.characterSpacing = -16;
    //    }
    //    if (lifetimeBarText.text.Length > 1)
    //    {
    //        lifetimeBarText.fontSize = HAND_LONG_LIFETIME_FONT_SIZE;
    //    }
    //    else if (lifetimeBarText.text.Length == 1)
    //    {
    //        lifetimeBarText.fontSize = HAND_FONT_SIZE;
    //    }
    //}
    private void AdaptLookForInspect()
    {
        //level.gameObject.SetActive(false);
        //level.anchorMin = new Vector2(1, 1);
        //level.anchorMax = new Vector2(1, 1);
        //level.anchoredPosition = new Vector2(0, 0);
        //level.sizeDelta = new Vector2(40, 40);
        //level.pivot = new Vector2(1, 1);
        //SetText(levelText,levelText.text);
        cardButtonCollider.enabled = false;
        
        shedIcon.gameObject.SetActive(true);
        shedText.gameObject.SetActive(true);

        costContainer.ShouldShow();
        powerContainer.ShouldShow();
        

        if (HasSkill()) { 
        skillIcon.gameObject.SetActive(true);
        skillDescUI.Show();
        }
        //quickLevelUpButtonRectTransform.anchorMin = new Vector2(1, 1);
        //quickLevelUpButtonRectTransform.anchorMax = new Vector2(1, 1);
        //quickLevelUpButtonRectTransform.anchoredPosition = new Vector2(0, 0);
        //quickLevelUpButtonRectTransform.sizeDelta = new Vector2(40, 40);
        //quickLevelUpButtonRectTransform.pivot = new Vector2(1, 1);

        background.sprite = Resources.Load<Sprite>(CARD_FRONT_NO_PANEL_IMAGE_NAME);
        overlay.sprite = Resources.Load<Sprite>(OVERLAY_NO_PANEL_IMAGE_NAME);

        stardustRectTransform.anchorMin = new Vector2(0, 1);
        stardustRectTransform.anchorMax = new Vector2(0, 1);
        stardustRectTransform.anchoredPosition = new Vector2(0, -177);
        stardustRectTransform.sizeDelta = new Vector2(50, 50);
        stardustRectTransform.pivot = new Vector2(0, 1);
        SetText(stardustText, stardustText.text);

        lightRectTransform.anchorMin = new Vector2(0, 1);
        lightRectTransform.anchorMax = new Vector2(0, 1);
        lightRectTransform.anchoredPosition = new Vector2(0, -27);
        lightRectTransform.sizeDelta = new Vector2(50, 50);
        lightRectTransform.pivot = new Vector2(0, 1);
        SetText(lightText, lightText.text);

        lifetime.anchorMin = new Vector2(1, 1);
        lifetime.anchorMax = new Vector2(1, 1);
        lifetime.anchoredPosition = new Vector2(0, -25);
        lifetime.sizeDelta = new Vector2(50, 50);
        lifetime.pivot = new Vector2(1, 1);
        SetText(lifetimeBarText, lifetimeBarText.text);

        

        //healthBarLine.gameObject.SetActive(true);
        //health.anchorMin = new Vector2(1, 0);
        //health.anchorMax = new Vector2(1, 0);
        //health.anchoredPosition = new Vector2(-80, 0);
        //health.sizeDelta = new Vector2(40, 40);
        //health.pivot = new Vector2(1, 0);
        //SetText(healthBarText,((ExpertCard)card).GetHealth() + "\n" + ((ExpertCard)card).GetCardSO().expertStatsObjectFromJson.levels[((ExpertCard)card).GetLevel() - 1].maxHealth);


        //tolerance.anchorMin = new Vector2(1, 1);
        //tolerance.anchorMax = new Vector2(1, 1);
        //tolerance.anchoredPosition = new Vector2(0, -70);
        //tolerance.sizeDelta = new Vector2(30, 30);
        //SetText(toleranceText,toleranceText.text);
        //tolerance.pivot = new Vector2(1, 1);

        //showLevelUpMenu.gameObject.SetActive(false);
        //levelUpMenuButtonUI.anchorMin = new Vector2(1, 0);
        //levelUpMenuButtonUI.anchorMax = new Vector2(1, 0);
        //levelUpMenuButtonUI.anchoredPosition = new Vector2(0, 0);
        //levelUpMenuButtonUI.sizeDelta = new Vector2(40, 40);
        //levelUpMenuButtonUI.pivot = new Vector2(1, 0);

        //experienceBarLine.gameObject.SetActive(false);

        //experience.gameObject.SetActive(true);
        //experience.anchorMin = new Vector2(1, 0);
        //experience.anchorMax = new Vector2(1, 0);
        //experience.anchoredPosition = new Vector2(-40, 0);
        //experience.sizeDelta = new Vector2(40, 40);
        //experience.pivot = new Vector2(1, 0);
        //ugly method should be in expert card like experience to string fraction
        //if (((ExpertCard)card).GetLevel() >= ((ExpertCard)card).GetCardSO().expertStatsObjectFromJson.levels.Count)
        //{
        //    //reached max level
        //    SetText(experienceBarText, "");
        //    experienceBarLine.gameObject.SetActive(false);
        //}
        //else { 
        //string numerator = "";
        //if (((ExpertCard)card).GetExperience() > ((ExpertCard)card).GetCardSO().expertStatsObjectFromJson.levels[((ExpertCard)card).GetLevel() - 1].maxExperience) {
        //    numerator = ((ExpertCard)card).GetCardSO().expertStatsObjectFromJson.levels[((ExpertCard)card).GetLevel() - 1].maxExperience.ToString();
        //}
        //else
        //{
        //    numerator = ((ExpertCard)card).GetExperience().ToString();
        //}
        //SetText(experienceBarText, numerator + "\n" + ((ExpertCard)card).GetCardSO().expertStatsObjectFromJson.levels[((ExpertCard)card).GetLevel() - 1].maxExperience);
        //}
        ///
        //skillBtnRectTransform.anchoredPosition = new Vector2(skillBtnRectTransform.anchoredPosition.x, 60.5f);
        //skillBtnRectTransform.sizeDelta = new Vector2(skillBtnRectTransform.sizeDelta.x, 34.5f);
        //moreSkillInfoButtonUI.gameObject.SetActive(false);

        //skillEffect.sizeDelta = new Vector2(30, 30);
        //SetText(skillEffectText, skillEffectText.text);
        //skillEffect.anchoredPosition = new Vector2(-10.6f, 0);
        //skillExpTextGroup.gameObject.SetActive(true);
        //skillCost.gameObject.SetActive(true);
        //skillExpTextGroup.anchoredPosition = new Vector2(0, 0);
        //skillCost.anchoredPosition = new Vector2(53.5f, 0);
        //string[] wordsInTitle = skillNameText.text.Split(" ");

        ////Adjust skill name to fit 
        //if (wordsInTitle[0].Length > 9)
        //{
        //    skillNameText.fontSize = 11;
        //}
        //else if (wordsInTitle[0].Length > 8)
        //{
        //    //ugly magic number
        //    skillNameText.fontSize = 11.5f;
        //}
        //else if (wordsInTitle[0].Length > 7)
        //{
        //    skillNameText.text = skillNameText.text.Replace(" ", "\n");
        //    skillNameText.fontSize = 14;
        //    skillNameText.lineSpacing = -10;
        //}
        //else
        //{
        //    skillNameText.fontSize = 15;
        //}


    }
    private void AdaptLookForField()
    {
        background.sprite = Resources.Load<Sprite>(CARD_FRONT_IMAGE_NAME);
        overlay.sprite = Resources.Load<Sprite>(OVERLAY_IMAGE_NAME);
        cardButtonCollider.enabled = true;
        shedIcon.gameObject.SetActive(false);
        shedText.gameObject.SetActive(false);
        skillIcon.gameObject.SetActive(false);
        costContainer.Hide();
        powerContainer.Hide();
        HideSkillDescUI();
        //level.gameObject.SetActive(true);
        //level.anchorMin = new Vector2(1, 1);
        //level.anchorMax = new Vector2(1, 1);
        //level.anchoredPosition = new Vector2(0, 0);
        //level.sizeDelta = new Vector2(60, 60);
        //level.pivot = new Vector2(1, 1);
        //SetText(levelText, levelText.text);

        //quickLevelUpButtonRectTransform.anchorMin = new Vector2(1, 1);
        //quickLevelUpButtonRectTransform.anchorMax = new Vector2(1, 1);
        //quickLevelUpButtonRectTransform.anchoredPosition = new Vector2(0, 0);
        //quickLevelUpButtonRectTransform.sizeDelta = new Vector2(60, 60);
        //quickLevelUpButtonRectTransform.pivot = new Vector2(1, 1);

        stardustRectTransform.anchorMin = new Vector2(0, 1);
        stardustRectTransform.anchorMax = new Vector2(0, 1);
        stardustRectTransform.anchoredPosition = new Vector2(0, -177);
        stardustRectTransform.sizeDelta = new Vector2(50, 50);
        stardustRectTransform.pivot = new Vector2(0, 1);
        SetText(stardustText, stardustText.text);

        lightRectTransform.anchorMin = new Vector2(0, 1);
        lightRectTransform.anchorMax = new Vector2(0, 1);
        lightRectTransform.anchoredPosition = new Vector2(0, -27);
        lightRectTransform.sizeDelta = new Vector2(50, 50);
        lightRectTransform.pivot = new Vector2(0, 1);
        SetText(lightText, lightText.text);

        lifetime.anchorMin = new Vector2(1, 1);
        lifetime.anchorMax = new Vector2(1, 1);
        lifetime.anchoredPosition = new Vector2(0, -25);
        lifetime.sizeDelta = new Vector2(50, 50);
        lifetime.pivot = new Vector2(1, 1);
        SetText(lifetimeBarText, lifetimeBarText.text);


        //health.anchorMin = new Vector2(0, 1);
        //health.anchorMax = new Vector2(0, 1);
        //health.anchoredPosition = new Vector2(0, -175);
        //health.sizeDelta = new Vector2(50, 50);
        //health.pivot = new Vector2(0, 1);
        //SetText(healthBarText, ((ExpertCard)card).GetHealth().ToString());

        //tolerance.anchorMin = new Vector2(1, 1);
        //tolerance.anchorMax = new Vector2(1, 1);
        //tolerance.anchoredPosition = new Vector2(0, -120);
        //tolerance.sizeDelta = new Vector2(60, 60);
        //SetText(toleranceText, toleranceText.text);
        //tolerance.pivot = new Vector2(1, 1);

        //experienceBarLine.gameObject.SetActive(false);
        //experience.gameObject.SetActive(true);
        //experience.anchorMin = new Vector2(.5f, 0);
        //experience.anchorMax = new Vector2(.5f, 0);
        //experience.anchoredPosition = new Vector2(60, 0);
        //experience.sizeDelta = new Vector2(60, 60);
        //experience.pivot = new Vector2(.5f, 0);


        //    if (((ExpertCard)card).GetLevel() >= ((ExpertCard)card).GetCardSO().expertStatsObjectFromJson.levels.Count)
        //    {
        //        //reached max level
        //        SetText(experienceBarText, "");
        //    }
        //    else { 
        //    string numerator = "";
        //    if (((ExpertCard)card).GetExperience() > ((ExpertCard)card).GetCardSO().expertStatsObjectFromJson.levels[((ExpertCard)card).GetLevel() - 1].maxExperience)
        //    {
        //        numerator = ((ExpertCard)card).GetCardSO().expertStatsObjectFromJson.levels[((ExpertCard)card).GetLevel() - 1].maxExperience.ToString();
        //    }
        //    else
        //    {
        //        numerator = ((ExpertCard)card).GetExperience().ToString();
        //    }
        //    SetText(experienceBarText, numerator);


        //}
        //showLevelUpMenu.gameObject.SetActive(false);
        //ShowSkills();
        //detailedSkillInfoButton.gameObject.SetActive(false);
        //skillBtnRectTransform.anchoredPosition = new Vector2(skillBtnRectTransform.anchoredPosition.x, 90);
        //skillBtnRectTransform.sizeDelta = new Vector2(skillBtnRectTransform.sizeDelta.x, 55);
        //moreSkillInfoButtonUI.gameObject.SetActive(false);
        //skillEffect.anchoredPosition = new Vector2(67, 0);
        //skillEffect.sizeDelta = new Vector2(50, 50);
        //SetText(skillEffectText, skillEffectText.text);
        ////skillExpTextGroup.anchoredPosition = new Vector2(21.4f, -10.5f);
        ////skillCost.anchoredPosition = new Vector2(74.9f, -10.5f);
        //skillExpTextGroup.gameObject.SetActive(false);
        //skillCost.gameObject.SetActive(false);
        //string[] wordsInTitle = skillNameText.text.Split(" ");

        ////Adjust skill name to fit 
        //if (wordsInTitle[0].Length > 9)
        //{
        //    skillNameText.fontSize = 18;
        //}
        //else if (wordsInTitle[0].Length > 8)
        //{
        //    //ugly magic number
        //    skillNameText.fontSize = 21.6f;
        //}
        //else if (wordsInTitle[0].Length > 7)
        //{
        //    skillNameText.text = skillNameText.text.Replace(" ", "\n");
        //    skillNameText.fontSize = 23.8f;
        //    skillNameText.lineSpacing = -10;
        //}
        //else
        //{
        //    skillNameText.fontSize = 28.81f;
        //}
    }

    private void AdaptLookForOpponentField()
    {
        background.sprite = Resources.Load<Sprite>(CARD_FRONT_IMAGE_NAME);
        overlay.sprite = Resources.Load<Sprite>(OVERLAY_IMAGE_NAME);
        HideSkillDescUI();
        shedIcon.gameObject.SetActive(false);
        shedText.gameObject.SetActive(false);
        skillIcon.gameObject.SetActive(false);
        costContainer.Hide();
        powerContainer.Hide();
        cardButtonCollider.enabled = true;
        //level.gameObject.SetActive(true);
        //level.anchorMin = new Vector2(1, 1);

        //level.anchorMax = new Vector2(1, 1);
        //level.anchoredPosition = new Vector2(0, 0);
        //level.sizeDelta = new Vector2(60, 60);
        //level.pivot = new Vector2(1, 1);
        //SetText(levelText, levelText.text);

        //quickLevelUpButtonRectTransform.anchorMin = new Vector2(1, 1);
        //quickLevelUpButtonRectTransform.anchorMax = new Vector2(1, 1);
        //quickLevelUpButtonRectTransform.anchoredPosition = new Vector2(0, 0);
        //quickLevelUpButtonRectTransform.sizeDelta = new Vector2(60, 60);
        //quickLevelUpButtonRectTransform.pivot = new Vector2(1, 1);
        stardustRectTransform.anchorMin = new Vector2(0, 1);
        stardustRectTransform.anchorMax = new Vector2(0, 1);
        stardustRectTransform.anchoredPosition = new Vector2(0, -177);
        stardustRectTransform.sizeDelta = new Vector2(50, 50);
        stardustRectTransform.pivot = new Vector2(0, 1);
        SetText(stardustText, stardustText.text);

        lightRectTransform.anchorMin = new Vector2(0, 1);
        lightRectTransform.anchorMax = new Vector2(0, 1);
        lightRectTransform.anchoredPosition = new Vector2(0, -27);
        lightRectTransform.sizeDelta = new Vector2(50, 50);
        lightRectTransform.pivot = new Vector2(0, 1);
        SetText(lightText, lightText.text);

        lifetime.anchorMin = new Vector2(1, 1);
        lifetime.anchorMax = new Vector2(1, 1);
        lifetime.anchoredPosition = new Vector2(0, -25);
        lifetime.sizeDelta = new Vector2(50, 50);
        lifetime.pivot = new Vector2(1, 1);
        SetText(lifetimeBarText, lifetimeBarText.text);

        //healthBarLine.gameObject.SetActive(false);
        //health.anchorMin = new Vector2(.5f, 0);
        //health.anchorMax = new Vector2(.5f, 0);
        //health.anchoredPosition = new Vector2(0, 0);
        //health.sizeDelta = new Vector2(60, 60);
        //health.pivot = new Vector2(.5f, 0);
        //SetText(healthBarText, ((ExpertCard)card).GetHealth().ToString());

        //tolerance.anchorMin = new Vector2(1, 1);
        //tolerance.anchorMax = new Vector2(1, 1);
        //tolerance.anchoredPosition = new Vector2(0, -120);
        //tolerance.sizeDelta = new Vector2(60, 60);
        //SetText(toleranceText, toleranceText.text);
        //tolerance.pivot = new Vector2(1, 1);

        //experienceBarLine.gameObject.SetActive(false);
        //experience.gameObject.SetActive(true);
        //experience.anchorMin = new Vector2(.5f, 0);
        //experience.anchorMax = new Vector2(.5f, 0);
        //experience.anchoredPosition = new Vector2(60, 0);
        //experience.sizeDelta = new Vector2(60, 60);
        //experience.pivot = new Vector2(.5f, 0);


        //if (((ExpertCard)card).GetLevel() >= ((ExpertCard)card).GetCardSO().expertStatsObjectFromJson.levels.Count)
        //{
        //    //reached max level
        //    SetText(experienceBarText, "");
        //}
        //else
        //{
        //    string numerator = "";
        //    if (((ExpertCard)card).GetExperience() > ((ExpertCard)card).GetCardSO().expertStatsObjectFromJson.levels[((ExpertCard)card).GetLevel() - 1].maxExperience)
        //    {
        //        numerator = ((ExpertCard)card).GetCardSO().expertStatsObjectFromJson.levels[((ExpertCard)card).GetLevel() - 1].maxExperience.ToString();
        //    }
        //    else
        //    {
        //        numerator = ((ExpertCard)card).GetExperience().ToString();
        //    }
        //    SetText(experienceBarText, numerator);


        //}
        //showLevelUpMenu.gameObject.SetActive(false);


    }

    internal void StardustDanger()
    {
        stardustAnimator.SetTrigger("StardustDanger");
    }
    internal void LightDanger()
    {
        lightAnimator.SetTrigger("LightTextDanger");
    }
    public void CallOnEndAnimation()
    {
        OnEndAnimation?.Invoke(this, EventArgs.Empty);
    }

    internal void LifetimeDanger()
    {
       lifetimeAnimator.SetTrigger("LifetimeDanger");
    }

    internal void LightDust()
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = dust.GetComponent<ParticleSystem>().velocityOverLifetime;
        Vector2 value = GetCharacterImage().transform.position - lightRectTransform.transform.position;
        velocityOverLifetime.orbitalOffsetX = value.x;
        velocityOverLifetime.orbitalOffsetY = value.y;
        Instantiate(dust, lightText.transform);

    }

    internal void StardustDust()
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = dust.GetComponent<ParticleSystem>().velocityOverLifetime;
        Vector2 value = GetCharacterImage().transform.position - stardustText.transform.parent.transform.position;
        velocityOverLifetime.orbitalOffsetX = value.x;
        velocityOverLifetime.orbitalOffsetY = value.y;
        Instantiate(dust, stardustText.transform);

    }

    internal void showStatChangeMessage(CardStats stat, int change)
    {
        
        Transform parent = null;
        string message = "";
        switch (stat)
        {
            case CardStats.Light: 
                parent = lightText.transform; 
                break;

            case CardStats.Stardust:
                parent = stardustText.transform; 
                break;
        }
        if (change > 0)
        {
            message = "+" + change;
            
        }
        else if(change < 0)
        {
            message = change.ToString();
        }
        if (message != "") {
            float wait = 0;
            float yoffset = 0f;
            float xoffset = .1f;
            StartCoroutine(AnimationManager.Instance.StatMessage(parent, message, null, wait,xoffset, yoffset));
        }
    }

    public Animator GetLifetimeAnimator()
    {
        return lifetimeAnimator;
    }
    public Image GetLightIcon()
    {
        return lightRectTransform.GetComponentInChildren<Image>();
    }
    public CostContainer GetCostContainer()
    {
        return costContainer;
    }
}
