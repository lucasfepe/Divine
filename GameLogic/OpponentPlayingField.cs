using Amazon.DynamoDBv2.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static PlayerHand;

public class OpponentPlayingField : MonoBehaviour
{
    public static OpponentPlayingField Instance { get; private set; }
    [SerializeField] private List<Transform> expertCardPositions;
    [SerializeField] private List<Transform> civilizationCardPositions;
    [SerializeField] private Transform subterfugeCardPosition;
    [SerializeField] private BaseCard subterfugeCardTemplate;
    

    private float subterfugeCardDeleteDelayTimer = 0f;
    private float subterfugeCardDeleteDelayTimerMax = 2f;
    private bool subterfugeCardPlayed = false;
    private BaseCard subterfugeCard;
    
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        DivineMultiplayer.Instance.OnFieldCardsChanged += DivineMultiplayer_OnFieldCardsChanged;

    }

   

    private void DivineMultiplayer_OnFieldCardsChanged(object sender, DivineMultiplayer.OnFieldCardsChangedEventArgs e)
    {
        if ((e.playerEnum == PlayerEnum.PlayerOne && Player.Instance.IAm() == PlayerEnum.PlayerTwo)
            ||
            (e.playerEnum == PlayerEnum.PlayerTwo && Player.Instance.IAm() == PlayerEnum.PlayerOne)
            )
        {
            switch(e.cardType)
            {
                case CardTypeEnum.Expert:
                    //I need a cardSO from the card name
                    GetCardSOFromTitle(e.cardName, CardTypeEnum.Expert);
                    break;
                case CardTypeEnum.Civilization:
                    //I need a cardSO from the card name
                    GetCardSOFromTitle(e.cardName, CardTypeEnum.Civilization);
                    break;
                case CardTypeEnum.Subterfuge:
                    GetCardSOFromTitle(e.cardName, CardTypeEnum.Subterfuge);
                    break;
            }
        }
    }

    private async void GetCardSOFromTitle(string title, CardTypeEnum cardType) {

        
        CardSO cardSO = await CardGenerator.Instance.CardNameToCardSO(title);
        //generate card from cardSO
        
        Transform emptySlot = null;
        switch (cardType)
        {
            case CardTypeEnum.Expert:
                foreach (Transform t in expertCardPositions)
                {
                    if (t.childCount == 0)
                    {
                        emptySlot = t; break;
                    }
                }
                break;
            case CardTypeEnum.Civilization:
                foreach (Transform t in civilizationCardPositions)
                {
                    if (t.childCount == 0)
                    {
                        emptySlot = t; break;
                    }
                }
                break;
            case CardTypeEnum.Subterfuge:
                emptySlot = subterfugeCardPosition;
                break;
        }
        BaseCard cardCreated = CardGenerator.Instance.GenerateCardFromCardSO(cardSO, emptySlot);
        cardCreated.SetCardOwner(Player.Instance.OpponentIs());
        //if (cardSO.cardType == CardTypeEnum.Civilization)
        //{
        //    RectTransform rectTransform = 
        //        cardCreated.gameObject.GetComponent<RectTransform>();
        //    rectTransform.localScale = new Vector2(UniversalConstants.FIELD_CARD_SCALE, UniversalConstants.FIELD_CARD_SCALE);
        //}
        //else if (cardSO.cardType == CardTypeEnum.Expert)
        //{
            RectTransform rectTransform = 
                cardCreated.gameObject.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector2(UniversalConstants.FIELD_CARD_SCALE, UniversalConstants.FIELD_CARD_SCALE);
        //}
        //else if (cardSO.cardType == CardTypeEnum.Subterfuge)
        //{
        //    subterfugeCardPlayed = true;
        //    subterfugeCard = cardCreated;
        //}
        
        cardCreated.SetCardGameArea(GameAreaEnum.OpponentField);
        
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
    public List<BaseCard> GetAllOpponentExpertCards()
    {
        List<BaseCard> expertCards = new List<BaseCard>();
        foreach (Transform expertCardPosition in expertCardPositions)
        {
            if (expertCardPosition.childCount > 0)
            {
                //Should only have one!
                expertCards.Add(expertCardPosition.GetChild(0).GetComponent<BaseCard>());
            }
        }
        return expertCards;
    }
}
