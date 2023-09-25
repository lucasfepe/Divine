using Amazon.DynamoDBv2.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using static DivineMultiplayer;

public class PlayerDeck : NetworkBehaviour
{
    public List<CardSO> playerDeck = new List<CardSO>();
    public static PlayerDeck Instance { get; private set; }
    
    public event EventHandler OnFetchActiveDeck;
    public int totalCards;
    
    private void Awake()
    {
        Instance = this;
    }

    public async void GetActiveDeck()
    {
        //Retrieve active deck name
        GetItemResponse playerActiveDeckResponse = await DynamoDB.RetrieveActiveDeck("firstPlayer");
        Dictionary<string, AttributeValue> playerActiveDeckItem = playerActiveDeckResponse.Item;
        playerActiveDeckItem.TryGetValue("ActiveDeck", out AttributeValue value);
        string playerActiveDeck = value.S;

        //retrive decks for this player
        GetItemResponse playerDecksResponse = await DynamoDB.RetrievePlayerDecks("firstPlayer");

        playerDecksResponse.Item.TryGetValue("Decks", out AttributeValue decksAttributeValue);
        string decks = decksAttributeValue.S;
        //Retrieve the active deck 
        Decks decksFromJson = JsonUtility.FromJson<Decks>(decks);
        List<DeckCard> cards = decksFromJson.decks
            .Where(x => x.name == playerActiveDeck)
            .First().cards;
        //count how many cards
        totalCards = cards.Aggregate(0, (acc, x) => acc + x.count);

        
        
        //from card title to DivineCards item

        foreach (DeckCard deckCard in cards)
        {
            
            for (int i = 0; i < deckCard.count; i++)
            {
                CardSO cardSO = await CardGenerator.Instance.CardNameToCardSO(deckCard.title);

                playerDeck.Add(cardSO);
            }
        }
        Shuffle(playerDeck);
        OnFetchActiveDeck?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership =false)]
    public void SetPlayerOneCardCountServerRpc(int totalCards)
    {
        DivineMultiplayer.Instance.playerOneDeckCards.Value = totalCards;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerTwoCardCountServerRpc(int totalCards)
    {
        DivineMultiplayer.Instance.playerTwoDeckCards.Value = totalCards;
    }

    public void Shuffle<T>( IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
