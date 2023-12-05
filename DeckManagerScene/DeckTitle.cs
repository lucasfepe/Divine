using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckTitle : MonoBehaviour
{
    private TMP_InputField inputField;
    private string deckToEdit;
    public static DeckTitle Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        deckToEdit = DeckManagerStatic.GetDeckToEdit();
        DecksManager.Instance.OnFetchDecks += DecksManager_OnFetchDecks;
        
    }

    private void DecksManager_OnFetchDecks(object sender, System.EventArgs e)
    {
        if (deckToEdit != null)
        {
            inputField.text = deckToEdit;
        }
        else
        {
            string newDeckTitle = "New Deck";
            Decks decks = DecksManager.Instance.GetDecks();
            List<string> existingDeckNames = decks.decks.Select(deck => deck.name).ToList();
            int i = 1;
            while (existingDeckNames.Contains(newDeckTitle + " " + i))
            {
                i++;
            }


            inputField.text = newDeckTitle + " " + i;
        }
    }

    public string GetDeckTitle()
    {
        return inputField.text;
    }
}
