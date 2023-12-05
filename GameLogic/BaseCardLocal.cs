using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseCardLocal : MonoBehaviour, ICard
{
    //how do I keep track of this card's stardust and light with a network variable in case a player disconnects 
    //if I make is a network Behaviour it will make two of it, can I have two of it but in different places in the two different players??
    //public event EventHandler<OnCardHoverEventArgs> OnCardHover;
    public event EventHandler OnCardHoverEnter;
    public event EventHandler OnCardHoverExit;
    public event EventHandler OnCardSOAssigned;
    public static event EventHandler OnCardMove;
    public static event EventHandler OnCardPreview;
    
    protected CardSO cardSO;
    protected CardUI cardUI;
    protected RectTransform rectTransform;
    private int cardStardust;
    private int cardLight;
    private IPlayer player;
    private GameAreaEnum gameArea;
    private Transform hand;
    private Transform cardInventory;
    private Transform deckPreview;
    private Transform deckEditor;
    private Transform mainCanvas;
    private Transform previousParent;
    private Canvas canvas;
    private int siblingPositionIndex;
    private Vector2 previousMousePosition;
    private Camera main;
    private bool isDragging;
    private Vector2 initialPosition;
    private Transform initialParent;
    private IsMouseInsideRectangle isMouseInside;
    private IsMouseInsideRectangle isMouseInsideCardInventory;
    private bool isCardDestroyed = false;
    private int pageIndex = 0;
    [SerializeField] private SelectedCardVisual selectedCardVisual;
    [SerializeField] private GameObject counterGameObject;

    private TextMeshProUGUI counterText;

    public void SetPageIndex(int index)
    {
        pageIndex = index;
    }
    public int GetPageIndex()
    {
        return pageIndex;
    }
    public void MakeDead()
    {

        isCardDestroyed = true;

    }
    public bool IsCardDestroyed()
    {
        return isCardDestroyed;
    }
    virtual public void PlayCard()
    {
        player.SetStardust(player.GetStardust() - cardStardust);
        DivineMultiplayer.Instance.PlayCardServerRpc(cardSO.Title, Player.Instance.IAm());
        Destroy(gameObject);
    }

    protected void CallOnCardSOAssigned()
    {
        OnCardSOAssigned?.Invoke(this, EventArgs.Empty);
    }
    [ServerRpc(RequireOwnership = false)]
    public void BecomeTargetServerRpc()
    {
        BecomeTargetClientRpc();
    }
    [ClientRpc]
    private void BecomeTargetClientRpc()
    {
        
            ((ExpertCardUI)cardUI).Flash();
            ((ExpertCardUI)cardUI).GetCardGraphicAnimator().BecomeTarget();
        
    }
    [ServerRpc(RequireOwnership = false)]
    public void StopBeingTargetServerRpc(PlayerEnum p)
    {
        StopBeingTargetClientRpc(p);
    }
    [ClientRpc]
    private void StopBeingTargetClientRpc(PlayerEnum p)
    {
        if (IsCardDestroyed()) return;

        Debug.Log("done flash, stop being atrgetll cleint rpc bcl");
            ((ExpertCardUI)cardUI).DoneFlash();
            ((ExpertCardUI)cardUI).GetCardGraphicAnimator().StopBeingTarget();
        
    }
    virtual protected void Awake()
    {
        counterText = counterGameObject.GetComponentInChildren<TextMeshProUGUI>();
        HideCounter();
        mainCanvas = GameObject.Find("Canvas").transform;
        main = Camera.main;
        if (CardGameManager.Instance == null)
        {
            InspectHandUI inspectHandUI = GetComponentInChildren<InspectHandUI>();
            if (inspectHandUI != null)
            {
                inspectHandUI.gameObject.SetActive(false);
            }
        }
        canvas = GetComponentInChildren<Canvas>();
        GameObject playerHand = GameObject.Find("PlayerHand");
        if (playerHand != null)
        {
            hand = playerHand.transform;
        }
        //ugly no comment
        GameObject cardInventoryGO = GameObject.Find("CardInventoryAreaContent");
        if (cardInventoryGO != null)
        {
            cardInventory = cardInventoryGO.transform;
        }
        GameObject displayDeckGO = GameObject.Find("DisplayDecksAreaContent");
        if (displayDeckGO != null)
        {
            deckPreview = displayDeckGO.transform;
        }
        GameObject deckEditorGO = GameObject.Find("DeckEditorAreaContent");
        if (deckEditorGO != null)
        {
            deckEditor = deckEditorGO.transform;
            isMouseInside = deckEditor.GetComponentInParent<IsMouseInsideRectangle>();
            isMouseInsideCardInventory = cardInventory.GetComponentInParent<IsMouseInsideRectangle>();
        }

        gameArea = GameAreaEnum.Hand;
        cardUI = GetComponentInChildren<CardUI>();
        rectTransform = GetComponent<RectTransform>();
    }
    protected void Start()
    {
        if (CardGameManager.Instance != null)
        {
            player = CardGameManager.Instance.GetPlayer();
        }



    }

    public void DeckEditorSceneSelected()
    {
        Transform cardHolderTransform = GameObject.Find("CardHolder").transform;
        Destroy(cardHolderTransform.GetChild(0).gameObject);
        GameObject cardPreview = Instantiate(this.gameObject, cardHolderTransform);
        cardPreview.transform.localPosition = Vector3.zero;
        ((RectTransform)cardPreview.transform).sizeDelta = new Vector2(200, 300);
        BaseCardLocal cardPreviewBaseCardLocal = cardPreview.GetComponent<BaseCardLocal>();
        
        //((ExpertCardUI)cardPreviewBaseCardLocal.GetCardUI()).SetHasSkill(((ExpertCardUI)GetCardUI()).HasSkill());
        cardPreviewBaseCardLocal.SetCardSO(GetCardSO());
        cardPreviewBaseCardLocal.SetCardGameArea(GameAreaEnum.CardPreview);
        OnCardPreview?.Invoke(this, EventArgs.Empty);
    }

    virtual public void InspectCard()
    {
        if (SceneManager.GetActiveScene().name == SceneLoader.Scene.DeckEditorScene.ToString()) return;
        siblingPositionIndex = transform.GetSiblingIndex();
        InspectCardUI inspectCardUI1 = GameObject.Find("InspectCardUI").GetComponent<InspectCardUI>();

        inspectCardUI1.Show(this);
        previousParent = transform.parent;
        transform.SetParent(inspectCardUI1.transform);
        rectTransform.anchorMax = new Vector2(.5f, .5f);
        rectTransform.anchorMin = new Vector2(.5f, .5f);

        if (CardGameManager.Instance == null)
        {
            rectTransform.localPosition = Vector2.zero;
        }
        else
        {
            rectTransform.position = Vector2.zero;
        }
        StartCoroutine(ChangeSorting());

        SetCardGameArea(GameAreaEnum.Inspect);
        //turn off highlight
        //selectedCardVisual.Hide();
    }

    private IEnumerator ChangeSorting()
    {
        yield return null;

        canvas.overrideSorting = true;
        int topSorting = 80;
        canvas.sortingOrder = topSorting;

    }

    private IEnumerator ChangeSortingBack()
    {
        yield return null;

        canvas.overrideSorting = false;
        canvas.sortingOrder = 0;

    }

    virtual public void DoneInspectingCard()
    {
        if (GetCardGameArea() == GameAreaEnum.Inspect)
        {
            if (hand != null)
            {
                transform.SetParent(hand);
            }
            else if (cardInventory != null)
            {
                transform.SetParent(previousParent);
            }
            else if (deckPreview != null)
            {
                transform.SetParent(deckPreview);
            }

            transform.SetSiblingIndex(siblingPositionIndex);
            rectTransform.anchorMax = new Vector2(.5f, .5f);
            rectTransform.anchorMin = new Vector2(.5f, .5f);
            int normalSorting = 0;
            canvas.sortingOrder = normalSorting;
            canvas.overrideSorting = false;
            rectTransform.localPosition = Vector2.zero;

            StartCoroutine(ChangeSortingBack());
             if (DisplayCollectionAreaContent.Instance != null)
            {
                SetCardGameArea(GameAreaEnum.Catalogue);
            }
            else
            {
                SetCardGameArea(GameAreaEnum.Hand);
            }
            
        }
    }
    private void OnMouseEnter()
    {
        OnCardHoverEnter?.Invoke(this, EventArgs.Empty);

    }
    private void OnMouseExit()
    {
        OnCardHoverExit?.Invoke(this, EventArgs.Empty);
    }
    virtual public void SetCardSO(CardSO cardSO)
    {
        this.cardSO = cardSO;
        //This is quite sneakily put in here
        //if I just set cardSO = cardSO instead of calling this method
        //light and stardust will never be set...
        cardStardust = cardSO.Stardust;
        cardLight = cardSO.Light;


        //Show();



    }
    public CardSO GetCardSO()
    {
        return cardSO;
    }
    public GameAreaEnum GetCardGameArea()
    {
        return gameArea;
    }
    public void SetCardGameArea(GameAreaEnum gameArea)
    {
        this.gameArea = gameArea;
        cardUI.AdaptLook();
    }










    virtual public bool IsGiant()
    {
        return false;
    }

    virtual public void TargetSelected(BaseCard targetCard) { }
    virtual public ViableTarget GetViableTarget() { return null; }


    public int GetStardust()
    {
        return cardStardust;
    }





    public int GetCardLight()
    {
        return cardLight;
    }

    public CardUI GetCardUI()
    {
        return cardUI;
    }


    private int GetIndex()
    {
        return PlayerPlayingField.Instance
                .GetAllPlayerExpertCards()
                .FindIndex(x => x == this);
    }

    public bool IsCardMine()
    {
        return true;
    }

    public int GetCardStardust()
    {
        return cardStardust;
    }

    virtual public int GetLifetime()
    {
        return 0;
    }



    //private void OnMouseDrag()
    //{
    //    if (CardGameManager.Instance != null) return;
    //    if (GetCardGameArea() == GameAreaEnum.Inspect) return;
    //    if (previousMousePosition != Vector2.zero)
    //    {


    //        Vector2 distanceMoved = (Vector2)main.ScreenToWorldPoint(Input.mousePosition) - previousMousePosition;
    //        if (distanceMoved != Vector2.zero)
    //        {

    //            if (!isDragging)
    //            {
    //                siblingPositionIndex = transform.GetSiblingIndex();
    //                initialParent = transform.parent;
    //                initialPosition = transform.position;
    //                transform.SetParent(mainCanvas);
    //            }
    //            isDragging = true;
    //        }
    //        transform.position = (Vector2)transform.position + distanceMoved;
    //    }
    //    previousMousePosition = main.ScreenToWorldPoint(Input.mousePosition);
    //}

    //private void Update()
    //{
    //    if (isDragging
    //        && Input.GetMouseButtonUp(0)
    //        && (Vector2)transform.position != initialPosition)
    //    {
    //        if (isMouseInside.IsMouseInside())
    //        {
    //            isDragging = false;
    //            List<BaseCardLocal> baseCardLocals = DeckEditorAreaContent
    //                .Instance
    //                .transform
    //                .GetComponentsInChildren<BaseCardLocal>()
    //                .ToList();
    //            int index = baseCardLocals.FindIndex(x => x.GetCardSO().Title == GetCardSO().Title);
    //            transform.SetParent(deckEditor);
    //            if (index != -1)
    //                transform.SetSiblingIndex(index);

    //            previousMousePosition = Vector2.zero;
    //            OnCardMove?.Invoke(this, EventArgs.Empty);
    //        }
    //        else if (isMouseInsideCardInventory.IsMouseInside())
    //        {
    //            isDragging = false;
    //            transform.SetParent(cardInventory);
    //            BaseCardLocal[] baseCardLocals = cardInventory.GetComponentsInChildren<BaseCardLocal>();

    //            if (baseCardLocals.ToList().Any(x => x.cardSO.Title == cardSO.Title))
    //            {
    //                siblingPositionIndex = baseCardLocals
    //                    .ToList()
    //                    .FindIndex(x => x.cardSO.Title == cardSO.Title);
    //                transform.SetSiblingIndex(siblingPositionIndex);
    //            }


    //            transform.position = initialPosition;
    //            previousMousePosition = Vector2.zero;
    //        }
    //        else
    //        {
    //            isDragging = false;
    //            transform.SetParent(initialParent);
    //            transform.SetSiblingIndex(siblingPositionIndex);
    //            transform.position = initialPosition;
    //            previousMousePosition = Vector2.zero;
    //        }

    //    }
    //}
    public void SetCounterText(string count)
    {
        counterText.text = count;
        ShowCounter();
    }
    public string GetCounterText() {
        return counterText.text;
    }
    public void HideCounter()
    {
        counterGameObject.SetActive(false);
    }
    public void ShowCounter()
    {
        counterGameObject.SetActive(true);
    }
    public void Discount(int stardustDiscount)
    {
        
        cardStardust -= stardustDiscount;
        if(cardStardust < 0)
        {
            cardStardust = 0;
        }
        cardUI.RefreshStardustText();
    }
    public void SetRarityVisual()
    {
        switch (cardSO.Rarity)
        {
            case RarityEnum.Uncommon:
                cardUI.ShowEmerald();
                break;
            case RarityEnum.Rare:
                cardUI.ShowAmethyst();
                break;
            case RarityEnum.Unique:
                cardUI.ShowJade();
                break;

        }
    }
}
