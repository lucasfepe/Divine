using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectHandUI : MonoBehaviour
{
    [SerializeField] private Button inspectButton;
    [SerializeField] private GameObject inspectContainer;
    private BaseCardLocal card;

    private void Awake()
    {
        card = transform.parent.GetComponentInParent<BaseCardLocal>();
        inspectButton.onClick.AddListener(() =>
        {
            card.InspectCard();
        });
    }
    private void Start()
    {
        card.OnCardHoverEnter += Card_OnCardHoverEnter;
        card.OnCardHoverExit += Card_OnCardHoverExit;
    }

    private void Card_OnCardHoverExit(object sender, EventArgs e)
    {
        Hide();
    }

    private void Card_OnCardHoverEnter(object sender, EventArgs e)
    {
        if (card.GetCardGameArea() == GameAreaEnum.Inspect) { return; }
        Show();
    }

    private void OnMouseOver()
    {
        if (card.GetCardGameArea() == GameAreaEnum.Inspect) { return; }
        Show();
    }
    private void OnMouseExit()
    {
        Hide();
    }
    public void Hide() { 
    inspectContainer.SetActive(false);
    }
    private void Show()
    {
        inspectContainer.SetActive(true);
    }
}
