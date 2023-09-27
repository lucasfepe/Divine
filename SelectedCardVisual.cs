using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCardVisual : MonoBehaviour
{
    [SerializeField] private GameObject cardGO;
    [SerializeField] private GameObject[] visualGameObjectArray;
    private Canvas canvas;
    private ICard card;

    private void Awake()
    {
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
        
        Hide();
    }

    private void Card_OnCardHover(object sender, EventArgs e)
    {

        if (card.GetCardGameArea() != GameAreaEnum.Inspect) { 
            Show();
        }
    }

    private void Show()
    {
        canvas.overrideSorting = true;
        canvas.sortingOrder = 10;
        foreach (var visualGameObject in visualGameObjectArray)
        {
            if (visualGameObject.transform.parent.TryGetComponent(out InspectHandUI _) && card.GetCardGameArea() == GameAreaEnum.Field) {
                //do not want the inspect balloon to show if it's in the field
                continue;
            }
            visualGameObject.SetActive(true);
            
        }
        


    }
    public void Hide()
    {
        canvas.overrideSorting = false;
        canvas.sortingOrder = 0;
        foreach (var visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(false);
        }
    }
}
