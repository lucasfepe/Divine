using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static PlayerHand;
using static UnityEngine.Rendering.DebugUI;

public class CardGenerator : MonoBehaviour
{
    [SerializeField] private BaseCard subterfugeCardTemplate;
    [SerializeField] private BaseCard expertCardTemplate;
    [SerializeField] private BaseCard civilizationCardTemplate;
    public static CardGenerator Instance { get; private set; }
    private List<Amazon.DynamoDBv2.DocumentModel.Document> documents;
    private void Awake()
    {
        Instance = this;
    }

    public event EventHandler<OnCardSOAssignedEventArgs> OnCardSOAssigned;
    public class OnCardSOAssignedEventArgs : EventArgs
    {
        public BaseCard card;
    }

    public BaseCard GenerateCardFromCardSO(CardSO cardSO, Transform parent)
    {
        BaseCard cardCreated = null;

        //if (cardSO.cardType == CardTypeEnum.Civilization)
        //{
        //    cardCreated = Instantiate(civilizationCardTemplate, parent);
        //}
        //else if (cardSO.cardType == CardTypeEnum.Expert)
        //{
            cardCreated = Instantiate(expertCardTemplate, parent);
        //}
        //else if (cardSO.cardType == CardTypeEnum.Subterfuge)
        //{
        //    cardCreated = Instantiate(subterfugeCardTemplate, parent);
        //}
        
        cardCreated.GetComponent<BaseCard>().SetCardSO(cardSO);

        
        //Will trigger population of prefab with cardSO data

        OnCardSOAssigned?.Invoke(this, new OnCardSOAssignedEventArgs
        {
            card = cardCreated.GetComponent<BaseCard>()
        });
        return cardCreated;
    }

    internal async Task<CardSO> CardNameToCardSO(string name)
    {
        GetItemResponse getItemResponse = await DynamoDB.RetrieveDivineCard(name);
        Dictionary<string, AttributeValue> divineCard = getItemResponse.Item;
        CardSO cardSO = ScriptableObject.CreateInstance<CardSO>();
        foreach (string attribute in divineCard.Keys)
        {

            switch (attribute)
            {
                case "ImageURL":
                    cardSO.ImageURL = divineCard[attribute].S.ToString();
                    break;
                case "RedGiantImageURL":
                    cardSO.RedGiantImageURL = divineCard[attribute].S.ToString();
                    break;
                case "Title":
                    cardSO.Title = divineCard[attribute].S.ToString();
                    break;
                case "Color":
                    cardSO.Rank = (StarClassEnum)Enum.Parse(typeof(StarClassEnum), divineCard[attribute].S);
                    break;
                case "Lifetime":
                    cardSO.Lifetime = Int32.Parse(divineCard[attribute].N);
                    break;
                case "Stardust":
                    cardSO.Stardust = Int32.Parse(divineCard[attribute].N);
                    break;
                case "Light":
                    cardSO.Light = Int32.Parse(divineCard[attribute].N);
                    break;
                case "RedGiantEffect":
                    cardSO.GiantEffect = JsonUtility.FromJson<GiantEffect>(divineCard[attribute].S);
                    break;


            }
        }
        return cardSO;
    }
}
