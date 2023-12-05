using Amazon.Runtime.Internal.Util;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoneButton : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown rankDropdown;
    [SerializeField] private TMP_Dropdown rarityDropdown;
    [SerializeField] private TMP_Dropdown skilltypeDropdown;
    [SerializeField] private TMP_InputField nameInputField;

    [SerializeField] private ToggleGroup sortCriterion;
    [SerializeField] private ToggleGroup sortOrder;

    private void Awake()
    {
       
      

        
        GetComponent<Button>().onClick.AddListener(() =>
        {
            CardInventory.Instance.SortAndFilter(
                sortCriterion.ActiveToggles().FirstOrDefault().name,
                sortOrder.ActiveToggles().FirstOrDefault().name,
                rankDropdown.options[rankDropdown.value].text,
                rarityDropdown.options[rarityDropdown.value].text,
                skilltypeDropdown.options[skilltypeDropdown.value].text,
                nameInputField.text
                );
            SortFilterUI.Instance.Hide();
           
        });
    }

  
}
