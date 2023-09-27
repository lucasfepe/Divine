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
    protected ICard card;
    [SerializeField] private RectTransform cardRectTransform;
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
        public ICard card;
    }
    

    virtual protected void Awake()
    {
        
        card = transform.parent.parent.GetComponent<ICard>();
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
        card.OnCardSOAssigned += Card_OnCardSOAssigned;
        //this feels iffy but prevents having to write two functions that do the same thing
    }

    virtual protected void Card_OnCardSOAssigned(object sender, EventArgs e)
    {
        
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
    virtual public void BecomeRedGiant()
    {
        GetRedGiantImage();
        
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
            selectTargetInitiatorCard.TargetSelected((BaseCard)card);
            CardGameManager.Instance.DoneSelectingTarget();
        }
    }
    //ugly  cardUI really shouldn't be concerned with playing the card itself
    virtual protected void CardUI_OnAnyPlayCard(object sender, OnPlayCardEventArgs e)
    {
        if (e.card != card) return;
        if(card is BaseCard)
        Debug.Log("CardUI_OnAnyPlayCard is giant: " + ((BaseCard)card).IsGiant() + " game area;: " + card.GetCardGameArea());
        if (card.GetCardGameArea() == GameAreaEnum.Field && ((BaseCard)card).IsGiant())
        {
            Debug.Log("CardUI_OnAnyPlayCard: ");
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
        if (card.GetCardGameArea() == GameAreaEnum.Hand
             && CardGameManager.Instance.IsMyTurn())
        {
            //hide the inspect balloon
            //very ugly
            transform.parent.GetComponentInChildren<InspectHandUI>().Hide();
            //
            if (CardGameManager.Instance.GetPlayer().GetStardust() >= card.GetCardStardust() && CardGameManager.Instance.CanPlayExpertCardThisTurn())
            {
                PlayerPlayingField.Instance.TryPlayCard(card);




            }
        }


    }
    
    public void TriggerOnAnyPlayCard()
    {
        OnAnyPlayCard?.Invoke(this, new OnPlayCardEventArgs
        {
            card = card
        });
    }

    public ICard GetCard()
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
        ;
        
        cardRectTransform.localScale = new Vector2(UniversalConstants.HAND_CARD_SCALE, UniversalConstants.HAND_CARD_SCALE);
    }
    private void AdaptLookForInspect()
    {
        titleRectTransform.anchoredPosition = new Vector2(0, 110f);
        titleText.fontSize = 15;
        titleText.gameObject.SetActive(true);
        cardTypeText.gameObject.SetActive(false);
        cardRectTransform.localScale = new Vector2(UniversalConstants.INSPECT_CARD_SCALE, UniversalConstants.INSPECT_CARD_SCALE);
    }
    private void AdaptLookForField()
    {
        titleRectTransform.anchoredPosition = new Vector2(0, 45.3f);
        titleText.fontSize = 25;
        titleText.gameObject.SetActive(true);
        cardTypeText.gameObject.SetActive(false);
        cardRectTransform.localScale = new Vector2(UniversalConstants.FIELD_CARD_SCALE, UniversalConstants.FIELD_CARD_SCALE);
    }
    private void AdaptLookForOpponentField()
    {
        titleRectTransform.anchoredPosition = new Vector2(0, 45.3f);
        titleText.fontSize = 25;
        titleText.gameObject.SetActive(true);
        cardTypeText.gameObject.SetActive(false);
        cardRectTransform.localScale = new Vector2(UniversalConstants.FIELD_CARD_SCALE, UniversalConstants.FIELD_CARD_SCALE);
    }

    public void GlowLight()
    {
        
        lightIcon.GlowLight(card.GetCardLight());

        //whenever a light glows it must also glow in the opponent's screen
        //use a server/client rpc pair to accomplish this
        //card only glows in field so okay to cast
        //only call server rpc if I own the card otherwise infinite loop
        if(((BaseCard)card).GetCardOwner() == Player.Instance.IAm())
        {
            ((BaseCard)card).GlowLightServerRpc();
        }
        
    }



    virtual public void RefreshStardustText( )
    {

    }
    virtual public void RefreshLightText()
    {

    }

}
