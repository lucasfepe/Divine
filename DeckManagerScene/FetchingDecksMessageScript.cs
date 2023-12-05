using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FetchingDecksMessageScript : MonoBehaviour
{
    private void Start()
    {
        DecksManager.Instance.OnFetchDecks += DecksManager_OnFetchDecks;
    }

    private void DecksManager_OnFetchDecks(object sender, System.EventArgs e)
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
    private void OnDestroy()
    {
        DecksManager.Instance.OnFetchDecks -= DecksManager_OnFetchDecks;
    }
}
