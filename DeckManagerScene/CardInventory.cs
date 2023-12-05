using Amazon.DynamoDBv2.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardInventory : MonoBehaviour
{
    public static CardInventory Instance { get; private set; }
    private PlayerCards deckCards;
    private List<string> tempCardsList = new List<string>();
    [SerializeField] private GameObject expertCardLocalPrefab;
    [SerializeField] private AddButton addButton;
    [SerializeField] private Transform cardHolderTransform;
    [SerializeField] private RemoveButton removeButton;
    private int inventoryCardsCount;
    private int cardsDone = 0;
    private const string ASCENDING = "ToggleAscending";
    public static List<string> cardInventorySortingOrder = new List<string>()
    {

        "Lac",
        "Arc",
        "Tau",
        "Sirius",
        "Ori",
        "Foal",
        "Aries",
        "Taurus",
        "Capricorn",
        "Aquarius",
        "Bao","Chang","Sandy","Rose","Guan",
        "Naomi","Mei","Krystel","Phoebe","Asha",
        "Centauri",
        "Leonis",
        "Uma",
        "Asellus",
        "Epsilon",
        "Shaula",
        "gj-1147",
        "hr-2162",
        "cd-9181",
        "hd-1651",
        "Lalonde",
        "Andromeda",
        "Auriga",
        "Botes",
        "Virgo",
        "Kapteyn",
        "Logan",
        
        "Ross",
        "Priya",
        "Fisher",
        "Capella",
        "Gliese",
        "Teegarden",
        "Bhibha",
        "Fu"

    };

    public event EventHandler OnMakeCards;

    private void Awake()
    {
        Instance = this;
        addButton.OnAdd += AddButton_OnAdd;
        removeButton.OnRemove += RemoveButton_OnRemove;
    }

    private void RemoveButton_OnRemove(object sender, EventArgs e)
    {
        BaseCardLocal previewCard = cardHolderTransform.GetChild(0).gameObject.GetComponent<BaseCardLocal>();
        string previewCardTitle = previewCard.GetCardSO().Title;
        List<BaseCardLocal> cardInInventory = transform.GetComponentsInChildren<BaseCardLocal>().ToList();
        BaseCardLocal relevantCardInInventory = cardInInventory.Where(x => x.GetCardSO().Title == previewCardTitle).First();
        relevantCardInInventory.SetCounterText((Int32.Parse(relevantCardInInventory.GetCounterText()) + 1).ToString());
        addButton.EnableButton();
    }

    private void AddButton_OnAdd(object sender, EventArgs e)
    {
        BaseCardLocal previewCard = cardHolderTransform.GetChild(0).gameObject.GetComponent<BaseCardLocal>();
        string previewCardTitle = previewCard.GetCardSO().Title;
        List<BaseCardLocal> cardInInventory = transform.GetComponentsInChildren<BaseCardLocal>().ToList();
        BaseCardLocal relevantCardInInventory = cardInInventory.Where(x => x.GetCardSO().Title == previewCardTitle).First();
        relevantCardInInventory.SetCounterText((Int32.Parse(relevantCardInInventory.GetCounterText()) - 1).ToString());


        if (Int32.Parse(relevantCardInInventory.GetCounterText()) <= 0)
        {
            addButton.DisableButton();
        }
        
        
        
    }

    public PlayerCards GetPlayerCardsObject()
    {
        return deckCards;
    }
    private void Start()
    {
        GetPlayerCards();
    }
    
    public void SortAndFilter(
        string sortCriterion,
        string sortOrder,
        string rankFilter,
        string rarityFilter,
        string skillTypeFilter,
        string nameInpu)
    {
        Debug.Log("sortandFilter");
        foreach (Transform child in transform) { 
            child.gameObject.SetActive(true);
    }
        //StartCoroutine(WaitAFrameThenApplySortAndFilter( sortCriterion,
        // sortOrder,
        // rankFilter,
        // rarityFilter,
        // skillTypeFilter,
        // nameInpu));
        int newNoCards = 0;
        BaseCardLocal[] inventoryCards = GetComponentsInChildren<BaseCardLocal>();
        foreach (BaseCardLocal card in inventoryCards)
        {

            bool matchesRankFilter = false;
            bool matchesRarityFilter = false;
            bool matchesSkillTypeFilter = false;
            bool matchesNameInpuFilter = false;
            Debug.Log("inactivate1: " + card.GetCardSO().Title);
            switch (rankFilter)
            {
                case "Any Rank":
                    matchesRankFilter = true;
                    break;
                case "BLUE":
                    if (card.GetCardSO().Rank == StarClassEnum.BLUE)
                        matchesRankFilter = true;
                    break;
                case "WHITE":
                    if (card.GetCardSO().Rank == StarClassEnum.WHITE)
                        matchesRankFilter = true;
                    break;
                case "YELLOW":
                    if (card.GetCardSO().Rank == StarClassEnum.YELLOW)
                        matchesRankFilter = true;
                    break;
                case "ORANGE":
                    if (card.GetCardSO().Rank == StarClassEnum.ORANGE)
                        matchesRankFilter = true;
                    break;
                case "RED":
                    if (card.GetCardSO().Rank == StarClassEnum.RED)
                        matchesRankFilter = true;
                    break;
            }
            switch (rarityFilter)
            {
                case "Any Rarity":
                    matchesRarityFilter = true;break;
                case "COMMON":
                    if(card.GetCardSO().Rarity == RarityEnum.Common) 
                        matchesRarityFilter = true;
                    break;
                case "UNCOMMON":
                    if (card.GetCardSO().Rarity == RarityEnum.Uncommon)
                        matchesRarityFilter = true;
                    break;
                case "RARE":
                    if (card.GetCardSO().Rarity == RarityEnum.Rare)
                        matchesRarityFilter = true;
                    break;
                case "UNIQUE":
                    if (card.GetCardSO().Rarity == RarityEnum.Unique)
                        matchesRarityFilter = true;
                    break;


            }
            switch (skillTypeFilter)
            {
                case "Any Skill":
                    matchesSkillTypeFilter = true;
                    break;
                case "Absorb":
                    if (card.GetCardSO().GiantEffect != null 
                        && card.GetCardSO().GiantEffect.Effect == EffectTypeEnum.Absorb)
                        matchesSkillTypeFilter = true;
                    break;
                case "Trigger Collision":
                    if (card.GetCardSO().GiantEffect != null
                        && card.GetCardSO().GiantEffect.Effect == EffectTypeEnum.Strange)
                        matchesSkillTypeFilter = true;
                    break;
                case "Shrink Enemy":
                    if (card.GetCardSO().GiantEffect != null
                        && card.GetCardSO().GiantEffect.Effect == EffectTypeEnum.Injure)
                        matchesSkillTypeFilter = true;
                    break;
                case "Untargetable":
                    if (card.GetCardSO().GiantEffect != null
                        && card.GetCardSO().GiantEffect.Effect == EffectTypeEnum.Untargetable)
                        matchesSkillTypeFilter = true;
                    break;
                case "Generate Light":
                    if (card.GetCardSO().GiantEffect != null
                        && card.GetCardSO().GiantEffect.Effect == EffectTypeEnum.GenerateLight)
                        matchesSkillTypeFilter = true;
                    break;
                case "Dim":
                    if (card.GetCardSO().GiantEffect != null
                        && card.GetCardSO().GiantEffect.Effect == EffectTypeEnum.Dim)
                        matchesSkillTypeFilter = true;
                    break;
                case "Grow Ally":
                    if (card.GetCardSO().GiantEffect != null
                        && card.GetCardSO().GiantEffect.Effect == EffectTypeEnum.Uphold)
                        matchesSkillTypeFilter = true;
                    break;
                case "Discount":
                    if (card.GetCardSO().GiantEffect != null
                        && card.GetCardSO().GiantEffect.Effect == EffectTypeEnum.Discount)
                        matchesSkillTypeFilter = true;
                    break;
                case "No Black Hole":
                    if (card.GetCardSO().GiantEffect != null
                        && card.GetCardSO().GiantEffect.Effect == EffectTypeEnum.NoBlackHole)
                        matchesSkillTypeFilter = true;
                    break;
                case "Gravity":
                    if (card.GetCardSO().GiantEffect != null
                        && card.GetCardSO().GiantEffect.Effect == EffectTypeEnum.Expensive)
                        matchesSkillTypeFilter = true;
                    break;
                case "Annihilation":
                    if (card.GetCardSO().GiantEffect != null
                        && card.GetCardSO().GiantEffect.Effect == EffectTypeEnum.WipeoutBothFields)
                        matchesSkillTypeFilter = true;
                    break;
                case "Gemini":
                    if (card.GetCardSO().GiantEffect != null
                        && card.GetCardSO().GiantEffect.Effect == EffectTypeEnum.Gemini)
                        matchesSkillTypeFilter = true;
                    break;
            }
            Debug.Log("nameInput: " + nameInpu);
            if(card.GetCardSO().Title.ToLower().Contains(nameInpu.ToLower()))matchesNameInpuFilter = true;
            if (!(matchesRankFilter && matchesRarityFilter && matchesSkillTypeFilter && matchesNameInpuFilter))
            {

                Debug.Log("inactivate2: " + card.GetCardSO().Title);
                card.gameObject.SetActive(false);
            }
            else
            {
                newNoCards++;
            }
        }
        Debug.Log("sortCriterion: " + sortCriterion + " sortOrder: " + sortOrder);
        switch (sortCriterion)
        {
            case "ToggleAlpha":
                inventoryCards
                    .OrderBy(x => x.GetCardSO().Title)
                    .ToList()
                    .ForEach(x =>
                    {
                        if (sortOrder == ASCENDING)
                        {
                            x.gameObject.transform.SetAsLastSibling();
                        }
                        else
                        {
                            x.gameObject.transform.SetAsFirstSibling();
                        }
                    });
                break;
            case "ToggleRarity":
                inventoryCards
                    .OrderBy(x => x.GetCardSO().Rarity)
                    .ToList()
                    .ForEach(x =>
                    {
                        if (sortOrder == ASCENDING)
                        {
                            x.gameObject.transform.SetAsLastSibling();
                        }
                        else
                        {
                            x.gameObject.transform.SetAsFirstSibling();
                        }
                    });
                break;
            case "ToggleStardust":
                inventoryCards
                    .OrderBy(x => x.GetCardSO().Stardust)
                    .ToList()
                    .ForEach(x =>
                    {
                        if (sortOrder == ASCENDING)
                        {
                            x.gameObject.transform.SetAsLastSibling();
                        }
                        else
                        {
                            x.gameObject.transform.SetAsFirstSibling();
                        }
                    });
                break;
            case "ToggleLight":
                inventoryCards
                    .OrderBy(x => x.GetCardSO().Light)
                    .ToList()
                    .ForEach(x =>
                    {
                        if (sortOrder == ASCENDING)
                        {
                            x.gameObject.transform.SetAsLastSibling();
                        }
                        else
                        {
                            x.gameObject.transform.SetAsFirstSibling();
                        }
                    });
                break;
            case "ToggleRank":
                inventoryCards
                    .OrderBy(x => x.GetCardSO().Rank)
                    .ToList()
                    .ForEach(x =>
                    {
                        if (sortOrder == ASCENDING)
                        {
                            x.gameObject.transform.SetAsLastSibling();
                        }
                        else
                        {
                            x.gameObject.transform.SetAsFirstSibling();
                        }
                    });
                break;
        }
        
        

        inventoryCardsCount = newNoCards;
        OnMakeCards?.Invoke(this, EventArgs.Empty);

    }
    private async void GetPlayerCards()
    {
        
        string  playerCardsString = await LambdaManager.Instance.GetPlayerCardsLambda();
        deckCards = JsonUtility.FromJson<PlayerCards>(playerCardsString);
        inventoryCardsCount = deckCards.playerCards.Count;
        List<DeckCard> sortedDeckCards = deckCards.playerCards.OrderBy(x => cardInventorySortingOrder.FindIndex(y => y == x.title)).ToList();

        foreach (DeckCard card in sortedDeckCards)
        {
             InstantiateDeckCard(card);
        }
        OnMakeCards?.Invoke(this, EventArgs.Empty);
        

    }
   

    private async void InstantiateDeckCard(DeckCard card)
    {
        //List<ExpertCardLocal> expertCardLocalList = new List<ExpertCardLocal>();
        //for (int i = 0; i < card.count; i++)
        //{
            GameObject cardUI = Instantiate(expertCardLocalPrefab, transform);
            tempCardsList.Add(card.title);
            cardUI.SetActive(false);
            ExpertCardLocal expertCardLocal = cardUI.GetComponent<ExpertCardLocal>();
            expertCardLocal.SetCounterText(card.count.ToString());
            //expertCardLocalList.Add(expertCardLocal);
            Image image = ((ExpertCardUI)expertCardLocal.GetCardUI()).GetLightIcon();
            image.material = null;
            
        //}
        CardSO cardSO = await CardGenerator.Instance.CardNameToCardSO(card.title);
       
        expertCardLocal.SetCardSO(cardSO);
        
        cardUI.GetComponent<BaseCardLocal>().SetCardGameArea(GameAreaEnum.Hand);
        expertCardLocal.gameObject.SetActive(true);
        
        if(DeckManagerStatic.GetDeckToEdit() != null)
        {
            DeckEditorAreaContent.Instance.InstantiateDeckCard(card);
        }
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
    public List<string> GetTempCardsList()
    {
        return tempCardsList;
    }
    public void SetTempCardsList(List<string> tempCardsList)
    {
        this.tempCardsList = tempCardsList;
    }
    public List<BaseCardLocal> GetCards()
    {
        return transform.GetComponentsInChildren<BaseCardLocal>().ToList();
    }

    public int GetInventoryCardsCount()
    {
        return inventoryCardsCount;
    }
}
