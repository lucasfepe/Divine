using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NeutronStarAnimation : MonoBehaviour
{
    

    private Material fadeMaterial;
    private Material graphicMaterial;

    private Material newMaterial;
    private bool issss = false;
    
    [SerializeField] protected GameObject light3;
    [SerializeField] private CardAnimationReferences cardAnimationReferences;    

    private const string FADE_AMOUNT = "_FadeAmount";
    private const string FADE_BURN_WIDTH = "_FadeBurnWidth";
    private const string DISTORTION_AMOUNT = "_DistortAmount";
    private const string ALPHA = "_Alpha";
    private const string OVERLAY_BLEND = "_OverlayBlend";

    private float directionX = 0;
    private float directionY = 0;
    private BaseCard card;
    private ParticleSystem.EmissionModule fire3ParticlesParticleSystemEmission;
    private ParticleSystem fire3ParticlesParticleSystem;

    public event EventHandler OnStartAbsorbingStardust;

    ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeFire3Particles;

    private void Awake()
    {
        card = GetComponentInParent<BaseCard>();
    }

    private void Start()
    {
        newMaterial = cardAnimationReferences.GetMaterial();
    }

    private void StepOne_CharacterImageFireOverlay(float duration)
    {
        
    }
    private void StepTwo_EntireCardFireOverlay(float duration, float delay)
    {
       

    }

    private void StepThree_Boiling(float duration, float delay)
    {
       
    }
    private void FadeOut(float duration, float delay, int stardustGain)
    {
        float endAlpha = 0;

        DOTween.To(() => newMaterial.GetFloat(ALPHA), x => newMaterial.SetFloat(ALPHA, x), endAlpha, duration).SetDelay(delay).OnComplete(() => AnimationManager.Instance.Supernova(card.transform, stardustGain, card.GetCardOwner() == Player.Instance.IAm())) ;


        //text fade out

        //fade out text and small icons
        CanvasGroup timeTextCanvasGroup = cardAnimationReferences.GetTimeText().GetComponent<CanvasGroup>();
        CanvasGroup titleTextCanvasGroup = cardAnimationReferences.GetTitleText().GetComponent<CanvasGroup>();
        CanvasGroup shedTextCanvasGroup = cardAnimationReferences.GetShedText().GetComponent<CanvasGroup>();
        CanvasGroup stardustTextCanvasGroup = cardAnimationReferences.GetStardustText().GetComponent<CanvasGroup>();
        CanvasGroup lightTextCanvasGroup = cardAnimationReferences.GetLightText().GetComponent<CanvasGroup>();

       
        //timeIconImageCanvasGroup.DOFade(0, invisibleTime).SetDelay(cumulativeTime);
        //shedImageCanvasGroup.DOFade(0, invisibleTime).SetDelay(cumulativeTime);
        timeTextCanvasGroup.DOFade(0, duration).SetDelay(delay);
        titleTextCanvasGroup.DOFade(0, duration).SetDelay(delay);
        shedTextCanvasGroup.DOFade(0, duration).SetDelay(delay);
        stardustTextCanvasGroup.DOFade(0, duration).SetDelay(delay);
        lightTextCanvasGroup.DOFade(0, duration).SetDelay(delay);


    }
        
    private void Explosions()
    {

        GameObject gameObject1 = Instantiate(light3, transform);
        gameObject1.GetComponentInChildren<KillOnSupernovaStop>().SetCard(card);
        //Instantiate(fire1, transform);
    }
    
    private void StepFive_Disintegrate(float duration, float delay, float endFadeBurnWidth, bool changeStats)
    {
       
    }
    private void StepSeven(float duration, float delay)
    {
        //start disintegrated fade out

    }
    

   

    public void Supernova(int stardustGain)
    {

        cardAnimationReferences.GetCardOverlay().material = newMaterial;
        cardAnimationReferences.GetCardBorder().material = newMaterial;
        cardAnimationReferences.GetSkillImage().material = newMaterial;
        cardAnimationReferences.GetTimeIconImage().material = newMaterial;
        cardAnimationReferences.GetShedImage().material = newMaterial;
        cardAnimationReferences.GetStardustImage().material = newMaterial;
        cardAnimationReferences.GetLightImage().material = newMaterial;
        cardAnimationReferences.GetRedGiantImage().material = newMaterial;
        cardAnimationReferences.GetGraphicImage().material = newMaterial;
        cardAnimationReferences.GetStatuses().GetComponentsInChildren<Image>().ToList().ForEach(x => {
            x.CrossFadeAlpha(0, .5f, true);
        });


        float fadeOutDuration = .1f;
        float fadeOutDelay = 1f;

        Explosions();
        FadeOut(fadeOutDuration, fadeOutDelay, stardustGain);
        //StepSeven(stepSevenDuration, stepOneDuration + stepTwoDuration + stepThreeDuration + stepFourDuration + stepSixDuration);
    }
}
