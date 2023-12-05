using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PageIndexText : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Awake()
    {
         text =  GetComponent<TextMeshProUGUI>();

    }
    private void Start()
    {
        DisplayCollectionAreaContent.Instance.OnDetermineNumberOfPages += CardCatalogue_OnDetermineNumberOfPages;
        DisplayCollectionAreaContent.Instance.OnPageIndexChanged += CardCatalogue_OnPageIndexChanged;
    }

    private void CardCatalogue_OnPageIndexChanged(object sender, System.EventArgs e)
    {
        text.text = DisplayCollectionAreaContent.Instance.GetPageIndex() + "/" + DisplayCollectionAreaContent.Instance.GetNumberOfPages();
    }

    private void CardCatalogue_OnDetermineNumberOfPages(object sender, System.EventArgs e)
    {
        text.text = "1/" + DisplayCollectionAreaContent.Instance.GetNumberOfPages();
    }
}
