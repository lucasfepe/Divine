using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICard
{
   

    void DoneInspectingCard();
    GameAreaEnum GetCardGameArea();
    CardSO GetCardSO();
    void PlayCard();
    bool IsCardMine();
    void InspectCard();
    int GetCardStardust();
    int GetCardLight();
    int GetLifetime();
    bool IsCardDestroyed();
    CardUI GetCardUI();
    void BecomeTargetServerRpc();
    void StopBeingTargetServerRpc(PlayerEnum initiator);
    void SetRarityVisual();
    event EventHandler OnCardSOAssigned;
    event EventHandler OnCardHoverEnter;
    event EventHandler OnCardHoverExit;
}
