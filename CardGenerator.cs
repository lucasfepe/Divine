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
    [SerializeField] private GameObject expertCardLocalTemplate;
    [SerializeField] private GameObject expertCardTemplate;
    public static CardGenerator Instance { get; private set; }
    private List<Amazon.DynamoDBv2.DocumentModel.Document> documents;
    private void Awake()
    {
        Instance = this;
    }

   

    public GameObject GenerateLocalCardFromCardSO()
    {
        return Instantiate(expertCardLocalTemplate);
    }

    
    public BaseCard GenerateCardFromCardSO(CardSO cardSO)
    {
        GameObject cardCreated = null;

        //if (cardSO.cardType == CardTypeEnum.Civilization)
        //{
        //    cardCreated = Instantiate(civilizationCardTemplate, parent);
        //}
        //else if (cardSO.cardType == CardTypeEnum.Expert)
        //{
        cardCreated = Instantiate(expertCardTemplate);
        //}
        //else if (cardSO.cardType == CardTypeEnum.Subterfuge)
        //{
        //    cardCreated = Instantiate(subterfugeCardTemplate, parent);
        //}


        BaseCard baseCard = cardCreated.GetComponent<BaseCard>();

        //Will trigger population of prefab with cardSO data


        return baseCard;
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
                case "Shed":
                    cardSO.Shed = Int32.Parse(divineCard[attribute].N);
                    break;
                case "Rarity":
                    cardSO.Rarity = (RarityEnum)Enum.Parse(typeof(RarityEnum), divineCard[attribute].N);
                    break;
                case "Collection":
                    cardSO.Collection = (CollectionsEnum)Enum.Parse(typeof(CollectionsEnum), divineCard[attribute].N);
                    break;



            }
        }
        return cardSO;
    }

   
}
