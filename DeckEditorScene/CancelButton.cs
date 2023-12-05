using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CancelButton : MonoBehaviour
{
    private void Awake()
    {




        GetComponent<Button>().onClick.AddListener(() =>
        {
            
            SortFilterUI.Instance.Hide();

        });
    }
}
