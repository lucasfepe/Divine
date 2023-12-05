using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewMessageTextScript : MonoBehaviour
{
    private void Start()
    {
        DecksManager.Instance.OnChangedSelectedDeck += DecksManager_OnChangedSelectedDeck;
        if(DisplayCollectionAreaContent.Instance != null)
        {
            DisplayCollectionAreaContent.Instance.OnSelectCollection += CardCatalogue_OnSelectCollection;
        }
        
    }

    private void CardCatalogue_OnSelectCollection(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void DecksManager_OnChangedSelectedDeck(object sender, System.EventArgs e)
    {
        if(DecksManager.Instance.GetSelectedDeckTitle() == null || DecksManager.Instance.GetSelectedDeckTitle() == "")
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        DecksManager.Instance.OnChangedSelectedDeck -= DecksManager_OnChangedSelectedDeck;
    }
}
