using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatTextMessageScriptUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statMessageText;
    [SerializeField] private Image statIcon;

    public void SetMessage(string message)
    {
        statMessageText.text = message;
    }

    public void SetImage(Sprite icon)
    {
        if (icon != null) { 
        statIcon.sprite = icon;
        }
        else
        {
            statIcon.enabled = false;
        }
    }
}
