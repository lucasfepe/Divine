//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using Unity.Netcode;
//using Unity.VisualScripting;
//using UnityEngine;


//public class SubterfugeCard : BaseCard
//{
//    new private void Start()
//    {
//    }

//    override public void TargetSelected(BaseCard targetCard)
//    {
//        switch (cardSO.subterfugeStatsObjectFromJson.effect)
//        {
//            case EffectTypeEnum.Pacify:
//                DivineMultiplayer.Instance.PacifyServerRpc(OpponentPlayingField.Instance.GetAllOpponentExpertCards().FindIndex(x => targetCard == x),Player.Instance.OpponentIs());
//                break;
//        }
        
//    }
//    public override ViableTarget GetViableTarget()
//    {
//        return cardSO.subterfugeStatsObjectFromJson.viableTarget;
//    }



//    public void PlayCard()
//    {
//        switch (cardSO.subterfugeStatsObjectFromJson.effect)
//        {
//            case EffectTypeEnum.Pacify:
//                CardGameManager.Instance.BeginSelectingTarget(this);
//                break;
//        }
//    }

     
    
   
//}
