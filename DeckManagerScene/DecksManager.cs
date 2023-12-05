using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecksManager : MonoBehaviour
{
    public static DecksManager Instance { get; private set; }
    private Decks decks;
    private string selectedDeckTitle;
    private AuthenticationManager _authenticationManager;

    public event EventHandler OnChangedSelectedDeck;
    public event EventHandler OnFetchDecks;

    [SerializeField] private Button deleteButton;
    [SerializeField] private Button editButton;

    private TextMeshProUGUI deleteButtonText;
    private TextMeshProUGUI editButtonText;

    private void Awake()
    {
        _authenticationManager = FindObjectOfType<AuthenticationManager>();
        Instance = this;
        if(deleteButton != null)
        deleteButtonText = deleteButton.GetComponentInChildren<TextMeshProUGUI>();
        if(editButton != null)
        editButtonText = editButton.GetComponentInChildren<TextMeshProUGUI>();
        
    }

    private void Start()
    {
        GetDecksFromDynamoDB();
        IndividualDeckButton.OnCreateDeckUIButton += IndividualDeckButton_OnCreateDeckUIButton;
        if(editButton != null)
        DisableButtons();
    }
    private void OnDestroy()
    {
        IndividualDeckButton.OnCreateDeckUIButton -= IndividualDeckButton_OnCreateDeckUIButton;
    }
    private void IndividualDeckButton_OnCreateDeckUIButton(object sender, EventArgs e)
    {
        IndividualDeckButton individualDeckButton = sender as IndividualDeckButton;
        individualDeckButton.OnSelectDeck += IndividualDeckButton_OnSelectDeck;
    }

    private void IndividualDeckButton_OnSelectDeck(object sender, IndividualDeckButton.OnSelectDeckEventArgs e)
    {
        selectedDeckTitle = e.deckTitle;
        OnChangedSelectedDeck?.Invoke(this, EventArgs.Empty);
        EnableButtons();
    }

    private async void GetDecksFromDynamoDB()
    {
        string playerDecksResponse = "";
#if !DEDICATED_SERVER
        playerDecksResponse = await LambdaManager.Instance.GetPlayerDecksLambda();
        decks = JsonUtility.FromJson<Decks>(playerDecksResponse);
        OnFetchDecks?.Invoke(this, EventArgs.Empty);
#endif

    }

    public Decks GetDecks()
    {
        return decks;
    }

    public string GetSelectedDeckTitle()
    {
        return selectedDeckTitle;
    }

    public async void DeleteDeck(string title)
    {
        List<Deck> decksAfterDeletion = decks.decks.Where(deck => deck.name != title).ToList();
        decks.decks = decksAfterDeletion;
        foreach (Deck deck in decks.decks)
        {
            foreach (DeckCard card in deck.cards)
            {
            }
        }
        string decksJson = JsonUtility.ToJson(decks);
        bool isActiveDeck = false;
        string playerActiveDeck = await LambdaManager.Instance.GetPlayerActiveDeckLambda();
        if (playerActiveDeck == title)
            isActiveDeck = true;
        await LambdaManager.Instance.DeletePlayerDeckLambda(decksJson, isActiveDeck);
        


        selectedDeckTitle = null;
        OnChangedSelectedDeck?.Invoke(this, EventArgs.Empty);
        DisableButtons();

    }

    private void EnableButtons()
    {
        if(deleteButton != null)
        deleteButton.interactable = true;
        if(editButton != null)
        editButton.interactable = true;

        float enabledButtonTextAlpha = 1f;
        if(deleteButtonText != null)
        deleteButtonText.alpha = enabledButtonTextAlpha;
        if(editButtonText != null)
        editButtonText.alpha = enabledButtonTextAlpha;
    }
    private void DisableButtons()
    {
        deleteButton.interactable = false;
        editButton.interactable = false;


        float disabledButtonTextAlpha = .5f;
        deleteButtonText.alpha = disabledButtonTextAlpha;
        editButtonText.alpha = disabledButtonTextAlpha;
    }
}
