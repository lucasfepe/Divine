using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionButton : MonoBehaviour
{
    [SerializeField] private CollectionsEnum collection;
    [SerializeField] private DisplayCollectionAreaContent displayCollectionAreaContent;
    private Button button;
    private List<TextMeshProUGUI> text;
    public static event EventHandler OnSelectCollection;
        

    private void Awake()
    {
        button = GetComponent<Button>();
        text = GetComponentsInChildren<TextMeshProUGUI>().ToList();
        button.interactable = false;
        text.ForEach(x => x.alpha = .5f);
        button.onClick.AddListener(() => { 
            displayCollectionAreaContent.SelectCollection(collection);
            OnSelectCollection?.Invoke(this, EventArgs.Empty);
        });
        displayCollectionAreaContent.OnReadyToShowCollections += CardCatalogue_OnPopulateCardInventory;

    }
   

    private void CardCatalogue_OnPopulateCardInventory(object sender, System.EventArgs e)
    {
        button.interactable = true;
        text.ForEach(x => x.alpha = 1f);
    }
}
