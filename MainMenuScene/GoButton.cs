using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class GoButton : MonoBehaviour
{
    [SerializeField] private PlayRankedButton playRankedButton;
    [SerializeField] private SetButton setNewActiveDeckButton;
    private Button button;
    private TextMeshProUGUI text;
    private bool canEnableButton = false;
    private bool playRanked;
    private async void Awake()
    {
        button = GetComponent<Button>();
        text = button.GetComponentInChildren<TextMeshProUGUI>();
        setNewActiveDeckButton.OnSetNewActiveDeckButtonPressed += SetNewActiveDeckButton_OnSetNewActiveDeckButtonPressed;
        button.interactable = false;
        text.alpha = .5f;
        //button.onClick.AddListener(() =>
        //{
        //    if (playRanked)
        //    {
        //        Debug.Log("playRanked");
        //        LobbyManager.Instance.QuickJoin();
        //    }
        //    else
        //    {

        //    }
        //});
        string playerActiveDeck = "";
#if !DEDICATED_SERVER
        playerActiveDeck  = await LambdaManager.Instance.GetPlayerActiveDeckLambda();
#endif
        //ugly do I even care anymore
        if(playerActiveDeck != "" && playerActiveDeck != "\"error\":{}" && button != null)
        {
            canEnableButton = true;
        }
    }
    public void CheckIfCanEnable()
    {
        if(canEnableButton)
        {
            EnableButton();
        }
    }
    private void SetNewActiveDeckButton_OnSetNewActiveDeckButtonPressed(object sender, System.EventArgs e)
    {
        DisableButton();
    }

    public void EnableButton()
    {
        button.interactable = true;
        text.alpha = 1f;
    }
    private void DisableButton()
    {
        button.interactable = false;
        text.alpha = .5f;
    }
    private void Start()
    {
        playRankedButton.OnPlayRankedButtonPressed += PlayRankedButton_OnPlayRankedButtonPressed;
    }

    private void PlayRankedButton_OnPlayRankedButtonPressed(object sender, System.EventArgs e)
    {
        playRanked = true;
    }
    private void OnDestroy()
    {
        playRankedButton.OnPlayRankedButtonPressed -= PlayRankedButton_OnPlayRankedButtonPressed;
    }
}
