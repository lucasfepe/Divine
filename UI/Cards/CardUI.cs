using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    
    private const string ADDRESS_CARD_GRAPHICS = "Assets\\CardGraphics";
    [SerializeField] protected Button cardButton;
    [SerializeField] protected BaseCard card;
    [SerializeField] protected TextMeshProUGUI titleText;
    [SerializeField] protected TextMeshProUGUI cardTypeText;
    [SerializeField] private Image characterImage;
    [SerializeField] protected Image background;
    [SerializeField] protected Image overlay;
    [SerializeField] private RockShaderController lightIcon;

    
    
    

    private InspectCardUI inspectCardUI;
    public static event EventHandler<OnPlayCardEventArgs> OnAnyPlayCard;
    private RectTransform titleRectTransform;
    public class OnPlayCardEventArgs : EventArgs
    {
        public BaseCard card;
    }
    

    virtual protected void Awake()
    {
        titleRectTransform = titleText.GetComponent<RectTransform>();
        //Don't like using Find but only way as cannot put a object that's not prefab inside prefab to my knowledge
        inspectCardUI = GameObject.Find("InspectCardUI").GetComponent<InspectCardUI>();
        cardButton.onClick.AddListener(() => {
           
            OnAnyPlayCard?.Invoke(this, new OnPlayCardEventArgs
            {
                card = card
            });
        });
        OnAnyPlayCard += CardUI_OnAnyPlayCard;
        //why do these not work when inside start
        //START happens after onnetwork spawn which is what invokes this esentailly
        CardGenerator.Instance.OnCardSOAssigned += CardGenerator_OnCardSOAssigned; ;
        //this feels iffy but prevents having to write two functions that do the same thing
    }

    virtual protected void CardGenerator_OnCardSOAssigned(object sender, CardGenerator.OnCardSOAssignedEventArgs e)
    {
        if (e.card != card) { return; }
        titleText.text = card.GetCardSO().Title.ToString();
        //cardTypeText.text = card.GetCardSO().cardType.ToString();
        switch (card.GetCardSO().Rank)
        {
            case StarClassEnum.RED:
                overlay.color = UniversalConstants.GetRed();
                break;
            case StarClassEnum.ORANGE:
                overlay.color = UniversalConstants.GetOrange();
                break;
            case StarClassEnum.YELLOW:
                overlay.color = UniversalConstants.GetYellow();
                break;
            case StarClassEnum.WHITE:
                overlay.color = UniversalConstants.GetWhite();
                break;
            case StarClassEnum.BLUE:
                overlay.color = UniversalConstants.GetBlue();
                break;

        }
        GetImage();
    }

    

    
    
    protected async void GetImage()
    {
        await S3.Instance.DownloadObjectFromBucketAsync(
                    S3.Instance.s3Client,
                    "divinebucket5487",
                    card.GetCardSO().ImageURL,
                    ADDRESS_CARD_GRAPHICS
                    );
        characterImage.sprite = Image2SpriteUtility.Instance.LoadNewSprite(ADDRESS_CARD_GRAPHICS + "\\" + card.GetCardSO().ImageURL);
    }
    public async void GetRedGiantImage()
    {
        await S3.Instance.DownloadObjectFromBucketAsync(
                    S3.Instance.s3Client,
                    "divinebucket5487",
                    card.GetCardSO().RedGiantImageURL,
                    ADDRESS_CARD_GRAPHICS
                    );
        characterImage.sprite = Image2SpriteUtility.Instance.LoadNewSprite(ADDRESS_CARD_GRAPHICS + "\\" + card.GetCardSO().RedGiantImageURL);
    }


    private void SelectTarget()
    {
        BaseCard selectTargetInitiatorCard = CardGameManager.Instance.GetSelectTargetInitiator();

        //This will select the viable target differently whether it's a expert of subterfuge card
        //very cool
        ViableTarget viableTarget = selectTargetInitiatorCard.GetViableTarget();

        if (viableTarget.Locations.Contains(card.GetCardGameArea())
            && viableTarget.Ranks.Contains(card.GetCardSO().Rank))
        {
            selectTargetInitiatorCard.TargetSelected(card);
            CardGameManager.Instance.DoneSelectingTarget();
        }
    }
    //ugly  cardUI really shouldn't be concerned with playing the card itself
    virtual protected void CardUI_OnAnyPlayCard(object sender, OnPlayCardEventArgs e)
    {
        if (e.card != card) return;

        if(card.GetCardGameArea() == GameAreaEnum.Field && card.IsGiant())
        {
            ((ExpertCard)card).UseSkill();
        }

        if ((card.GetCardGameArea() == GameAreaEnum.Field 
                || card.GetCardGameArea() == GameAreaEnum.OpponentField ) 
            && !CardGameManager.Instance.IsSelectingTarget())
        {

            card.InspectCard(); 
        }else if(CardGameManager.Instance.IsSelectingTarget()) 
        {
            SelectTarget();
        }
        if(card.GetCardOwner() == Player.Instance.IAm()
            && card.GetCardGameArea() == GameAreaEnum.Hand
            && CardGameManager.Instance.IsMyTurn()) {
            //hide the inspect balloon
            //very ugly
            transform.parent.GetComponentInChildren<InspectHandUI>().Hide();
            //
            if (CardGameManager.Instance.GetPlayer().GetStardust() >= card.GetStardust() && CardGameManager.Instance.CanPlayExpertCardThisTurn()) { 
                PlayerPlayingField.Instance.PlayCard(card);
                CardGameManager.Instance.PlayCard(card);
                
                

            }
        }


    }
    virtual public void RefreshUI() { }
    
    public void TriggerOnAnyPlayCard()
    {
        OnAnyPlayCard?.Invoke(this, new OnPlayCardEventArgs
        {
            card = card
        });
    }

    public BaseCard GetCard()
    {
        return card;
    }

    virtual public void AdaptLook()
    {
        switch (card.GetCardGameArea())
        {
            case GameAreaEnum.Hand:
                AdaptLookForHand();
                break;
            case GameAreaEnum.OpponentField:
                AdaptLookForOpponentField();
                    break;
            case GameAreaEnum.Field:
                AdaptLookForField();
                break;
            case GameAreaEnum.Inspect:
                AdaptLookForInspect();
                break;
        }
    }
    private void AdaptLookForHand()
    {
        

        titleRectTransform.anchoredPosition = new Vector2(0, 45.3f);
        titleText.fontSize = 25;
        titleText.gameObject.SetActive(true);
        cardTypeText.gameObject.SetActive(false);
    }
    private void AdaptLookForInspect()
    {
        titleRectTransform.anchoredPosition = new Vector2(0, 110f);
        titleText.fontSize = 15;
        titleText.gameObject.SetActive(true);
        cardTypeText.gameObject.SetActive(false);
    }
    private void AdaptLookForField()
    {
        titleRectTransform.anchoredPosition = new Vector2(0, 45.3f);
        titleText.fontSize = 25;
        titleText.gameObject.SetActive(true);
        cardTypeText.gameObject.SetActive(false);
    }private void AdaptLookForOpponentField()
    {
        titleRectTransform.anchoredPosition = new Vector2(0, 45.3f);
        titleText.fontSize = 25;
        titleText.gameObject.SetActive(true);
        cardTypeText.gameObject.SetActive(false);
    }

    public void GlowLight()
    {
        
        lightIcon.GlowLight(card.GetCardLight());
    }

    virtual public void RefreshStardustText( )
    {

    }
    virtual public void RefreshLightText()
    {

    }

}
