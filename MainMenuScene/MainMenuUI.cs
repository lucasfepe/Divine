using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button deckManagerButton;
    [SerializeField] private Button playRankedButton;
    [SerializeField] private Button playUnrankedButton;
    [SerializeField] private Button cardCatalogueButton;

    private void Awake()
    {
        deckManagerButton.onClick.AddListener(() => {
            SceneLoader.Load(SceneLoader.Scene.DeckManagerScene);
        });
        cardCatalogueButton.onClick.AddListener(() =>
        {
            SceneLoader.Load(SceneLoader.Scene.CardCatalogueScene);
        });
        
    }
    private void Start()
    {
#if DEDICATED_SERVER
        Debug.Log("DEDICATED_SERVER !@@@@@@@@@@");
#endif
    }

}
