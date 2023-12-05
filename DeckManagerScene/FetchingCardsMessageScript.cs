using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FetchingCardsMessageScript : MonoBehaviour
{
    private void Start()
    {
        CardInventory.Instance.OnMakeCards += Instance_OnMakeCards; ;
    }

    private void Instance_OnMakeCards(object sender, System.EventArgs e)
    {
        Hide();
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
