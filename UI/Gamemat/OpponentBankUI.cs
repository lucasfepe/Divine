using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpponentBankUI : MonoBehaviour
{
    public static OpponentBankUI Instance { get; private set;}

    [SerializeField] private TextMeshProUGUI lightText;
    [SerializeField] private TextMeshProUGUI stardustText;
    [SerializeField] private RockShaderController stardust;
    [SerializeField] private RockShaderController lightIcon;
    private ChangeTextGradually changeStardustTextGradually;
    private ChangeTextGradually changeLightTextGradually;
    private float speedUpWhenDecrease = 5f;
    private void Awake()
    {
        Instance = this;
        changeStardustTextGradually = stardustText.GetComponent<ChangeTextGradually>();
        changeLightTextGradually = lightText.GetComponent<ChangeTextGradually>();
    }
    private void Start()
    {

        
        CardGameManager.Instance.OnBeginMatch += CardGameManager_OnBeginMatch; ;
        
    }

    private void CardGameManager_OnBeginMatch(object sender, EventArgs e)
    {
        CardGameManager.Instance.GetPlayer().OnSetOpponentStardust += IPlayer_OnUpdateOpponentStardust;
        CardGameManager.Instance.GetPlayer().OnSetOpponentLight += IPlayer_OnSetOpponentLight;
        
    }

    private void IPlayer_OnSetOpponentLight(object sender, EventArgs e)
    {
        int previousLight = Int32.Parse(lightText.text);
        int lightValue = CardGameManager.Instance.GetOpponent().GetLight();
        int lightDiff =  lightValue - previousLight;
        float lightTimer;

        if (lightDiff < 0)
        {
            lightTimer = RockShaderController.EnergyIntensityToPeriodStatic(-1 * lightDiff / speedUpWhenDecrease);
            changeLightTextGradually.SetText(lightTimer, lightValue);
        }
        else if (lightDiff > 0)
        {
            lightTimer = RockShaderController.EnergyIntensityToPeriodStatic(lightDiff);
            changeLightTextGradually.SetText(lightTimer, lightValue);
            GlowLight(lightDiff);
        }
    }
//ugly maybe this whole thing is also done in BankUI

    private void IPlayer_OnUpdateOpponentStardust(object sender, EventArgs e)
    {
        int previousStardust = Int32.Parse(stardustText.text);
        int stardust = CardGameManager.Instance.GetOpponent().GetStardust();
        int stardustDiff = stardust - previousStardust;
        float stardustTimer;

        if (stardustDiff < 0)
        {
            stardustTimer = RockShaderController.EnergyIntensityToPeriodStatic(-1 * stardustDiff / speedUpWhenDecrease);

            changeStardustTextGradually.SetText(stardustTimer, stardust);
        }
        else if (stardustDiff > 0)
        {
            stardustTimer = RockShaderController.EnergyIntensityToPeriodStatic(-1 * stardustDiff);

            changeStardustTextGradually.SetText(stardustTimer, stardust);
            GlowStardust(stardustDiff);
        }
    }

    public void SetLightText(int value)
    {
        lightText.text = value.ToString();
    }
    public void SetStardustText(int value)
    {
        stardustText.text = value.ToString();
    }

    public void GlowStardust(int energyGen)
    {
        float dimmingFactor = 3f;
        stardust.Glow(energyGen, dimmingFactor);

    }
    public void GlowLight(int lightGen)
    {
        float dimmingFactor = 4.5f;
        lightIcon.Glow(lightGen, dimmingFactor);
    }
}
