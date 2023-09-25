using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubterfugeCardUI : CardUI
{
    [SerializeField] private Image energyCostIcon;
    [SerializeField] private Image energyCostBackground;
    [SerializeField] private TextMeshProUGUI energyCostNumberText;

    override protected void CardGenerator_OnCardSOAssigned(object sender, CardGenerator.OnCardSOAssignedEventArgs e)
    {
        if (e.card != card) { return; }
        base.CardGenerator_OnCardSOAssigned(sender, e);

        //SetCircleVisual(card.GetCardSO().subterfugeStatsObjectFromJson.cost,
        //    energyCostNumberText,
        //    energyCostIcon,
        //    energyCostBackground);
    }
}
