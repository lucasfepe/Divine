using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayDeckAreaContent : MonoBehaviour
{
    [SerializeField] private GameObject expertCardLocalPrefab;
    private RectTransform rectTransform;
    

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        
    }
    private void Start()
    {
        IndividualDeckButton.OnCreateDeckUIButton += IndividualDeckButton_OnCreateDeckUIButton;
        DecksManager.Instance.OnChangedSelectedDeck += DecksManager_OnChangedSelectedDeck; 
    }
    private void OnDestroy()
    {
        IndividualDeckButton.OnCreateDeckUIButton -= IndividualDeckButton_OnCreateDeckUIButton;
        DecksManager.Instance.OnChangedSelectedDeck -= DecksManager_OnChangedSelectedDeck; ;
    }

    private void DecksManager_OnChangedSelectedDeck(object sender, System.EventArgs e)
    {
        if(DecksManager.Instance.GetSelectedDeckTitle() == null || DecksManager.Instance.GetSelectedDeckTitle() == "") {
            Clear();
        }
    }

    private void IndividualDeckButton_OnCreateDeckUIButton(object sender, System.EventArgs e)
    {
        IndividualDeckButton individualDeckButton = sender as IndividualDeckButton;
        individualDeckButton.OnSelectDeck += IndividualDeckButton_OnSelectDeck;
        
    }
    private void Clear()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void IndividualDeckButton_OnSelectDeck(object sender, IndividualDeckButton.OnSelectDeckEventArgs e)
    {
        Clear();
        Decks decks = DecksManager.Instance.GetDecks();
        List<Deck> listOfDecks = decks.decks;
        Deck deck = listOfDecks.Find(x => x.name == e.deckTitle);
        deck.cards.ForEach(card =>
        {
            InstantiateDeckCard(card);
        });
        

    }

    private async void InstantiateDeckCard(DeckCard card)
    {
        List<ExpertCardLocal> expertCardLocalList = new List<ExpertCardLocal>();
        for (int i = 0; i < card.count; i++)
        {
            GameObject cardUI = Instantiate(expertCardLocalPrefab, transform);
            cardUI.SetActive(false);
            ExpertCardLocal expertCardLocal = cardUI.GetComponent<ExpertCardLocal>();
            expertCardLocalList.Add(expertCardLocal);
            Image image = ((ExpertCardUI)expertCardLocal.GetCardUI()).GetLightIcon();
            image.material = null;
            
        }
        CardSO cardSO = await CardGenerator.Instance.CardNameToCardSO(card.title);
        for (int i = 0; i < card.count; i++)
        {
            expertCardLocalList.ForEach(x => { 
                x.SetCardSO(cardSO);
                x.SetCardGameArea(GameAreaEnum.Hand);
                x.gameObject.SetActive(true); 
            });
           
        }


    }
}
