using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DesintegrateAnimation : MonoBehaviour
{
    private Material fadeMaterial;
    private Material graphicMaterial;

    private Material newMaterial;
    private bool issss = false;

    
    [SerializeField] protected GameObject fire3;
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
        Transform particlesFire3 = fire3.transform.Find("Particle");
        
        fire3ParticlesParticleSystem = particlesFire3.GetComponent<ParticleSystem>();
        velocityOverLifetimeFire3Particles = fire3ParticlesParticleSystem.velocityOverLifetime;
         fire3ParticlesParticleSystemEmission = fire3ParticlesParticleSystem.emission;
    }

    private void Start()
    {
        newMaterial = cardAnimationReferences.GetMaterial();
    }

    private void StepOne_CharacterImageFireOverlay(float duration)
    {
        Image graphicImage = cardAnimationReferences.GetGraphicImage();
        graphicMaterial = new Material(graphicImage.material);
        graphicImage.material = graphicMaterial;
        float endOverlayBlend = 0.25f;

        //make only character image overlay of fire
        DOTween.To(() => graphicImage.material.GetFloat(OVERLAY_BLEND), x => graphicImage.material.SetFloat(OVERLAY_BLEND, x), endOverlayBlend, duration).OnComplete(() => {
            Explosions();
            //GasMove();
            //LightParticlesMove(); 
        });
    }
    private void StepTwo_EntireCardFireOverlay(float duration, float delay)
    {
        float endFadeAmount = 1f;

        //make entire card overlay of fire

        DOTween.To(() => newMaterial.GetFloat(FADE_AMOUNT), x => newMaterial.SetFloat(FADE_AMOUNT, x), endFadeAmount, duration).SetDelay(delay);
        DOTween.To(() => fadeMaterial.GetFloat(FADE_AMOUNT), x => fadeMaterial.SetFloat(FADE_AMOUNT, x), endFadeAmount, duration).SetDelay(delay);
        DOTween.To(() => graphicMaterial.GetFloat(FADE_AMOUNT), x => graphicMaterial.SetFloat(FADE_AMOUNT, x), endFadeAmount, duration).SetDelay(delay);

    }

    private void StepThree_Boiling(float duration, float delay)
    {
        float endDistortionAmount = .5f;

        //start bubbling entire card

        DOTween.To(() => newMaterial.GetFloat(DISTORTION_AMOUNT), x => newMaterial.SetFloat(DISTORTION_AMOUNT, x), endDistortionAmount, duration).SetDelay(delay);
        DOTween.To(() => fadeMaterial.GetFloat(DISTORTION_AMOUNT), x => fadeMaterial.SetFloat(DISTORTION_AMOUNT, x), endDistortionAmount, duration).SetDelay(delay);
        DOTween.To(() => graphicMaterial.GetFloat(DISTORTION_AMOUNT), x => graphicMaterial.SetFloat(DISTORTION_AMOUNT, x), endDistortionAmount, duration).SetDelay(delay);
    }
    private void StepFour_FadeOutText(float duration, float delay)
    {
        //fade out text and small icons
        CanvasGroup timeTextCanvasGroup = cardAnimationReferences.GetTimeText().GetComponent<CanvasGroup>();
        CanvasGroup titleTextCanvasGroup = cardAnimationReferences.GetTitleText().GetComponent<CanvasGroup>();
        CanvasGroup shedTextCanvasGroup = cardAnimationReferences.GetShedText().GetComponent<CanvasGroup>();
        CanvasGroup stardustTextCanvasGroup = cardAnimationReferences.GetStardustText().GetComponent<CanvasGroup>();
        CanvasGroup lightTextCanvasGroup = cardAnimationReferences.GetLightText().GetComponent<CanvasGroup>();

        DOTween.To(() => fadeMaterial.GetFloat(ALPHA), x => fadeMaterial.SetFloat(ALPHA, x), 0, duration).SetDelay(delay);
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
        //explosions
        float speed = 0;
        velocityOverLifetimeFire3Particles.orbitalOffsetX = directionX;
        velocityOverLifetimeFire3Particles.orbitalOffsetY = directionY;

        

        if (directionX == 0) { speed = 4; }
        else if (Mathf.Abs(directionX) < 4) { speed = 6; }
        else if (Mathf.Abs(directionX) < 8) { speed = 11; }
        else{ speed = 16; }
        ParticleSystem.MinMaxCurve radial = velocityOverLifetimeFire3Particles.radial;
        radial.mode = ParticleSystemCurveMode.TwoCurves;
        radial.curveMultiplier = speed;
        velocityOverLifetimeFire3Particles.radial = radial;

        float stardustEmissionRatio = 20;

        fire3ParticlesParticleSystemEmission.rateOverTimeMultiplier = stardustEmissionRatio * card.GetCardStardust();


        GameObject explosion = Instantiate(fire3);

       
        

       

        explosion.transform.position = transform.position;
        //explosion.GetComponentInChildren<KillOnDisintegrateStop>().SetCard(card);
        //Instantiate(fire1, transform);
    }
    
    private void StepFive_Disintegrate(float duration, float delay, float endFadeBurnWidth, bool changeStats)
    {
        //start disintegrate
        DOTween.To(() => newMaterial.GetFloat(FADE_BURN_WIDTH), x => newMaterial.SetFloat(FADE_BURN_WIDTH, x), endFadeBurnWidth, duration).SetDelay(delay);
        if (changeStats) {
            DOTween.To(() => graphicMaterial.GetFloat(FADE_BURN_WIDTH), x => graphicMaterial.SetFloat(FADE_BURN_WIDTH, x), endFadeBurnWidth, duration).SetDelay(delay).OnComplete(() => {

                if (card.GetCardOwner() == Player.Instance.OpponentIs()) {
                    OnStartAbsorbingStardust?.Invoke(this, EventArgs.Empty);
                }
                card.Die();
            }

            );
        }
        else {
            DOTween.To(() => graphicMaterial.GetFloat(FADE_BURN_WIDTH), x => graphicMaterial.SetFloat(FADE_BURN_WIDTH, x), endFadeBurnWidth, duration).SetDelay(delay);
        }



    }
    private void StepSeven(float duration, float delay)
    {
        //start disintegrated fade out

        DOTween.To(() => newMaterial.GetFloat(ALPHA), x => newMaterial.SetFloat(ALPHA, x), 0, duration).SetDelay(delay);
        DOTween.To(() => graphicMaterial.GetFloat(ALPHA), x => graphicMaterial.SetFloat(ALPHA, x), 0, duration).SetDelay(delay);
    }
    

   

    public void Disintegrate(float directionX)
    {
        this.directionX = directionX;
        this.directionY = GetComponentInParent<BaseCard>().GetCardOwner() == Player.Instance.IAm() ? 2.4f : -2.1f;

        //don't want to change status material
        List<Image> images = GetComponentsInChildren<Image>()
            .Where(x => !x.transform.parent.TryGetComponent<StatusInfo>(out _))
            .ToList();
        images.ForEach(x => x.material = newMaterial);
       

        fadeMaterial = new Material(newMaterial);
        cardAnimationReferences.GetSkillImage().material = fadeMaterial;
        cardAnimationReferences.GetTimeIconImage().material = fadeMaterial;
        cardAnimationReferences.GetShedImage().material = fadeMaterial;
        cardAnimationReferences.GetStardustImage().material = fadeMaterial;
        cardAnimationReferences.GetLightImage().material = fadeMaterial;
        cardAnimationReferences.GetRedGiantImage().material = fadeMaterial;
        

        cardAnimationReferences.GetStatuses().GetComponentsInChildren<Image>().ToList().ForEach(x => {
            x.CrossFadeAlpha(0, .5f, true);
        });



        float stepOneDuration = .5f;
        float stepTwoDuration = 1f;
        float stepThreeDuration = .5f;
        float stepFourDuration = .5f;
        float stepFiveDuration = 1f;

        StepOne_CharacterImageFireOverlay(stepOneDuration);
        StepTwo_EntireCardFireOverlay(stepTwoDuration, stepOneDuration / 2);
        StepThree_Boiling(stepThreeDuration, stepOneDuration / 2);
        StepFour_FadeOutText(stepFourDuration, stepOneDuration / 2);
        StepFive_Disintegrate(stepFiveDuration, stepOneDuration / 4, .5f, true);
        StepFive_Disintegrate(stepFiveDuration, stepOneDuration / 4 + stepFiveDuration, 0f, false);
        //StepSeven(stepSevenDuration, stepOneDuration + stepTwoDuration + stepThreeDuration + stepFourDuration + stepSixDuration);









    }
}
