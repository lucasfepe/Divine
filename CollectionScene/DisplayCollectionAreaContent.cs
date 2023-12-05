using Amazon.DynamoDBv2.DocumentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using Mono.CSharp;
using TMPro;

public class DisplayCollectionAreaContent : MonoBehaviour
{

    public event EventHandler OnDetermineNumberOfPages;
    public event EventHandler OnPageIndexChanged;
    private int numberPages = 0;
    private PlayerCards cardInventory;
    public event EventHandler OnSelectCollection;
    public event EventHandler OnReadyToShowCollections;
    private int cardInstantiationComplete = 0;
    private int cardsToInstantiate = 0;
    private int pageIndex = 1;
    private int cardsPerPage = 0;
    private bool doneInstantiatingFirstRow = false;
    private float xPositionRowStart;
    private bool instantiatedFirstCard = false;
    private int cardsBeganInstantiating = 0;
    private int cardsPerRow = 0;
    public static DisplayCollectionAreaContent Instance { get; private set; }   
    [SerializeField] private GameObject cardPrefab;
    private List<Document> allCards;
    private bool retrievedCardCatalogue = false;
    private bool retrievedCardInventory = false;

    private async void Awake()
    {
        
        Instance = this;
        allCards = await DynamoDB.RetrieveAllCards();
        retrievedCardCatalogue = true;
        CheckReadyToShowCollections();
    }
    private void CheckReadyToShowCollections()
    {
        if(retrievedCardInventory && retrievedCardCatalogue)
        {
            OnReadyToShowCollections?.Invoke(this, EventArgs.Empty);
        }
    }
    private async void Start()
    {
        string playerCardsString = await LambdaManager.Instance.GetPlayerCardsLambda();
        cardInventory = JsonUtility.FromJson<PlayerCards>(playerCardsString);
        retrievedCardInventory = true;
        CheckReadyToShowCollections();
    }
    public  void SelectCollection(CollectionsEnum collection)
    {


        OnSelectCollection?.Invoke(this, EventArgs.Empty);

          
        List<DeckCard> cards = new List<DeckCard>();
        foreach (Document card in allCards)
        {
            //if the card belongs in the collection selected
            if (Int32.Parse(card["Collection"]) == (int)collection)
            {
                //if the player has this card
                if(cardInventory
                    .playerCards
                    .Select(x => x.title)
                    .ToList()
                    .Contains(card["Title"]))
                {
                    //add it to 'cards' to be displayed
                    DeckCard ownCard = new DeckCard();
                    ownCard.title = card["Title"];
                    ownCard.count = cardInventory.playerCards.Find(x  => x.title == card["Title"]).count;
                    cards.Add(ownCard);
                }
                else{
                    //if player does not have this card also add it but count of 0
                    DeckCard ownCard = new DeckCard();
                    ownCard.title = card["Title"];
                    ownCard.count = 0;
                    cards.Add(ownCard);
                }
            }
        }
        //sort cards to be displayed according to default order
        List<DeckCard> sortedDeckCards = cards.OrderBy(x => CardInventory.cardInventorySortingOrder.FindIndex(y => y == x.title)).ToList();

       

        cardsToInstantiate = sortedDeckCards.Count;
        int i = 1;
        foreach (DeckCard card in sortedDeckCards)
        {
            InstantiateDeckCard(card, i);
            i++;
        }

        
    }

    private IEnumerator ProcessNewCard(GameObject cardUI, int index)
    {
        yield return 0;

        BaseCardLocal baseCardLocalCreated = cardUI.GetComponent<BaseCardLocal>();
        if (!doneInstantiatingFirstRow)
        {
            baseCardLocalCreated.SetPageIndex(1);
            if (!instantiatedFirstCard)
            {
                Debug.Log("instantiatedFirstCard: " + instantiatedFirstCard);
                instantiatedFirstCard = true;
                xPositionRowStart = transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition.x;
            }
            else
            {
                if (!doneInstantiatingFirstRow && cardUI.transform.GetComponent<RectTransform>().anchoredPosition.x == xPositionRowStart)
                {

                    doneInstantiatingFirstRow = true;
                    cardsPerRow = cardsBeganInstantiating;
                    Debug.Log("done instantiating  first row, cards per row: " + cardsPerRow);
                    PopulatePageIndexUI();
                }
            }
        }
        else
        {
            baseCardLocalCreated.SetPageIndex((int)Mathf.Ceil((float)index / GetCardsPerPage()));
        }
        cardsBeganInstantiating++;

        Debug.Log("card index: " + index + " card page index: " + baseCardLocalCreated.GetPageIndex());
        if (baseCardLocalCreated.GetPageIndex()
               == 1)
        {

            cardUI.gameObject.SetActive(true);
        }
        else
        {
            cardUI.gameObject.SetActive(false);
        }
    }
    private async void InstantiateDeckCard(DeckCard card, int index)
    {
        //List<ExpertCardLocal> expertCardLocalList = new List<ExpertCardLocal>();
        //for (int i = 0; i < card.count; i++)
        //{
        
        GameObject cardUI = Instantiate(cardPrefab, transform);
        MakeInvisible(cardUI);
        StartCoroutine(ProcessNewCard(cardUI, index));

        if (card.count == 0)
        {
            Material greyscaleMaterial = new Material(cardUI.transform.GetChild(0).GetComponent<Renderer>().material);

            cardUI.GetComponentsInChildren<Image>().ToList().ForEach(x => x.material = greyscaleMaterial);
            
            
            cardUI.GetComponent<ExpertCardLocal>().GetCardUI().GetLightImage().material = greyscaleMaterial;
            greyscaleMaterial.SetFloat("_GreyscaleBlend", 1);
        }

        //cardUI.SetActive(false);
        ExpertCardLocal expertCardLocal = cardUI.GetComponent<ExpertCardLocal>();
        expertCardLocal.SetCounterText(card.count.ToString());
        //expertCardLocalList.Add(expertCardLocal);
        

        //}
        CardSO cardSO = await CardGenerator.Instance.CardNameToCardSO(card.title);

        expertCardLocal.SetCardSO(cardSO);

        cardUI.GetComponent<BaseCardLocal>().SetCardGameArea(GameAreaEnum.Catalogue);
        MakeVisible(cardUI);



        cardInstantiationComplete++;
        
        //for (int i = 0; i < card.count; i++)
        //{
        //expertCardLocalList
        //    .Where(x => !x.IsCardDestroyed())
        //    .ToList()
        //    .ForEach(x => {
        //    x.SetCardSO(cardSO);
        //    x.gameObject.SetActive(true);
        //});

        //}


    }
    private void MakeInvisible( GameObject gameObject)
    {
        gameObject.GetComponentsInChildren<Image>().ToList().ForEach(x => x.enabled = false);
        gameObject.GetComponentsInChildren<TextMeshProUGUI>().ToList().ForEach(x => x.enabled = false);
        Transform countBG = gameObject.transform.Find("Canvas/CardUI/CountIndicator");
        countBG.GetComponent<Image>().enabled = false;
        Transform countTxt = gameObject.transform.Find("Canvas/CardUI/CountIndicator/CountText");
        countTxt.GetComponent<TextMeshProUGUI>().enabled = false;
    }

    private void MakeVisible(GameObject gameObject)
    {
        gameObject.GetComponentsInChildren<Image>().ToList().ForEach(x => x.enabled = true);
        gameObject.GetComponentsInChildren<TextMeshProUGUI>().ToList().ForEach(x => x.enabled = true);
    }
    private int GetCardsPerPage()
    {
        return cardsPerRow * 3;
    }

    private void PopulatePageIndexUI()
    {

        numberPages = (int)MathF.Ceiling((float)cardsToInstantiate / cardsPerRow / 3);
        Debug.Log("number pages: " + numberPages);
        
        OnDetermineNumberOfPages?.Invoke(this, EventArgs.Empty);
    }
   
    public int GetNumberOfPages()
    {
        return numberPages;
    }
    //public int GetCardsPerRow()
    //{
    //    bool firstChild = true;
    //    int noCardsPerRow = 0;
        
    //    foreach(Transform child in transform)
    //    {
    //        if (firstChild)
    //        {
    //            firstChild = false;
    //            continue;
    //        }
    //        if (child.GetComponent<RectTransform>().anchoredPosition.x != xPositionRowStart)
    //        {
    //            noCardsPerRow++;
    //        }
    //        else
    //        {
    //            break;
    //        }
    //    }
    //    //the one is the first child
    //    return noCardsPerRow + 1;

    //}
    public void PageBack()
    {

        pageIndex--;
        OnPageIndexChanged?.Invoke(this, EventArgs.Empty);
        foreach(Transform child in transform) { 
            if(child.GetComponent<BaseCardLocal>().GetPageIndex() == pageIndex)
            {
                child.gameObject.SetActive(true);
            }
            else {
                child.gameObject.SetActive(false);
            }
        }
    }
    public void PageForward()
    {

        pageIndex++;
        OnPageIndexChanged?.Invoke(this, EventArgs.Empty);
        foreach (Transform child in transform)
        {
            if (child.GetComponent<BaseCardLocal>().GetPageIndex() == pageIndex)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }
    public int GetPageIndex()
    {
        return pageIndex;
    }
}
