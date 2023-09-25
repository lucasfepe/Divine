using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnUI : MonoBehaviour
{
    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI endTurnButtonText;
    

    private void Awake()
    {
        endTurnButton.onClick.AddListener(() =>
        {
            CardGameManager.Instance.EndTurn();
        });
        DisableButton();
    }

    private void Start()
    {
        CardGameManager.Instance.OnBeginTurn += CardGameManager_OnBeginTurn;
        CardGameManager.Instance.OnEndTurn += CardGameManager_OnEndTurn;
    }

    //Is this repetitive could I do it different
    //bot on start match and on start turn do the same thing here but they are invoked in different places
    //so i don't think I could do with just one
    

    private void CardGameManager_OnEndTurn(object sender, System.EventArgs e)
    {
        DisableButton();
    }

    private void CardGameManager_OnBeginTurn(object sender, System.EventArgs e)
    {
        EnableButton();
    }

    private void DisableButton()
    {
        endTurnButton.interactable = false;
        endTurnButtonText.alpha = .5f;
    }
    private void EnableButton()
    {
        endTurnButton.interactable = true;
        endTurnButtonText.alpha = 1.0f;
    }
}
