using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragCardMessageText : MonoBehaviour
{
    [SerializeField] private DeckEditorAreaContent deckEditorAreaContent;

    private void Awake()
    {
        deckEditorAreaContent.OnDeckEditorCardChange += DeckEditorAreaContent_OnDeckEditorCardChange;
    }
    private void OnDestroy()
    {
        deckEditorAreaContent.OnDeckEditorCardChange -= DeckEditorAreaContent_OnDeckEditorCardChange;
    }
    private void DeckEditorAreaContent_OnDeckEditorCardChange(object sender, System.EventArgs e)
    {
        if(deckEditorAreaContent.transform.childCount != 0)
        {
            Hide();
        }
        else
        {
            Show();
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
}
