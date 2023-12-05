using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForwardArrow : MonoBehaviour
{
    private Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            DisplayCollectionAreaContent.Instance.PageForward();
        });
        button.interactable = false;

    }
    private void Start()
    {
        DisplayCollectionAreaContent.Instance.OnPageIndexChanged += CardCatalogue_OnPageIndexChanged;
        DisplayCollectionAreaContent.Instance.OnDetermineNumberOfPages += CardCatalogue_OnDetermineNumberOfPages;
    }

    private void CardCatalogue_OnDetermineNumberOfPages(object sender, System.EventArgs e)
    {
        if(DisplayCollectionAreaContent.Instance.GetNumberOfPages() > 1)
        {
            button.interactable = true;
        }
    }

    private void CardCatalogue_OnPageIndexChanged(object sender, System.EventArgs e)
    {
        if(DisplayCollectionAreaContent.Instance.GetNumberOfPages() == DisplayCollectionAreaContent.Instance.GetPageIndex())
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }
}
