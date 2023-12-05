using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class DeckEditorAreaContent : MonoBehaviour
{
    public static DeckEditorAreaContent Instance { get; private set; }
    public event EventHandler OnDeckEditorCardChange;
    private string deckToEdit;
    private bool ready1 = false;
    private bool ready2 = false;
    private List<DeckCard> cards = new List<DeckCard>();
    [SerializeField] private AddButton addBtn;
    [SerializeField] private Transform cardHolderTransform;
    [SerializeField] private RemoveButton removeButton;
    public event EventHandler OnDeckToEditCardsAssigned;

    private void Awake()
    {
        Instance = this;
        deckToEdit = DeckManagerStatic.GetDeckToEdit();
        addBtn.OnAdd += AddBtn_OnAdd;
        removeButton.OnRemove += RemoveButton_OnRemove;

    }

    private void RemoveButton_OnRemove(object sender, EventArgs e)
    {
        GameObject previewCardGO = cardHolderTransform.GetChild(0).gameObject;
        BaseCardLocal previewCardBaseCardLocal = previewCardGO.GetComponent<BaseCardLocal>();
        string previewCardTitle = previewCardBaseCardLocal.GetCardSO().Title;
        List<BaseCardLocal> cardsInDeck = transform.GetComponentsInChildren<BaseCardLocal>().ToList();
        BaseCardLocal cardInDeck = cardsInDeck.Find(x => x.GetCardSO().Title == previewCardTitle);
        cardInDeck.SetCounterText((Int32.Parse(cardInDeck.GetCounterText()) - 1).ToString());
        if(cardInDeck.GetCounterText() == "0")
        {
            removeButton.DisableButton();
            Destroy(cardInDeck.gameObject);
        }else
        {
            addBtn.EnableButton();
        }
    }
    private void AddBtn_OnAdd(object sender, System.EventArgs e)
    {
        GameObject previewCardGO = cardHolderTransform.GetChild(0).gameObject;
        BaseCardLocal previewCardBaseCardLocal = previewCardGO.GetComponent<BaseCardLocal>();
        string previewCardTitle = previewCardBaseCardLocal.GetCardSO().Title;

        List<BaseCardLocal> cardsInDeckEditor = transform.GetComponentsInChildren<BaseCardLocal>().ToList();

        if (cardsInDeckEditor.Any(x => x.GetCardSO().Title == previewCardTitle))
        {
            List<BaseCardLocal> baseCardLocals = transform.GetComponentsInChildren<BaseCardLocal>().ToList();
            BaseCardLocal cardInDeck = baseCardLocals.Find(x => x.GetCardSO().Title == previewCardBaseCardLocal.GetCardSO().Title);
            if((Int32.Parse(cardInDeck.GetCounterText()) + 1) > 2)
            {
                addBtn.DisableButton();
            }
            cardInDeck.SetCounterText((Int32.Parse(cardInDeck.GetCounterText()) + 1).ToString());
        }
        else
        {
            GameObject card = Instantiate(previewCardGO, transform);
            BaseCardLocal baseCardLocal = card.GetComponent<BaseCardLocal>();
            baseCardLocal.SetCardGameArea(GameAreaEnum.Hand);
            baseCardLocal.SetCardSO(previewCardGO.GetComponent<BaseCardLocal>().GetCardSO());
            baseCardLocal.SetCounterText("1");
        }
        OnDeckEditorCardChange?.Invoke(this, EventArgs.Empty);
    }
    private void Start()
    {
        OnDeckEditorCardChange?.Invoke(this, EventArgs.Empty);
        BaseCardLocal.OnCardMove += BaseCardLocal_OnCardMove;
        DecksManager.Instance.OnFetchDecks += DecksManager_OnFetchDecks;
        CardInventory.Instance.OnMakeCards += CardInventory_OnMakeCards;
    }

    private void CardInventory_OnMakeCards(object sender, EventArgs e)
    {
        ready1 = true;
        if (ready1 && ready2)
            PopulateDeckEditorAreaContent();
    }

    private void DecksManager_OnFetchDecks(object sender, EventArgs e)
    {
        if (deckToEdit == null) return;
        ready2 = true;
        if(ready1 && ready2)
        PopulateDeckEditorAreaContent();
        
    }

    private void BaseCardLocal_OnCardMove(object sender, EventArgs e)
    {
        OnDeckEditorCardChange?.Invoke(this, EventArgs.Empty);
    }
    public int GetCardsCount()
    {
        List<DeckCard> deckCards = GetEditedDeckCards();
        return deckCards.Aggregate(0, (acc, x) => acc + x.count);
    }

    public List<DeckCard> GetEditedDeckCards()
    {
        List<DeckCard> returnValue = new List<DeckCard>();
        Dictionary<string, int> cardsInDeckEditorDic = new Dictionary<string, int>();
        List<BaseCardLocal> cardsInDeckEditor
            = transform.GetComponentsInChildren<BaseCardLocal>().ToList();
        foreach (BaseCardLocal card in cardsInDeckEditor)
        {
            string title = card.GetCardSO().Title;
            
                cardsInDeckEditorDic[title] = Int32.Parse(card.GetCounterText());
            
        }
        foreach(KeyValuePair<string, int> entry in cardsInDeckEditorDic)
        {
            DeckCard deckCard = new DeckCard();
            deckCard.title = entry.Key;
            deckCard.count = entry.Value;
            returnValue.Add(deckCard);
        }
        Debug.Log("getditeddeckcards: " + returnValue.Count);
        return returnValue;
    }
    public int CardsInEditDeck()
    {
        return cards.Aggregate(0,(sum,cur ) => sum + cur.count);
    }
    private void PopulateDeckEditorAreaContent()
    {
        ready1 = false;
        ready2 = false;
        Decks decks = DecksManager.Instance.GetDecks();
        List<Deck> listOfDecks = decks.decks;
        Deck deck = listOfDecks.Find(x => x.name == deckToEdit);
        cards = deck.cards;

        OnDeckToEditCardsAssigned?.Invoke(this, EventArgs.Empty);


    }
    public void InstantiateDeckCard(DeckCard card)
    {
        if(cards.Any(x => x.title == card.title)) {
            //ugly surely there is a simpler way to achieve this but I give up
        DeckCard cardInDeck = cards.First(x => x.title == card.title);
        List<string> tempCardsList = CardInventory.Instance.GetTempCardsList();
        int childIndex = tempCardsList.FindIndex(x => x == card.title);
        Transform correspondingInventoryCardTransform =  CardInventory.Instance.transform.GetChild(childIndex);
        GameObject correspondingInventoryCardGO = correspondingInventoryCardTransform.gameObject;
        BaseCardLocal correspondingInventoryBaseCardLocal = correspondingInventoryCardTransform.GetComponent<BaseCardLocal>();
        correspondingInventoryBaseCardLocal.SetCounterText((card.count - cardInDeck.count).ToString());

        GameObject cardInDeckEditor = Instantiate(correspondingInventoryCardGO, transform);
        
        BaseCardLocal cardInDeckEditorBaseCardLocal = cardInDeckEditor.GetComponent<BaseCardLocal>();
        cardInDeckEditorBaseCardLocal.SetCardSO(correspondingInventoryBaseCardLocal.GetCardSO());
        cardInDeckEditorBaseCardLocal.SetCounterText(cardInDeck.count.ToString());
        cardInDeckEditor.GetComponent<BaseCardLocal>().SetCardGameArea(GameAreaEnum.Hand);
        OnDeckEditorCardChange?.Invoke(this, EventArgs.Empty);


            //Image image = ((ExpertCardUI)expertCardLocal.GetCardUI()).GetLightIcon();
            //image.material = null;

        }
    }

}
