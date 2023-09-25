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

    [SerializeField] private RockShaderController stardust;
    [SerializeField] private RockShaderController lightIcon;

    private ChangeTextGradually changeStardustTextGradually;
    private ChangeTextGradually changeLightTextGradually;
    private float stardustTimer;
    private float lightTimer;
    

   
    

    private void Awake()
    {
        Instance = this;
        changeStardustTextGradually = stardustText.GetComponent<ChangeTextGradually>();
        changeLightTextGradually = lightText.GetComponent<ChangeTextGradually>();


    }
    private void Start()
    {
        CardGameManager.Instance.OnUpdateBankedStardust += CardGameManager_OnUpdateBankedStardust;
        CardGameManager.Instance.OnUpdateBankedLight += CardGameManager_OnUpdateBankedLight;
    }
    //ugly avoid calling Instance instead call the method in the instance class itself
    //ugly don't think need re do this can make 2 into 1 methods?
    private void CardGameManager_OnUpdateBankedLight(object sender, EventArgs e)
    {
        int previousLight = Int32.Parse(lightText.text);
        int lightDiff = 0;
        int lightValue = 0;
        
        if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            lightDiff = DivineMultiplayer.Instance.playerOneLight.Value - previousLight;
            lightValue = DivineMultiplayer.Instance.playerOneLight.Value;
        }
        else if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            lightDiff = DivineMultiplayer.Instance.playerTwoLight.Value - previousLight;
            lightValue = DivineMultiplayer.Instance.playerTwoLight.Value;
        }

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

    private void CardGameManager_OnUpdateBankedStardust(object sender, System.EventArgs e)
    {
        int previousStardust = Int32.Parse(stardustText.text);
        int stardust = DivineMultiplayer.Instance.GetStardust();
        int stardustDiff = stardust - previousStardust;

       

        if (stardustDiff < 0)
        {
            stardustTimer = RockShaderController.EnergyIntensityToPeriodStatic(-1 * stardustDiff / speedUpWhenDecrease);

            changeStardustTextGradually.SetText(stardustTimer, stardust);
        }
        else if (stardustDiff > 0)
        {
            stardustTimer = RockShaderController.EnergyIntensityToPeriodStatic(-1 * stardustDiff );

            changeStardustTextGradually.SetText(stardustTimer, stardust);
        }
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
