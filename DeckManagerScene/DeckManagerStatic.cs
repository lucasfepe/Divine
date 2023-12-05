using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DeckManagerStatic
{
    private static string deckToEditName;

    public static void SetDeckToEdit(string deckToEditNameParam)
    {
        deckToEditName = deckToEditNameParam;
    }
    public static string GetDeckToEdit()
    {
        return deckToEditName;
    }
}
