//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class CivilizationCardUI : CardUI
//{
//    [SerializeField] private TextMeshProUGUI progressText;
//    [SerializeField] private Image progressBackground;
//    [SerializeField] private Image energyCostIcon;
//    [SerializeField] private Image energyCostBackground;
//    [SerializeField] private TextMeshProUGUI energyCostNumberText;
    
//    //Don't know if inheritance here is the best way to go
//    override protected void CardGenerator_OnCardSOAssigned(object sender, CardGenerator.OnCardSOAssignedEventArgs e)
//    {
//        if (e.card != card) { return; }
//        base.CardGenerator_OnCardSOAssigned(sender, e);
       
//        SetCircleVisual(card.GetCardSO().civilizationStatsObjectFromJson.cost,
//            energyCostNumberText,
//            energyCostIcon,
//            energyCostBackground);
        
//            progressText.text = card.GetCardSO().progress.ToString();
//            switch (card.GetCardSO().cardTalentClass)
//            {
//                case TalentEnum.Art: progressBackground.color = UniversalConstants.GetGreen();break;
//                case TalentEnum.Brawn: progressBackground.color = UniversalConstants.GetRed();break;
//                case TalentEnum.Philosophy: progressBackground.color = UniversalConstants.GetBlue();break;
//                case TalentEnum.Science: progressBackground.color = UniversalConstants.GetGreen(); break;
//            }
        
//    }

    
//}
