using DG.Tweening;
using Mono.CSharp;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class BeSuckedByBlackHoleAnimation: MonoBehaviour 
{
    private const string PINCH_AMOUNT = "_PinchUvAmount";
    private const string ROTATE_AMOUNT = "_RotateUvAmount";
    private const string TWIST_AMOUNT = "_TwistUvAmount";
    private const string TWIST_RADIUS = "_TwistUvRadius";
    private const string ZOOM_AMOUNT = "_ZoomUvAmount";
    private const string HIT_BLEND = "_HitEffectBlend";
    private const string HIT_GLOW = "_HitEffectGlow";
    private const string OFFSET_Y = "_OffsetUvY";
    private const string OFFSET_X = "_OffsetUvX";
    private const string ALPHA = "_Alpha";
    private const string GLOW = "_Glow";
    private Material newMaterial;
    private Material graphicMaterial;
    private Material fadeMaterial;
    private BlackHoleSuckAnimation blackHoleSuckAnimation;

    [SerializeField] private CardAnimationReferences cardAnimationReferences;

    private void Awake()
    {
        blackHoleSuckAnimation = GetComponentInParent<BlackHoleSuckAnimation>();
    }

    private void Start()
    {
        newMaterial = cardAnimationReferences.GetMaterial();
        graphicMaterial = new Material(newMaterial);
        fadeMaterial = new Material(newMaterial);


    }
    public void BeSuckedByBlackHole()
    {

        float endPinchAmount = .5f;
        float endRotateAmount = 6.28f;
        float endTwistAmount = 1.4f;
        float endTwistRadius = .75F;
        float endZoomAmount = 1.8f;
        float endHitBlend = 1;
        float endHitGlow = 30;
        float endScaleAmount = 0f;
        float endTranslationY = -.075f;
        float endTranslationX = .05f;
        float duration = 5;
        float fastDuration = 1f;
        float delay = 0;
        float delay2 = .5f;
        float delay3 = 1f;
        float endGlow = 10f;

        cardAnimationReferences.GetCardOverlay().material = newMaterial;
        cardAnimationReferences.GetCardBorder().material = newMaterial;
        cardAnimationReferences.GetSkillImage().material = fadeMaterial;
        cardAnimationReferences.GetTimeIconImage().material = fadeMaterial;
        cardAnimationReferences.GetShedImage().material = fadeMaterial;
        cardAnimationReferences.GetStardustImage().material = fadeMaterial;
        cardAnimationReferences.GetLightImage().material = fadeMaterial;
        cardAnimationReferences.GetRedGiantImage().material = fadeMaterial;
        cardAnimationReferences.GetGraphicImage().material = graphicMaterial;

        cardAnimationReferences.GetStatuses().GetComponentsInChildren<Image>().ToList().ForEach(x => {
            x.CrossFadeAlpha(0, .5f, true);
        });










        //make blackhole appear
        //ugly definitely needs improvement how come I only call half the animation but the other hald is defined bellow?

        blackHoleSuckAnimation.Consume();

        FadeOut(fastDuration,delay );
        Pinch(duration-1, delay3, endPinchAmount);
        Rotate(duration - 3, delay2, endRotateAmount);
        Twist(duration - 3 , delay2 , endTwistRadius);
        Zoom(duration - 4.2f, delay2, endZoomAmount);
        TranslateBorder(duration / 4, delay3 , endTranslationY, endTranslationX);
        ScaleDown(duration-3.5f, delay3 + .7f, endScaleAmount);
        Hit(duration - 2, delay3, endHitBlend, endHitGlow);
        //Glow(duration - 2, delay2 * 1.2f, endGlow);





    }

    public void FadeOut2(float duration)
    {
        DOTween.To(() => newMaterial.GetFloat(ALPHA), x => newMaterial.SetFloat(ALPHA, x), 0, duration);
        DOTween.To(() => graphicMaterial.GetFloat(ALPHA), x => graphicMaterial.SetFloat(ALPHA, x), 0, duration);
    }

    private void TranslateBorder(float duration, float delay, float endTranslationY, float endTranslationX)
    {
        //DOTween.To(() => newMaterial.GetFloat(OFFSET_X), x => newMaterial.SetFloat(OFFSET_X, x), endTranslationX, duration).SetDelay(delay);
        DOTween.To(() => newMaterial.GetFloat(OFFSET_Y), x => newMaterial.SetFloat(OFFSET_Y, x), endTranslationY, duration).SetDelay(delay / 2);


    }

    private void FadeOut(float duration, float delay)
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
    private void Pinch(float duration, float delay, float endPinchAmount)
    {
        DOTween.To(() => newMaterial.GetFloat(PINCH_AMOUNT), x => newMaterial.SetFloat(PINCH_AMOUNT, x), endPinchAmount, duration).SetDelay(delay / 2);
        DOTween.To(() => graphicMaterial.GetFloat(PINCH_AMOUNT), x => graphicMaterial.SetFloat(PINCH_AMOUNT, x), endPinchAmount, duration / 2);
    }
    private void Rotate(float duration, float delay, float endRotateAmount)
    {
        //DOTween.To(() => newMaterial.GetFloat(ROTATE_AMOUNT), x => newMaterial.SetFloat(ROTATE_AMOUNT, x), endRotateAmount, duration).SetDelay(delay + delay);
        DOTween.To(() => graphicMaterial.GetFloat(ROTATE_AMOUNT), x => graphicMaterial.SetFloat(ROTATE_AMOUNT, x), endRotateAmount, duration).SetDelay(delay);
    }
    private void Twist(float duration, float delay, float endTwistRadius)
    {
        
        DOTween.To(() => newMaterial.GetFloat(TWIST_RADIUS), x => newMaterial.SetFloat(TWIST_RADIUS, x), endTwistRadius , duration);

       
        DOTween.To(() => graphicMaterial.GetFloat(TWIST_RADIUS), x => graphicMaterial.SetFloat(TWIST_RADIUS, x), endTwistRadius , duration / 3);
    }
    private void Zoom(float duration, float delay, float endZoomAmount)
    {
        DOTween.To(() => newMaterial.GetFloat(ZOOM_AMOUNT), x => newMaterial.SetFloat(ZOOM_AMOUNT, x), endZoomAmount, duration).SetDelay(delay);
        DOTween.To(() => graphicMaterial.GetFloat(ZOOM_AMOUNT), x => graphicMaterial.SetFloat(ZOOM_AMOUNT, x), endZoomAmount, duration * .8f).SetDelay(delay  );
    }
    private void Hit(float duration, float delay, float endHitBlend, float endHitGlow)
    {
        DOTween.To(() => newMaterial.GetFloat(HIT_BLEND), x => newMaterial.SetFloat(HIT_BLEND, x), endHitBlend, duration).SetDelay(delay);
        DOTween.To(() => graphicMaterial.GetFloat(HIT_BLEND), x => graphicMaterial.SetFloat(HIT_BLEND, x), endHitBlend, duration).SetDelay(delay);

        DOTween.To(() => newMaterial.GetFloat(HIT_BLEND), x => newMaterial.SetFloat(HIT_GLOW, x), endHitGlow, duration).SetDelay(delay);
        DOTween.To(() => graphicMaterial.GetFloat(HIT_BLEND), x => graphicMaterial.SetFloat(HIT_GLOW, x), endHitGlow, duration).SetDelay(delay);
        
        //float y = 0;
        //float waitUntilShowWhiteDwarfIcon = 2.25f;
        //DOTween.To(() => 1, x => y = x, 2, waitUntilShowWhiteDwarfIcon).OnComplete(() => GetComponentInChildren<WhiteDwarfAnimator>().Appear());
    }
    private void Glow(float duration, float delay, float endGlow)
    {
        DOTween.To(() => newMaterial.GetFloat(GLOW), x => newMaterial.SetFloat(GLOW, x), endGlow, duration).SetDelay(delay);
        DOTween.To(() => graphicMaterial.GetFloat(GLOW), x => graphicMaterial.SetFloat(GLOW, x), endGlow, duration).SetDelay(delay);
       
    }
    private void ScaleDown(float  duration, float delay, float endScaleAmount)
    {
        Vector2 endScale = new Vector2(endScaleAmount, endScaleAmount);
        RectTransform rectTransform = cardAnimationReferences.GetGraphicImage().GetComponent<RectTransform>();
        RectTransform rectTransform2 = cardAnimationReferences.GetCardBorder().GetComponent<RectTransform>();
        RectTransform rectTransform3 = cardAnimationReferences.GetCardOverlay().GetComponent<RectTransform>();
        DOTween.To(() => (Vector2)rectTransform.localScale, x => rectTransform.localScale = new Vector2(x.x, x.y), endScale, duration).SetDelay(delay);
        DOTween.To(() => (Vector2)rectTransform2.localScale, x => rectTransform2.localScale = new Vector2(x.x, x.y), endScale, duration).SetDelay(delay);
        DOTween.To(() => (Vector2)rectTransform3.localScale, x => rectTransform3.localScale = new Vector2(x.x, x.y), endScale, duration).SetDelay(delay).OnComplete(() =>
        {
            if(GetComponentInParent<BaseCard>().GetCardOwner() == Player.Instance.IAm())
            GetComponentInParent<BaseCard>().Die();
        });
    }
}