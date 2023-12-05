using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CostContainer : MonoBehaviour
{
    private CardUI cardUI;
    private ICard card;
    private TextMeshProUGUI text;
    private Image image;
    private bool started = false;
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
        
        started = true;
    }

    private void CostContainer_OnCardSOAssigned(object sender, System.EventArgs e)
    {
        if (card.GetCardSO().GiantEffect != null)
        {
            
            if(card.GetCardSO().GiantEffect.StardustCost != 0)
            {
                if (card.GetCardSO().GiantEffect.StardustCost > 99)
                {
                    text.fontSize = 28;
                    text.transform.localPosition = new Vector2(52, text.transform.localPosition.y);
                }
                else if(card.GetCardSO().GiantEffect.StardustCost > 9)
                {
                    text.fontSize = 28;
                    text.transform.localPosition = new Vector2(45, text.transform.localPosition.y);
                }
                text.text = card.GetCardSO().GiantEffect.StardustCost.ToString();
            }else if(card.GetCardSO().GiantEffect.PassiveActive == PassiveActiveEnum.Passive)
            {
                text.text = "P";
                text.color = Color.cyan;
                text.alpha = 0;
            }
            if(card is ExpertCard)
            SetSkillStardustCost();

            ShouldShow();
        }
    }
    public void SetSkillStardustCost()
    {
        if(card.GetCardSO() != null && card.GetCardSO().GiantEffect.PassiveActive == PassiveActiveEnum.Active)
        {
            int originalValue = card.GetCardSO().GiantEffect.StardustCost;
            int newvlaue = ((ExpertCard)card).GetSkillStardustCost();
            text.text = newvlaue.ToString();
            if (newvlaue > originalValue) { 
                text.color = Color.red;
            }
            else if (newvlaue < originalValue) { 
                text.color = Color.green;
            }
            else { 
                text.color = Color.white;
            }
            if (newvlaue > 99)
            {
                text.fontSize = 28;
                text.transform.localPosition = new Vector2(52, text.transform.localPosition.y);
            }
            else if (newvlaue > 9)
            {
                text.fontSize = 28;
                text.transform.localPosition = new Vector2(45, text.transform.localPosition.y);
            }
            else
            {
                text.fontSize = 36;
                text.transform.localPosition = new Vector2(39.2f, text.transform.localPosition.y);
            }
            text.text = newvlaue.ToString();
            ShouldShow();
        }
    }
    public void ShouldShow()
    {
        //Debug.Log("started: " + started);
        
        //if ((!started)) return;
        if (!(card.GetCardGameArea() == GameAreaEnum.CardPreview
            || card.GetCardGameArea() == GameAreaEnum.Inspect)) return;

        if (card.GetCardSO().GiantEffect != null)
        {
            Show();


        }
        else
        {
            Hide();
        }
    }
    public void Show()
    {
        image.CrossFadeAlpha(1,0,true);
        text.alpha = 1;
    }
    public void Hide()
    {
        image.CrossFadeAlpha(0, 0, true);
        text.alpha = 0;
    }
}
