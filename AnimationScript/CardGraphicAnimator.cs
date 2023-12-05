using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CardGraphicAnimator : MonoBehaviour
{
    private Material material;
    private Image image;
    private const string OVERLAY_BLEND = "_OverlayBlend";
    private const string SHAKE_SPEED = "_ShakeUvSpeed";


    private void Awake()
    {
        image = GetComponent<Image>();
        material = new Material(image.material);
        image.material = material;


    }
    private void Start()
    {
        if(CardGameManager.Instance != null)
        CardGameManager.Instance.OnEndTurn += CardGameManager_OnEndTurn;
    }

    

    private void CardGameManager_OnEndTurn(object sender, System.EventArgs e)
    {
        StopAttacking();
        StopBeingTarget();
    }

    private Sequence sequence;
    public void BecomeTarget()
    {
        float endOverlayBlend = 0.25f;
        float startOverlayBlend = 0f;
        float transitionDuration = .5f;
        

         sequence = DOTween.Sequence();
        sequence.Append(DOTween.To(() => material.GetFloat(OVERLAY_BLEND), x => material.SetFloat(OVERLAY_BLEND, x), endOverlayBlend, transitionDuration)
        );
        sequence.Append(DOTween.To(() => material.GetFloat(OVERLAY_BLEND), x => material.SetFloat(OVERLAY_BLEND, x), startOverlayBlend, transitionDuration));
        sequence.SetLoops(-1);
        
    }
    public void BecomeAttacking()
    {
        Debug.Log("BecomeAttacking");
        float startShakeSpeed = 15f;
        material.SetFloat(SHAKE_SPEED, startShakeSpeed);
    }
    
    public void StopAttacking()
    {
        float endingShakeSpeed = 0;
        material.SetFloat(SHAKE_SPEED, endingShakeSpeed);
    }
    public void StopBeingTarget()
    {
        float startOverlayBlend = 0f;
        
        sequence.Kill();
        material.SetFloat(OVERLAY_BLEND, startOverlayBlend);

    }
    
}
