using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.Model.Internal.MarshallTransformations;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using UnityEngine;

public class DynamoDB : MonoBehaviour
    {
    public static DynamoDB Instance { get; private set; }
    public const string ACCESS_KEY = "AKIAV76KVGEC6GJ5J2VF";
    public const string SECRET = "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";
        private const string DIVINE_CARD_TABLE_NAME = "DivineCard";
    private const string DIVINE_CARD_TABLE_KEY = "Title";
    private const string PLAYER_DECK_ACTIVE_TABLE_NAME = "PlayerDeckActive";
    private const string PLAYER_DECKS_TABLE_NAME = "PlayerDecks";
    private const string PLAYER_DECK_ACTIVE_TABLE_KEY = "Player";
    private const string PLAYER_DECKS_TABLE_KEY = "Player";
    private const string PLAYER_CARDS_TABLE_KEY = "Player";
    private const string PLAYER_CARDS_TABLE_NAME = "PlayerCards";
    private static AmazonDynamoDBClient client;

    private void Awake()
    {
        Instance = this;
        client = new AmazonDynamoDBClient(
            new BasicAWSCredentials(ACCESS_KEY, SECRET), RegionEndpoint.USEast1);
    }


    public static async Task<GetItemResponse> RetrieveActiveDeck(string player)
    {
        GetItemRequest request = new GetItemRequest
        {
            TableName = PLAYER_DECK_ACTIVE_TABLE_NAME,
            Key = new Dictionary<string, AttributeValue>() { { PLAYER_DECK_ACTIVE_TABLE_KEY, new AttributeValue { S = player } } },
        };
        return await client.GetItemAsync(request);
    }
    public static async Task<GetItemResponse> RetrievePlayerCards()
    {
        GetItemRequest request = new GetItemRequest
        {
            TableName = PLAYER_CARDS_TABLE_NAME,
            Key = new Dictionary<string, AttributeValue>() { { PLAYER_CARDS_TABLE_KEY, new AttributeValue { S = "firstPlayer" } } },
        };
        return await client.GetItemAsync(request);
    }
    public static async Task<GetItemResponse> RetrievePlayerDecks(string player)
    {
        GetItemRequest request = new GetItemRequest
        {
            TableName = PLAYER_DECKS_TABLE_NAME,
            Key = new Dictionary<string, AttributeValue>() { { PLAYER_DECKS_TABLE_KEY, new AttributeValue { S = player } } },
        };
        return await client.GetItemAsync(request);
    }



    public static async Task<UpdateItemResponse> UpdatePlayerDecks(string decks)
    {
        UpdateItemRequest request = new UpdateItemRequest
        {
            TableName = PLAYER_DECKS_TABLE_NAME,
            Key = new Dictionary<string, AttributeValue>() { { PLAYER_DECKS_TABLE_KEY, new AttributeValue { S = "firstPlayer" } } },
            ExpressionAttributeNames = new Dictionary<string, string>()
            {
                {"#D", "Decks"}
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
            {
                {":d", new AttributeValue { S = decks} }
            },
            UpdateExpression = "SET #D = :d"
        };
        return await client.UpdateItemAsync(request);
    }



    public static async Task<GetItemResponse> RetrieveDivineCard(string cardTitle)
    {
        GetItemRequest request = new GetItemRequest
        {
            TableName = DIVINE_CARD_TABLE_NAME,
            Key = new Dictionary<string, AttributeValue>() { { DIVINE_CARD_TABLE_KEY, new AttributeValue { S = cardTitle } } },
        };
        return await client.GetItemAsync(request);
    }
    public static async Task<List<Document>> RetrieveAllCards()
    {
        Table table = Table.LoadTable(client, DIVINE_CARD_TABLE_NAME);
        Search search = table.Scan(new ScanFilter());
        List<Document> documentSet = new List<Document>();
        List<Document> documentSetReturn = new List<Document>();
        do
        {
            documentSet = await search.GetNextSetAsync();
            foreach (Document document in documentSet)
                documentSetReturn.Add(document);
        } while (!search.IsDone);
        return documentSet;
    }
        private static void PrintDocument(Document document)
        {
            foreach (var attribute in document.GetAttributeNames())
            {
                string stringValue = null;
                var value = document[attribute];
                if (value is Primitive)
                    stringValue = value.AsPrimitive().Value.ToString();
                else if (value is PrimitiveList)
                    stringValue = string.Join(",", (from primitive
                                                      in value.AsPrimitiveList().Entries
                                                    select primitive.Value).ToArray());
            }
        }
    }

      
      
    
