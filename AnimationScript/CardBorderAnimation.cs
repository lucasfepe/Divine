using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class CardBorderAnimation : MonoBehaviour
{

    private Material material;
    private Image image;
    private bool issss = false;

    public event EventHandler OnCardShedAnimationDone;

    private void Awake()
    {
        image = GetComponent<Image>();
        material = new Material(image.material);
        image.material = material;

    }
   
    public void BecomeRedGiant()
    {
        float borderThickness = 0.08f;
        float transitionDuration = 1f;
        float endingAlpha = 1f;
        DOTween.To(() => material.GetFloat("_OutlineAlpha"), x => material.SetFloat("_OutlineAlpha", x), endingAlpha, transitionDuration);
        DOTween.To(() => material.GetFloat("_OutlineWidth"), x => material.SetFloat("_OutlineWidth", x), borderThickness, transitionDuration);
    }
    public void Shed()
    {
        issss = true ;
        float borderThickness = 0.2f;
        float borderThickness2 = 0.05f;
        float borderThickness3 = 0f;
        float transitionDuration = 1.5f;
        float delay = .5f;
        float delay2 = 0.25f;
        float startingAlpha = 1f;
        float endingAlpha = 0f;
        float outlineGlow = 3f;
        float endingZoom = 0.82f;
        float startingOutlineGlow = 7.7f;
        float startingBorderThickness = 0.05f;
        float startingZoom = 1f;




        material.SetFloat("_OutlineAlpha", startingAlpha);
        material.SetFloat("_OutlineGlow", startingOutlineGlow);
        material.SetFloat("_OutlineWidth", startingBorderThickness);
        material.SetFloat("_ZoomUvAmount", startingZoom);


        //DOTween.To(() => material.GetFloat("_OutlineWidth"), x => material.SetFloat("_OutlineWidth", x), borderThickness, delay);
        DOTween.To(() => material.GetFloat("_ZoomUvAmount"), x => material.SetFloat("_ZoomUvAmount", x), endingZoom, transitionDuration);
        DOTween.To(() => material.GetFloat("_OutlineGlow"), x => material.SetFloat("_OutlineGlow", x), outlineGlow, transitionDuration);
        DOTween.To(() => material.GetFloat("_OutlineWidth"), x => material.SetFloat("_OutlineWidth", x), borderThickness, delay * 2);
        DOTween.To(() => material.GetFloat("_OutlineWidth"), x => material.SetFloat("_OutlineWidth", x), borderThickness3, transitionDuration - delay).SetDelay(delay);
        DOTween.To(() => material.GetFloat("_OutlineAlpha"), x => material.SetFloat("_OutlineAlpha", x), endingAlpha, transitionDuration).OnComplete(() => OnCardShedAnimationDone?.Invoke(this,EventArgs.Empty));

       




    }
}
