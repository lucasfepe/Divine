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

    private float subterfugeCardDeleteDelayTimer = 0f;
    private float subterfugeCardDeleteDelayTimerMax = 2f;
    private bool subterfugeCardPlayed = false;
    private GameObject subterfugeCard;
    public event EventHandler OnPlayCard;
    private void Awake()
    {
        Instance = this;
    }

    //private void Update()
    //{

    //    if (subterfugeCardPlayed)
    //    {
    //        subterfugeCardDeleteDelayTimer += Time.deltaTime;
    //        if (subterfugeCardDeleteDelayTimer >= subterfugeCardDeleteDelayTimerMax)
    //        {
    //            subterfugeCardDeleteDelayTimer = 0;
    //            subterfugeCardPlayed = false;
    //            Destroy(subterfugeCard);

    //        }
    //    }
    //}
    public void PlayCard(BaseCard card)
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
        //if (cardSO.cardType == CardTypeEnum.Expert)
        //{
            if (CardGameManager.Instance.CanPlayExpertCardThisTurn())
            {
                CardGameManager.Instance.PlayedExpertCardThisTurn();
                foreach (Transform t in expertCardPositions)
                {
                    if (t.childCount == 0)
                    {
                        OnPlayCard?.Invoke(this, EventArgs.Empty);
                        card.SetCardGameArea(GameAreaEnum.Field);
                        card.transform.SetParent(t);
                        AddToFieldCardList(card.GetCardSO().Title);
                        RectTransform rectTransform = card.transform.gameObject.GetComponent<RectTransform>();
                        rectTransform.position = t.position;
                        rectTransform.localScale = new Vector2(UniversalConstants.FIELD_CARD_SCALE, UniversalConstants.FIELD_CARD_SCALE);
                        break;
                    }
                }
            }

        //}
        //else if (cardSO.cardType == CardTypeEnum.Civilization)
        //{
        //    TalentValues cost = cardSO.civilizationStatsObjectFromJson.cost;
        //    if (((CivilizationCard)card).CanPlayCard()
        //        && CardGameManager.Instance.bankedArt >= (cost.art)
        //        && CardGameManager.Instance.bankedBrawn >= (cost.brawn)
        //        && CardGameManager.Instance.bankedPhilosophy >= (cost.philosophy)
        //        && CardGameManager.Instance.bankedScience >= (cost.science))
        //    {

        //        CardGameManager.Instance.bankedArt -= cost.art;
        //        CardGameManager.Instance.bankedBrawn -= cost.brawn;
        //        CardGameManager.Instance.bankedPhilosophy -= cost.philosophy;
        //        CardGameManager.Instance.bankedScience -= cost.science;
        //        CardGameManager.Instance.UpdateBankedEnergy();

                
        //            foreach (Transform t in civiliationCardPositions)
        //            {
        //                if (t.childCount == 0)
        //                {
        //                    OnPlayCard?.Invoke(this, EventArgs.Empty);
        //                    card.SetCardGameArea(GameAreaEnum.Field);
        //                    card.transform.SetParent(t);
        //                    AddToFieldCardList(card.GetCardSO().title, card.GetCardSO().cardType);
        //                    RectTransform rectTransform = card.transform.gameObject.GetComponent<RectTransform>();
        //                    rectTransform.position = t.position;
        //                    rectTransform.localScale = new Vector2(UniversalConstants.FIELD_CARD_SCALE, UniversalConstants.FIELD_CARD_SCALE);
        //                    //this definitely doesn't belong here
        //                    if (cardSO.civilizationStatsObjectFromJson.playConditions.Contains(PlayConditionsEnum.Eureka))
        //                    {
        //                        CardGameManager.Instance.PlayEurekaCard();
        //                    }
        //                    break;
        //                }
                    
        //        };
                

                

        //    }
        //}
        //else if (cardSO.cardType == CardTypeEnum.Subterfuge)
        //{
        //    TalentValues cost = cardSO.subterfugeStatsObjectFromJson.cost;
        //    if (CardGameManager.Instance.bankedArt >= (cost.art)
        //        && CardGameManager.Instance.bankedBrawn >= (cost.brawn)
        //        && CardGameManager.Instance.bankedPhilosophy >= (cost.philosophy)
        //        && CardGameManager.Instance.bankedScience >= (cost.science))
        //    {
        //        card.SetCardGameArea(GameAreaEnum.Field);
        //        subterfugeCardPlayed = true;
        //        subterfugeCard = card.gameObject;
        //        CardGameManager.Instance.bankedArt -= cost.art;
        //        CardGameManager.Instance.bankedBrawn -= cost.brawn;
        //        CardGameManager.Instance.bankedPhilosophy -= cost.philosophy;
        //        CardGameManager.Instance.bankedScience -= cost.science;
        //        CardGameManager.Instance.UpdateBankedEnergy();
        //        ((SubterfugeCard)card).PlayCard();

        //        OnPlayCard?.Invoke(this, EventArgs.Empty);

        //        card.transform.SetParent(subterfugeCardPosition);
        //        AddToFieldCardList(card.GetCardSO().title, card.GetCardSO().cardType);
        //        RectTransform rectTransform = card.transform.gameObject.GetComponent<RectTransform>();
        //        rectTransform.position = subterfugeCardPosition.position;

        //        if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
        //        {
        //            SetPlayerTwoProgressServerRpc(card.GetCardSO().opponentProgress ?? default(int));
        //        }
        //        else if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        //        {
        //            SetPlayerOneProgressServerRpc(card.GetCardSO().opponentProgress ?? default(int));
        //        }



        //        //rectTransform.localScale = new Vector2(1.1f, 1.1f);


        //    }
        //}
    }


    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerOneProgressServerRpc(int change)
    {
        DivineMultiplayer.Instance.playerOneProgress.Value += change;
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerTwoProgressServerRpc(int change)
    {
        DivineMultiplayer.Instance.playerTwoProgress.Value += change;
    }

    //Will update other player's opponent field
    private void AddToFieldCardList(string cardName)
    {
        if (Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            
                    AddToPlayerOneExpertCardListServerRpc(cardName);
                   
            

        }
        else if (Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
           
                    AddToPlayerTwoExpertCardListServerRpc(cardName);
                  
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddToPlayerTwoExpertCardListServerRpc(string cardName)
    {
        DivineMultiplayer.Instance.fieldExpertCardsPlayerTwo.Add(cardName);
    }
    [ServerRpc(RequireOwnership = false)]
    private void AddToPlayerTwoCivilizationCardListServerRpc(string cardName)
    {
        DivineMultiplayer.Instance.fieldCivilizationCardsPlayerTwo.Add(cardName);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerTwoSubterfugeCardServerRpc(string cardName)
    {
        DivineMultiplayer.Instance.fieldSubterfugeCardPlayerTwo.Value = cardName;
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddToPlayerOneExpertCardListServerRpc(string cardName)
    {
        DivineMultiplayer.Instance.fieldExpertCardsPlayerOne.Add(cardName);
    }
    [ServerRpc(RequireOwnership = false)]
    private void AddToPlayerOneCivilizationCardListServerRpc(string cardName)
    {
        DivineMultiplayer.Instance.fieldCivilizationCardsPlayerOne.Add(cardName);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerOneSubterfugeCardServerRpc(string cardName)
    {
        DivineMultiplayer.Instance.fieldSubterfugeCardPlayerOne.Value = cardName;
    }

    public int GetNumberExpertCardsInPlayerField()
    {
        int numberExpertCards = 0;
        foreach (Transform t in expertCardPositions)
        {
            if (t.childCount > 0)
            {
                numberExpertCards++;
            }
        }
        if (InspectCardUI.Instance.IsInspectingMyCard())
        {
            numberExpertCards++;
        } 
        return numberExpertCards;
    }
    public List<BaseCard> GetAllPlayerExpertCards()
    {
        List<BaseCard> expertCards = new List<BaseCard>();
        int position = -1;
        if (InspectCardUI.Instance.IsInspectingMyCard())
        {
            if (InspectCardUI.Instance.GetActiveCard() is ExpertCard)
            {
                position = expertCardPositions.FindIndex(x => x == InspectCardUI.Instance.GetActiveCard().GetPreviousParent());
                
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
                expertCards.Add(InspectCardUI.Instance.GetActiveCard());
            }
            iterationCount++;
        }
        
        return expertCards;
    }
}
