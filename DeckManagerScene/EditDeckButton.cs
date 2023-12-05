using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditDeckButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            DeckManagerStatic.SetDeckToEdit(DecksManager.Instance.GetSelectedDeckTitle());
            SceneLoader.Load(SceneLoader.Scene.DeckEditorScene);
        });
    }
}
