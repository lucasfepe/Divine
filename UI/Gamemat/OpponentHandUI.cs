using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class OpponentHandUI : NetworkBehaviour
{
    public static OpponentHandUI Instance { get; private set; }
    [SerializeField] private Image cardBack;
    [SerializeField] private LoadingTextUI loadingText;
    public int numberCardsInOpponentHand;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DivineMultiplayer.Instance.OnHandCardsNumberChanged += DivineMultiplayer_OnHandCardsNumberChanged;
    }
    

    public void DivineMultiplayer_OnHandCardsNumberChanged(object sender, DivineMultiplayer.OnHandCardsNumberChangedEventArgs e)
    {
        //OpponentHandResponsive.Instance.RespondOnce();
        //if player one drew a card and this is player two then change the opponent hand ui
        if(e.playerEnum == PlayerEnum.PlayerOne 
            && Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            //need this as destroying a child won't change the childCount until next
            //frame resulting in infinite loop
            //minus one because the loading component is also counted
            //but wait the instantiating works fine without it...
            numberCardsInOpponentHand = transform.childCount - 1;
            while (DivineMultiplayer.Instance.playerOneHandCards.Value < numberCardsInOpponentHand)
            {
                Image image = GetComponentInChildren<Image>();
                if(image == null)
                {
                    return;
                }
                Destroy(image.gameObject);
                numberCardsInOpponentHand--;
            }
            while(DivineMultiplayer.Instance.playerOneHandCards.Value > numberCardsInOpponentHand)
            {
                numberCardsInOpponentHand++;
                Image newCard = Instantiate(cardBack, transform);
               
            }
            
            
        }
        else if(e.playerEnum == PlayerEnum.PlayerTwo
            && Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            numberCardsInOpponentHand = transform.childCount - 1;
            while (DivineMultiplayer.Instance.playerTwoHandCards.Value < numberCardsInOpponentHand)
            {
                numberCardsInOpponentHand--;
                Destroy(GetComponentInChildren<Image>().gameObject);
            }
            while (DivineMultiplayer.Instance.playerTwoHandCards.Value > numberCardsInOpponentHand)
            {
                numberCardsInOpponentHand++;
                Image newCard = Instantiate(cardBack, transform);
               
            }
            
                
        }
        //Image[] cardBacks = GetComponentsInChildren<Image>();
        //switch (numberCardsInOpponentHand)
        //{
        //    case 1:
        //        cardBacks[0].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
        //        break;
        //    case 2:
        //        cardBacks[0].GetComponent<RectTransform>().rotation = Quaternion.Euler(0,0,3);
        //        cardBacks[1].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -3);
        //        break;
        //    case 3:
        //        cardBacks[0].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 6);
        //        cardBacks[1].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
        //        cardBacks[2].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -6);
        //        break;
        //    case 4:
        //        cardBacks[0].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 9);
        //        cardBacks[1].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 3);
        //        cardBacks[2].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -3);
        //        cardBacks[3].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -9);
        //        break;
        //    case 5:
        //        cardBacks[0].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 12);
        //        cardBacks[1].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 6);
        //        cardBacks[2].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
        //        cardBacks[3].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -6);
        //        cardBacks[4].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -12);
        //        break;
        //}
    }
}
