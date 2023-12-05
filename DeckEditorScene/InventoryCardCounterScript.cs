using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryCardCounterScript : MonoBehaviour
{

    private TextMeshProUGUI text;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        CardInventory.Instance.OnMakeCards += CardInventory_OnMakeCards;
    }

    private void CardInventory_OnMakeCards(object sender, System.EventArgs e)
    {
        text.text = "Cards: " + CardInventory.Instance.GetInventoryCardsCount().ToString();
    }
}
