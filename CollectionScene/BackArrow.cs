using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackArrow : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            DisplayCollectionAreaContent.Instance.PageBack();
        });
        button.interactable = false;
    }
    private void Start()
    {
        DisplayCollectionAreaContent.Instance.OnPageIndexChanged += Catalogue_OnPageIndexChanged;
    }

    private void Catalogue_OnPageIndexChanged(object sender, System.EventArgs e)
    {
        if(DisplayCollectionAreaContent.Instance.GetPageIndex() == 1)
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        };
    }
}
