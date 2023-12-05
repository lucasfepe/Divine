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
        TurnPhaseStateMachine.Instance.OnTurnPhase_BlackHoleSuck += OnTurnPhase_BlackHoleSuck;
        CardGameManager.Instance.OnEndTurn += CardGameManager_OnEndTurn;
        CardGameManager.Instance.OnBeginSelectingTarget += CardGameManager_OnBeginSelectingTarget;
        CardGameManager.Instance.OnDoneSelectingTarget += CardGameManager_OnDoneSelectingTarget;
    }

    private void OnTurnPhase_BlackHoleSuck(object sender, System.EventArgs e)
    {
        EnableButton();
    }

    private void CardGameManager_OnDoneSelectingTarget(object sender, System.EventArgs e)
    {
        endTurnButton.onClick.RemoveAllListeners();
        endTurnButton.onClick.AddListener(() =>
        {
            CardGameManager.Instance.EndTurn();
        });
        endTurnButtonText.fontSize = 45;
        endTurnButtonText.text = "End\nTurn";
    }

    private void CardGameManager_OnBeginSelectingTarget(object sender, System.EventArgs e)
    {
        endTurnButton.onClick.RemoveAllListeners();
        endTurnButton.onClick.AddListener(() =>
        {
            CardGameManager.Instance.CancelSkill();
        });
        endTurnButtonText.fontSize = 38;
        endTurnButtonText.text = "Cancel\nSkill";
    }

    //Is this repetitive could I do it different
    //bot on start match and on start turn do the same thing here but they are invoked in different places
    //so i don't think I could do with just one


    private void CardGameManager_OnEndTurn(object sender, System.EventArgs e)
    {
        DisableButton();
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
