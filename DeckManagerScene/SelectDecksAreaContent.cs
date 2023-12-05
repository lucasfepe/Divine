using Mono.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SelectDecksAreaContent : MonoBehaviour
{
    [SerializeField] private GameObject individualDeckUI;
    [SerializeField] private DeleteDeckButton deleteDeckButton;
    [SerializeField] private GoButton goButton;
    public event EventHandler<OnCreateDeckUIEventArgs> OnCreateDeckUI;
    private string activeDeck;
    private static SelectDecksAreaContent Instance;
    public class OnCreateDeckUIEventArgs : EventArgs
    {
        public GameObject newUI;
    }

    private Decks decks;

    private void Start()
    {
        Instance = this;
        DecksManager.Instance.OnFetchDecks += DecksManager_OnFetchDecks;
        if(deleteDeckButton != null)
        deleteDeckButton.OnDeleteDeck += DeleteDeckButton_OnDeleteDeck;


    }

    private void OnDestroy()
    {
        DecksManager.Instance.OnFetchDecks -= DecksManager_OnFetchDecks;
        if (deleteDeckButton != null)
            deleteDeckButton.OnDeleteDeck -= DeleteDeckButton_OnDeleteDeck;
    }

    private void DeleteDeckButton_OnDeleteDeck(object sender, EventArgs e)
    {
        IndividualDeckButton buttonToBeDeleted = GetComponentsInChildren<IndividualDeckButton>()
            .ToList()
            .Where(x => x.GetDeckTitle() == DecksManager.Instance.GetSelectedDeckTitle()).First(); 
        Destroy(buttonToBeDeleted.gameObject);
    }

    

    private async void DecksManager_OnFetchDecks(object sender, System.EventArgs e)
    {
        
        activeDeck = await LambdaManager.Instance.GetPlayerActiveDeckLambda();
        SelectDeckUI.Instance.SetActiveDeck(activeDeck);
        decks = DecksManager.Instance.GetDecks();
        if (decks.decks == null) return;
        //int index = 0;
        decks.decks.ForEach(deck =>
        {
            if (this == null || Instance == null || transform == null) return;
            GameObject deckUI = Instantiate(individualDeckUI, transform);
            
            //deckUI.transform.localPosition = new Vector2(deckUI.transform.localPosition.x, deckUI.transform.localPosition.y - 150 * index++);
            IndividualDeckButton individualDeckButton = deckUI.GetComponent<IndividualDeckButton>();
            if(individualDeckButton == null)
            {
                IndividualDeckButtonMM idbm = deckUI.GetComponent<IndividualDeckButtonMM>();
                idbm.SetDeckTitle(deck.name);
                int cardNumber = deck.cards.Aggregate(0, (acc, deckCard) => acc + deckCard.count);
                idbm.SetCardNumber(cardNumber);
            }
            else
            {
                individualDeckButton.SetDeckTitle(deck.name);
                int cardNumber = deck.cards.Aggregate(0, (acc, deckCard) => acc + deckCard.count);
                individualDeckButton.SetCardNumber(cardNumber);
            }
           
            OnCreateDeckUI?.Invoke(this, new OnCreateDeckUIEventArgs
            {
                newUI = deckUI
            });
        });
        goButton.CheckIfCanEnable();
    }

    
}
