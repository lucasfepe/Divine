using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterButton : MonoBehaviour
{
    [SerializeField] private GameObject sortFilterUI;
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            sortFilterUI.SetActive(true);
        });
    }
}
