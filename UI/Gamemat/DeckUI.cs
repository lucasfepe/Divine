using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckUI : MonoBehaviour
{
    [SerializeField] private Image cardBack;
    [SerializeField] private Transform cards;
    [SerializeField] private TextMeshProUGUI numberCardsText;
    
    
    private void Start()
    {
        PlayerDeck.Instance.OnFetchActiveDeck += PlayerDeck_OnFetchActiveDeck;
        PlayerHand.Instance.OnDrawCard += PlayerHand_OnDrawCard;
    }

    private void PlayerHand_OnDrawCard(object sender, System.EventArgs e)
    {
        Destroy(cards.GetChild(cards.childCount - 1).gameObject);
        numberCardsText.transform.localPosition = new Vector2(cards.childCount * UniversalConstants.CARD_PILE_OFFSET, cards.childCount * UniversalConstants.CARD_PILE_OFFSET);
        numberCardsText.text = PlayerDeck.Instance.totalCards.ToString();

    }

    private void PlayerDeck_OnFetchActiveDeck(object sender, System.EventArgs e)
    {
        
        for(int i = 1;i < PlayerDeck.Instance.totalCards;i++)
        {
            Image image = Instantiate(cardBack, cards);
            image.transform.localPosition = new Vector2(UniversalConstants.CARD_PILE_OFFSET * i, UniversalConstants.CARD_PILE_OFFSET * i);
        }
        numberCardsText.text = PlayerDeck.Instance.totalCards.ToString();
        numberCardsText.transform.localPosition = new Vector2((PlayerDeck.Instance.totalCards - 1) * UniversalConstants.CARD_PILE_OFFSET, (PlayerDeck.Instance.totalCards - 1) * UniversalConstants.CARD_PILE_OFFSET);
    }
}
