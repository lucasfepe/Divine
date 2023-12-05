using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectedCardVisual : MonoBehaviour
{
    [SerializeField] private GameObject cardGO;
    [SerializeField] private GameObject[] visualGameObjectArray;
    private Canvas canvas;
    private ICard card;
    private Animator animator;
    private bool isSelectingtarget = false;
    private const string CAN_USE_SKILL = "canUseSkill";
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        card = cardGO.GetComponent<ICard>();
    }

    private void Start()
    {
        card.OnCardHoverEnter += Card_OnCardHover;
        card.OnCardHoverExit += Card_OnCardHoverExit;
        canvas = cardGO.GetComponentInChildren<Canvas>();
    }

    private void Card_OnCardHoverExit(object sender, EventArgs e)
    {
        if (card is BaseCard && ((BaseCard)card).IsCardDestroyed())
        {
            //need this for when target is absorbed
            return;
        }
        Hide();
    }

    private void Card_OnCardHover(object sender, EventArgs e)
    {
        if (card.GetCardGameArea() != GameAreaEnum.Inspect 
            && !InspectCardUI.Instance.IsInspectingCard()
            && !animator.GetBool(CAN_USE_SKILL)) {
            Show();
        }
    }
    public void LastingHighlight()
    {
        isSelectingtarget = true;
        animator.SetBool(CAN_USE_SKILL, false);
        Show();
    }
    public void EndLastingHighlight()
    {
        isSelectingtarget = false;
        Hide();
    }
    private void Show()
    {
        if (!card.IsCardDestroyed()) { 
        animator.ResetTrigger("Hide");
            
        }
        if (card is BaseCard && ((BaseCard)card).IsCardDestroyed())
        {
            //need this for when target is absorbed
            return;
        }
        animator.SetTrigger("Show");
        if (SceneManager.GetActiveScene().name == SceneLoader.Scene.DeckEditorScene.ToString()) return;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 10;
        
            
            

        
        


    }
    public void Hide()
    {
        if (isSelectingtarget) return;
        if (card is BaseCard && ((BaseCard)card).IsCardDestroyed())
        {
            //need this for when target is absorbed
            return;
        }
        if (animator.GetBool(CAN_USE_SKILL)) return;
        canvas.overrideSorting = false;
        canvas.sortingOrder = 0;
        //foreach (var visualGameObject in visualGameObjectArray)
        //{
        //    visualGameObject.SetActive(false);
        //}
        
        animator.SetTrigger("Hide");
    }
}
