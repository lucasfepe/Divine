using Amazon.DynamoDBv2.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class SaveDeckButton : MonoBehaviour
{
    public static SaveDeckButton Instance { get; private set; }
    public event EventHandler OnSaveExistingDeck;
    public event EventHandler OnSaveDeck;   

    private Decks decks;
    private void Awake()
    {
        Instance = this;
        GetComponent<Button>().onClick.AddListener(() =>
        {
            UpdateDeck();
        });
    }

    private void Start()
    {
        ConfirmSaveButton.Instance.OnConfirmSave += ConfirmSaveButton_OnConfirmSave;
    }
    private void OnDestroy()
    {
        ConfirmSaveButton.Instance.OnConfirmSave -= ConfirmSaveButton_OnConfirmSave;
    }
    private void ConfirmSaveButton_OnConfirmSave(object sender, EventArgs e)
    {
        SaveDecks();
    }

    private void UpdateDeck()
    {
        Deck editedDeck = GetDeckFromEditor();
        decks = DecksManager.Instance.GetDecks();
        IEnumerable<Deck> decksEnumerable = decks.decks.Where(deck => deck.name == DeckTitle.Instance.GetDeckTitle());
        if (decksEnumerable.Any())
        {
            Deck deck = decksEnumerable.First();
            int index = decks.decks.FindIndex(x => x.name ==  DeckTitle.Instance.GetDeckTitle());
            decks.decks[index] = editedDeck;
            OnSaveExistingDeck?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            decks.decks.Add(editedDeck);
            
            //brand new deck
            SaveDecks();
            //triiger caution message saying it will overqrite
        }

    }

    private Deck GetDeckFromEditor()
    {
        Deck deck = new Deck();
        deck.cards = DeckEditorAreaContent.Instance.GetEditedDeckCards();
        deck.name = DeckTitle.Instance.GetDeckTitle();
        return deck;
    }

    public async void SaveDecks()
    {


        foreach (Deck deck in decks.decks)
        {
            foreach (DeckCard card in deck.cards)
            {
            }
        }
        if ( await LambdaManager.Instance.SavePlayerDeckLambda(JsonUtility.ToJson(decks)))
        {
            OnSaveDeck?.Invoke(this, EventArgs.Empty);
        }
        
    }

}
