using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewDeckButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            DeckManagerStatic.SetDeckToEdit(null);
            SceneLoader.Load(SceneLoader.Scene.DeckEditorScene);
        });
    }
}
