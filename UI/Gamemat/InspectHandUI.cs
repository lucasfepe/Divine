using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectHandUI : MonoBehaviour
{
    [SerializeField] private Button inspectButton;
    [SerializeField] private GameObject inspectContainer;
    private BaseCard card;

    private void Awake()
    {
        card = transform.parent.GetComponentInParent<BaseCard>();
        inspectButton.onClick.AddListener(() =>
        {
            card.InspectCard();
        });
    }

    private void OnMouseOver()
    {
        if (card.GetCardGameArea() == GameAreaEnum.Field
            || card.GetCardGameArea() == GameAreaEnum.Inspect) { return; }
        inspectContainer.SetActive(true);
    }
    private void OnMouseExit()
    {
        inspectContainer.SetActive(false);
    }
    public void Hide() { 
    inspectContainer.gameObject.SetActive(false);
    }
}
