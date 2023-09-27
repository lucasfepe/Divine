using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OpponentDeckUI : NetworkBehaviour
{
    
    [SerializeField] private Image cardBack;
    [SerializeField] private Transform cards;
    [SerializeField] private TextMeshProUGUI numberCardsText;
    

    public static OpponentDeckUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DivineMultiplayer.Instance.OnDeckCardsNumberChanged += DivineMultiplayer_OnDeckCardsNumberChanged;
        //Hide();
        //OpponentHandResponsive.Instance.OnLoad += OpponentHandResonsive_OnLoad;
    }

    //private void OpponentHandResonsive_OnLoad(object sender, System.EventArgs e)
    //{
    //    Show();
    //}

    public void DivineMultiplayer_OnDeckCardsNumberChanged(object sender, DivineMultiplayer.OnDeckCardsNumberChangedEventArgs e)
    {
        //OpponentHandResponsive.Instance.RespondOnce();
        int totalCards = -1;
        if (e.playerEnum == PlayerEnum.PlayerOne 
            && Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            
            totalCards = DivineMultiplayer.Instance.playerOneDeckCards.Value;
          
        }
        else if (e.playerEnum == PlayerEnum.PlayerTwo
            && Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            totalCards = DivineMultiplayer.Instance.playerTwoDeckCards.Value;
        }
        else
        {
            //if say player two's deck card number changed (e.playerEnum = plaeryTwo)
            //this method will run on the player two client but totalCards will remain -1
            //don't want to run next bit for player two itself only player one otherwise 
            //will set player two's opponent deck card number to -1
            return;
        }
        
        numberCardsText.text = totalCards.ToString();
        for (int i = 1; i < totalCards; i++)
        {
            Image image = Instantiate(cardBack, cards);
            image.transform.localPosition = new Vector2(UniversalConstants.CARD_PILE_OFFSET * i, UniversalConstants.CARD_PILE_OFFSET * i);
            //ugly don't do this instead use mehtod on an object don't pass object
            
        }
        numberCardsText.transform.localPosition =
            new Vector2((PlayerDeck.Instance.totalCards - 1) * UniversalConstants.CARD_PILE_OFFSET, (PlayerDeck.Instance.totalCards - 1) * UniversalConstants.CARD_PILE_OFFSET);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }

}
