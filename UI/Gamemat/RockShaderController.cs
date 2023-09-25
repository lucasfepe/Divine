using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RockShaderController : MonoBehaviour
{
    private Material material;
    private bool isGlowing = false;
    private bool isGlowingFastAndBright = false;
    private const string TIME_SHADER_VARIABLE = "_time";
    private const string PERIOD_SHADER_VARIABLE = "_period";
    private float timer = 0;
    private float timerMax;
    private float dimmingFactor = 1f;
    private float timeDimAdjustFactor = 1f;
    private void Awake()
    {
        Image image = GetComponent<Image>();
        material = new Material(GetComponent<Image>().material);
        image.material = material;
        material.SetFloat(TIME_SHADER_VARIABLE, timer);
        float defaultPeriod = 1f;
        material.SetFloat(PERIOD_SHADER_VARIABLE, defaultPeriod);
    }
    private void Start()
    {
        CardGameManager.Instance.OnBeginTurn += CardGameManager_OnBeginTurn;
    }
    private void OnDestroy()
    {
        CardGameManager.Instance.OnBeginTurn -= CardGameManager_OnBeginTurn;
    }
    private void CardGameManager_OnBeginTurn(object sender, System.EventArgs e)
    {
        if(transform.parent.parent.TryGetComponent(out StatusIconsUI statusIconsUI))
        {
            //is a status icon
            GlowFastAndBright();
        }
    }

    private void Update()
    {
        if (isGlowing)
        {
            timer += Time.deltaTime / timeDimAdjustFactor;
                /// dimmingFactor;
            material.SetFloat(TIME_SHADER_VARIABLE, timer);
            if(timer > timerMax)
            {
                timeDimAdjustFactor = 1f;
                isGlowing = false;
                timer = 0;
            }
        }
        else if (isGlowingFastAndBright)
        {
            float fastRateOfTime = 2f;
            timer += Time.deltaTime * fastRateOfTime;
            material.SetFloat(TIME_SHADER_VARIABLE, timer);
            if (timer > timerMax)
            {
                isGlowingFastAndBright = false;
                timer = 0;
            }
        }
    }

    public void Glow(float energyGenerated)
    {
        timerMax = EnergyIntensityToPeriod(energyGenerated);
        material.SetFloat(PERIOD_SHADER_VARIABLE, timerMax);
        isGlowing =true;
    }
    public void GlowLight(float energyGenerated)
    {
        timerMax = EnergyIntensityToLightPeriod(energyGenerated);
        material.SetFloat(PERIOD_SHADER_VARIABLE, timerMax);
        isGlowing = true;
    }
    public void Glow(float energyGenerated, float dimmingFactor)
    {
        this.dimmingFactor = dimmingFactor;
        timerMax = EnergyIntensityToPeriod(energyGenerated / dimmingFactor);
        timeDimAdjustFactor = EnergyIntensityToPeriod(energyGenerated) / timerMax;
        material.SetFloat(PERIOD_SHADER_VARIABLE, timerMax );
        isGlowing = true;
    }
    public float EnergyIntensityToPeriod(float energyCount)
    {
        return Mathf.Log10(energyCount) + 1;
    }
    public float EnergyIntensityToLightPeriod(float energyCount)
    {
        return (float)(1.9 * Mathf.Log10(energyCount / 5 + 1) + 0.5);
    }


    public static float EnergyIntensityToPeriodStatic(float energyCount)
    {
        return Mathf.Log10(Mathf.Abs(energyCount)) + 1;
    }

    public void GlowFastAndBright()
    {
        timeDimAdjustFactor = 2f;
        timerMax = 2;
        //start at the height of brightness
        timer = timerMax / 2;
        material.SetFloat(PERIOD_SHADER_VARIABLE, timerMax);
        isGlowingFastAndBright = true;
    }
    public void SetMaterial(Material material)
    {
        this.material = material;
    }
}
