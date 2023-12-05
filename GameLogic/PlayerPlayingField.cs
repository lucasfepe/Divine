using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerPlayingField : NetworkBehaviour
{
    public static PlayerPlayingField Instance { get; private set; }
    [SerializeField] private List<Transform> expertCardPositions;
    [SerializeField] private List<Transform> civiliationCardPositions;
    [SerializeField] private Transform subterfugeCardPosition;

   
    public event EventHandler OnPlayCard;
    private void Awake()
    {
        Instance = this;
    }

    public List<Transform> GetExpertCardPositions()
    {
        return expertCardPositions;
    }
    public void PlaceCardInField()
    {
       
    }
    private bool HasOpenExpertCardPosition()
    {
        foreach (Transform t in expertCardPositions)
        {
            if (t.childCount == 0)
            {
                return true;
            }
        }
        return false;
    }
    public Transform GetFirstOpenExpertCardPosition()
    {
        foreach (Transform t in expertCardPositions)
        {
            if (t.childCount == 0)
            {
                return t;
            }
        }
        return null;
    }
    public void TryPlayCard(ICard card)
    {
        if (!CardGameManager.Instance.IsMyTurn())
        {
            return;
        }
        if (card.GetCardGameArea() != GameAreaEnum.Hand)
        {
            return;
        }
        CardSO cardSO = card.GetCardSO();
        
            if (CardGameManager.Instance.CanPlayExpertCardThisTurn())
            {
            if (HasOpenExpertCardPosition()) {
                //CardGameManager.Instance.PlayedExpertCardThisTurn();
                OnPlayCard?.Invoke(this, EventArgs.Empty);
                card.PlayCard();
            }

        }

    }

   
    public List<BaseCard> GetAllPlayerExpertCards()
    {
        List<BaseCard> expertCards = new List<BaseCard>();
        int position = -1;
        if (InspectCardUI.Instance.IsInspectingMyCard())
        {
            if (InspectCardUI.Instance.GetActiveCard() is ExpertCard)
            {
                position = expertCardPositions.FindIndex(x => x == ((BaseCard)InspectCardUI.Instance.GetActiveCard()).GetPreviousParent());
                
            }
        }
        int iterationCount = 0;
        foreach (Transform expertCardPosition in expertCardPositions)
        {
            if (expertCardPosition.childCount > 0)
            {
                //Should only have one!
                expertCards.Add(expertCardPosition.GetChild(0).GetComponent<BaseCard>());
            }else if (iterationCount == position)
            {
                expertCards.Add(((BaseCard)InspectCardUI.Instance.GetActiveCard()));
            }
            iterationCount++;
        }
        
        return expertCards;
    }
}
