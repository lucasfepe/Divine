//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class CivilizationCard : BaseCard
//{
//    private int oi = 5;

//    public int GetOi()
//    {
//        return oi;
//    }

//    public bool CanPlayCard()
//    {
//        bool canPlayCard = true;
//        foreach(PlayConditionsEnum playcondition in cardSO.civilizationStatsObjectFromJson.playConditions)
//        {
//            switch (playcondition)
//            {
//                case PlayConditionsEnum.Eureka:
//                    canPlayCard = CardGameManager.Instance.HasEureka() ? 
//                        true:
//                         false;
//                break;

//            }
//        }
//        return canPlayCard;
//    }
//}
