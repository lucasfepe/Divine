using Mono.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    
    private const string ADDRESS_CARD_GRAPHICS = "Assets\\CardGraphics";
    [SerializeField] protected Button cardButton;
    protected ICard card;
    [SerializeField] private RectTransform cardRectTransform;
    [SerializeField] protected TextMeshProUGUI titleText;
    [SerializeField] private Image characterImage;
    [SerializeField] protected Image background;
    [SerializeField] protected Image overlay;
    [SerializeField] private GlowShaderController lightIcon;
    [SerializeField] private BeSuckedByBlackHoleAnimation beSuckedByBlackHoleAnimation;
    [SerializeField] private BecomeWhiteDwarfAnimation becomeWhiteDwarfAnimation;
    [SerializeField] private NeutronStarAnimation neutronStarAnimation;
    [SerializeField] private BlackHoleAnimation blackHoleAnimation;
    [SerializeField] private StrangifyAnimation strangifyAnimation;
    [SerializeField] private GameObject emerald;
    [SerializeField] private GameObject amethyst;
    [SerializeField] private GameObject jade;
    [SerializeField] private GameObject counterText;
    public Image GetCharacterImage()
    {
        return characterImage;
    }





    private InspectCardUI inspectCardUI;
    public static event EventHandler<OnPlayCardEventArgs> OnAnyPlayCard;
    private RectTransform titleRectTransform;
    
    public class OnPlayCardEventArgs : EventArgs
    {
        public ICard card;
    }
    private void Update()
    {
        
        //canUseSkill = ((ExpertCard)card).IsGiant()
        //    && card.GetCardSO()
    }
   

    

    virtual protected void Awake()
    {



        
        card = transform.parent.parent.GetComponent<ICard>();


        titleRectTransform = titleText.GetComponent<RectTransform>();
        //Don't like using Find but only way as cannot put a object that's not prefab inside prefab to my knowledge
        GameObject inspectCardUI = GameObject.Find("InspectCardUI");
        if(inspectCardUI != null )
        {
            this.inspectCardUI = inspectCardUI.GetComponent<InspectCardUI>();
        }
        
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
    private void OnDestroy()
    {
        OnAnyPlayCard -= CardUI_OnAnyPlayCard;
        card.OnCardSOAssigned -= Card_OnCardSOAssigned;
    }
    virtual protected void Card_OnCardSOAssigned(object sender, EventArgs e)
    {
        if (card.IsCardDestroyed()) return;

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
        //await S3.Instance.DownloadObjectFromBucketAsync(
        //            S3.Instance.s3Client,
        //            "divinebucket5487",
        //            card.GetCardSO().Title,
        //            ADDRESS_CARD_GRAPHICS
        //            );
        characterImage.sprite = Resources.Load<Sprite>(card.GetCardSO().Title); /*Image2SpriteUtility.Instance.LoadNewSprite(ADDRESS_CARD_GRAPHICS + "\\" + card.GetCardSO().ImageURL);*/
    }
    virtual public void BecomeRedGiant()
    {
        GetRedGiantImage();
        
    }
    public async void GetRedGiantImage()
    {
        //await S3.Instance.DownloadObjectFromBucketAsync(
        //            S3.Instance.s3Client,
        //            "divinebucket5487",
        //            card.GetCardSO().RedGiantImageURL,
        //            ADDRESS_CARD_GRAPHICS
        //            );
        characterImage.sprite = Resources.Load<Sprite>("redGiant" + card.GetCardSO().Title);
        //Image2SpriteUtility.Instance.LoadNewSprite(ADDRESS_CARD_GRAPHICS + "\\" + card.GetCardSO().RedGiantImageURL);
    }


    private void SelectTarget()
    {
        Debug.Log("SelectTarget");
        BaseCard selectTargetInitiatorCard = CardGameManager.Instance.GetSelectTargetInitiator();

        if(selectTargetInitiatorCard.GetCardSO().GiantEffect.Effect == EffectTypeEnum.Uphold)
        {
            Debug.Log("CardGameManager.Instance.GetInitiatorIndex(): " + CardGameManager.Instance.GetInitiatorIndex());
            Debug.Log("((ExpertCard)card).transform.parent.GetSiblingIndex(): " + ((ExpertCard)card).transform.parent.GetSiblingIndex());
            if (CardGameManager.Instance.GetInitiatorIndex() == ((ExpertCard)card).transform.parent.GetSiblingIndex()) return;
        }
        //This will select the viable target differently whether it's a expert of subterfuge card
        //very cool
        ViableTarget viableTarget = selectTargetInitiatorCard.GetViableTarget();

        if (viableTarget.Locations.Contains(card.GetCardGameArea())
            && viableTarget.Ranks.Contains(card.GetCardSO().Rank))
        {
            selectTargetInitiatorCard.TargetSelected((ICard)card);
            CardGameManager.Instance.DoneSelectingTarget();
        }
    }
    virtual public void Disintegrate(float directionX)
    {

    }
    //ugly  cardUI really shouldn't be concerned with playing the card itself
    virtual protected void CardUI_OnAnyPlayCard(object sender, OnPlayCardEventArgs e)
    {
        bool useSkill = false;
        if (e.card != card) return;
        //ugly very bad
        if (SceneManager.GetActiveScene().name == SceneLoader.Scene.DeckEditorScene.ToString())
        {
            if (card.GetCardGameArea() == GameAreaEnum.CardPreview)
                return;

            ((BaseCardLocal)card).DeckEditorSceneSelected();
        }
        if (CardGameManager.Instance == null) 
        {
            card.InspectCard();
        } 
        else 
        { 
            //in game scene
            

            
            if(CardGameManager.Instance.IsSelectingTarget()) 
            {
                if (card is BaseCard)
                {
                    if (((BaseCard)card).IsTargetable())
                    {
                        SelectTarget();
                    }
                }
                else if(card is BaseCardLocal)
                {
                    SelectTarget();
                    Debug.Log("CARDUI, SELECTTARGET");
                    return;
                }
            
            }
            else if (card is ExpertCard && ((ExpertCard)card).CanUseSkill())
            {
               
                ((ExpertCard)card).UseSkill();
                useSkill = true;
                
            }
            else if ((card.GetCardGameArea() == GameAreaEnum.Field
                || card.GetCardGameArea() == GameAreaEnum.OpponentField)
                && !CardGameManager.Instance.IsSelectingTarget()
                && !useSkill)
            {

                card.InspectCard();
            }
            if (card.GetCardGameArea() == GameAreaEnum.Hand
             && CardGameManager.Instance.IsMyTurn()
             //Don't let them play cards before the current cards have aged and whatnot will mess up the turn phase switching
             && TurnPhaseStateMachine.Instance.GetTurnPhase() == TurnPhaseStateMachine.State.BlackHoleSuck)
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
            case GameAreaEnum.CardPreview:
                AdaptLookForCardPreview();
                break;
            case GameAreaEnum.Catalogue:
                AdaptLookForCatalogue();
                break;

        }
    }
    private void AdaptLookForHand()
    {

        titleRectTransform.anchoredPosition = new Vector2(0, 45.3f);
        titleText.fontSize = 25;
        if (titleText.text.Length > 8)
        {
            titleText.fontSize = 19;
        }else if(titleText.text.Length > 7)
        {
            titleText.fontSize = 23.5f;
        }
        titleText.gameObject.SetActive(true);
        ;
        
        cardRectTransform.localScale = new Vector2(UniversalConstants.HAND_CARD_SCALE, UniversalConstants.HAND_CARD_SCALE);
        if (SceneManager.GetActiveScene().name == SceneLoader.Scene.DeckEditorScene.ToString())
            counterText.SetActive(true);
    }
    private void AdaptLookForInspect()
    {
        titleRectTransform.anchoredPosition = new Vector2(0, 110f);
        titleText.fontSize = 15;
        if (titleText.text.Length > 8)
        {
            titleText.fontSize = 12;
        }
        titleText.gameObject.SetActive(true);
        cardRectTransform.localScale = new Vector2(UniversalConstants.INSPECT_CARD_SCALE, UniversalConstants.INSPECT_CARD_SCALE);
        if (SceneManager.GetActiveScene().name != SceneLoader.Scene.GameScene.ToString())
            counterText.SetActive(false);
    }
    private void AdaptLookForCardPreview()
    {
        titleRectTransform.anchoredPosition = new Vector2(0, 110f);
        titleText.fontSize = 15;
        counterText.SetActive(false);
        if (titleText.text.Length > 8)
        {
            titleText.fontSize = 12;
        }
        titleText.gameObject.SetActive(true);
        cardRectTransform.localScale = new Vector2(UniversalConstants.CARD_PREVIEW_SCALE, UniversalConstants.CARD_PREVIEW_SCALE);
        if (SceneManager.GetActiveScene().name != SceneLoader.Scene.GameScene.ToString())
            counterText.SetActive(false);
    }
    private void AdaptLookForCatalogue()
    {
        titleRectTransform.anchoredPosition = new Vector2(0, 45.3f);
        titleText.fontSize = 27;
        if (titleText.text.Length > 8)
        {
            titleText.fontSize = 19;
        }
        else if (titleText.text.Length > 7)
        {
            titleText.fontSize = 22;
        }
        titleText.gameObject.SetActive(true);
        cardRectTransform.localScale = new Vector2(UniversalConstants.CARD_CATALOGUE_SCALE, UniversalConstants.CARD_CATALOGUE_SCALE);
        if (SceneManager.GetActiveScene().name != SceneLoader.Scene.GameScene.ToString())
            counterText.SetActive(true);
    }
    private void AdaptLookForField()
    {
        titleRectTransform.anchoredPosition = new Vector2(0, 45.3f);
        titleText.fontSize = 25;
        if (titleText.text.Length > 8)
        {
            titleText.fontSize = 19f;
        }else if(titleText.text.Length > 7)
        {
            titleText.fontSize = 23f;
        }
        titleText.gameObject.SetActive(true);
        cardRectTransform.localScale = new Vector2(UniversalConstants.FIELD_CARD_SCALE, UniversalConstants.FIELD_CARD_SCALE);
    }
    private void AdaptLookForOpponentField()
    {
        titleRectTransform.anchoredPosition = new Vector2(0, 45.3f);
        titleText.fontSize = 25;
        if(titleText.text.Length > 8)
        {
            titleText.fontSize = 19f;
        }
        else if (titleText.text.Length > 7)
        {
            titleText.fontSize = 23f;
        }
        titleText.gameObject.SetActive(true);
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

    internal void BecomeWhiteDwarf()
    {
        if (((BaseCard)card).GetCardOwner() == Player.Instance.IAm())
        {
            ((BaseCard)card).BecomeWhiteDwarfServerRpc();
        }

        ((BaseCard)card).GetHighlight().Hide();
        ((BaseCard)card).MakeDead();
        becomeWhiteDwarfAnimation.BecomeWhiteDwarf();

    }
    internal void BeSuckedByBlackHole()
    {
        ((BaseCard)card).GetHighlight().Hide();
        ((BaseCard)card).MakeDead();
        beSuckedByBlackHoleAnimation.BeSuckedByBlackHole();
    }

    public void BecomeNeutronStar(int stardustGain)
    {
        if (((BaseCard)card).GetCardOwner() == Player.Instance.IAm())
        {
            ((BaseCard)card).BecomeNeutronStarServerRpc(stardustGain);
        }

        ((BaseCard)card).GetHighlight().Hide();
        ((BaseCard)card).MakeDead();
        neutronStarAnimation.Supernova(stardustGain);
    }

    public void BecomeBlackHole(int stardustGain, bool meganova = false)
    {
        if (CardGameManager.Instance.WillWipeOutBothField())
        {
            meganova = true;
        }
        if (((BaseCard)card).GetCardOwner() == Player.Instance.IAm())
        {
            ((BaseCard)card).BecomeBlackHoleServerRpc(stardustGain, meganova);
        }
        ((BaseCard)card).GetHighlight().Hide();
        ((BaseCard)card).MakeDead();
        blackHoleAnimation.Supernova(stardustGain, false, meganova);
    }
    public void SupernovaNoBlackHole(int stardustGain = 0, bool meganova = false)
    {
        if (CardGameManager.Instance.WillWipeOutBothField())
        {
            meganova = true;
        }
        if (((BaseCard)card).GetCardOwner() == Player.Instance.IAm())
        {
            ((BaseCard)card).SupernovaNoBlackHoleServerRpc(stardustGain, meganova);
        }
        ((BaseCard)card).GetHighlight().Hide();
        ((BaseCard)card).MakeDead();
        blackHoleAnimation.Supernova(stardustGain, true, meganova);
    }
    public void Strangify()
    {
        strangifyAnimation.Strangify();
    }
    public GlowShaderController GetLightGlowShaderController()
    {
        return lightIcon;
    }
    public void ShowEmerald()
    {
        emerald.SetActive(true);
    }
    public void ShowAmethyst() { 
        amethyst.SetActive(true);
}
    public void ShowJade()
    {
        jade.SetActive(true);
    }
    public Image GetLightImage()
    {
        return lightIcon.GetComponent<Image>();
    }
}
