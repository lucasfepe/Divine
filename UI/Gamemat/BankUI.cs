using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BankUI : MonoBehaviour
{
    public static BankUI Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI stardustText;
    [SerializeField] private TextMeshProUGUI lightText;
    [SerializeField] private float speedUpWhenDecrease = 5f;

    [SerializeField] private GlowShaderController stardust;
    [SerializeField] private GlowShaderController lightIcon;

    private ChangeTextGradually changeStardustTextGradually;
    private ChangeTextGradually changeLightTextGradually;
    private float stardustTimer;
    private float lightTimer;

    private void Awake()
    {
        Instance = this;
        changeStardustTextGradually = stardustText.GetComponent<ChangeTextGradually>();
        changeLightTextGradually = lightText.GetComponent<ChangeTextGradually>();
        CardGameManager.Instance.OnBeginMatch += CardGameManager_OnBeginMatch;
    }
    private void Start()
    {
        
    }

    private void CardGameManager_OnBeginMatch(object sender, EventArgs e)
    {
        CardGameManager.Instance.GetPlayer().OnSetMyStardust += IPlayer_OnUpdateBankedStardust;
        CardGameManager.Instance.GetPlayer().OnSetMyLight += IPlayer_OnSetMyLight;
    }

    private void IPlayer_OnSetMyLight(object sender, EventArgs e)
    {
        int previousLight = Int32.Parse(lightText.text);
        int lightValue = CardGameManager.Instance.GetPlayer().GetLight();
        int lightDiff = lightValue - previousLight;
        if (lightDiff < 0)
        {
            lightTimer = GlowShaderController.EnergyIntensityToPeriodStatic(-1 * lightDiff / speedUpWhenDecrease);
            changeLightTextGradually.SetText(lightTimer, lightValue);
        }
        else if (lightDiff > 0)
        {
            lightTimer = GlowShaderController.EnergyIntensityToPeriodStatic(lightDiff);
            changeLightTextGradually.SetText(lightTimer, lightValue);
            GlowLight(lightDiff);
        }
    }

    private void IPlayer_OnUpdateBankedStardust(object sender, EventArgs e)
    {
        //sadly this will activate for both connected plyers when want to activate only for player one
        int previousStardust = Int32.Parse(stardustText.text);
        int stardust = CardGameManager.Instance.GetPlayer().GetStardust();
        int stardustDiff = stardust - previousStardust;

        if (stardustDiff < 0)
        {
            stardustTimer = GlowShaderController.EnergyIntensityToPeriodStatic(-1 * stardustDiff / speedUpWhenDecrease);

            changeStardustTextGradually.SetText(stardustTimer, stardust);
        }
        else if (stardustDiff > 0)
        {
            stardustTimer = GlowShaderController.EnergyIntensityToPeriodStatic(-1 * stardustDiff);

            changeStardustTextGradually.SetText(stardustTimer, stardust);
        }
    }


    //ugly avoid calling Instance instead call the method in the instance class itself
    //ugly don't think need re do this can make 2 into 1 methods?
    

    

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

    public GlowShaderController GetStardustGlowShaderController()
    {
        return stardust;
    }
}
