using Amazon.Lambda.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerContainer : MonoBehaviour
{

    private CardUI cardUI;
    private ICard card;
    private TextMeshProUGUI text;
    private Image image;
    private void Awake()
    {
        cardUI = GetComponentInParent<CardUI>();
        
        image = GetComponentInChildren<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        card = cardUI.GetCard();
        card.OnCardSOAssigned += CostContainer_OnCardSOAssigned;
    }
    private void Start()
    {
        
    }
    private void CostContainer_OnCardSOAssigned(object sender, System.EventArgs e)
    {

        if (card.GetCardSO().GiantEffect != null)
        {

            if (card.GetCardSO().GiantEffect.Power != 0)
            {
                if (card.GetCardSO().GiantEffect.Power > 9)
                {
                    text.fontSize = 28;
                    text.transform.localPosition = new Vector2(45, text.transform.localPosition.y);
                }
                else
                {
                    text.fontSize = 36;
                    text.transform.localPosition = new Vector2(39.2f, text.transform.localPosition.y);
                }
                text.text = card.GetCardSO().GiantEffect.Power.ToString();
                ShouldShow();


            }

        }

    }
    public void ShouldShow()
    {
        if (!(card.GetCardGameArea() == GameAreaEnum.CardPreview
            || card.GetCardGameArea() == GameAreaEnum.Inspect)) return;
        if (card.GetCardSO().GiantEffect != null)
        {

            if (card.GetCardSO().GiantEffect.Power != 0)
            {
                Show();

            }
            else
            {
                Hide();
            }

        }
        else
        {
            Hide();
        }
    }
    public void Show()
    {
        image.CrossFadeAlpha(1, 0, true);
        text.alpha = 1;
    }
    public void Hide()
    {
        image.CrossFadeAlpha(0, 0, true);
        text.alpha = 0;
    }
}
