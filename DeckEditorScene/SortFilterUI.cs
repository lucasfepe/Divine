using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortFilterUI : MonoBehaviour
{
    public static SortFilterUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Hide();
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }


}
