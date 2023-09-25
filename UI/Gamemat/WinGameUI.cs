using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinGameUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI matchOutcomeText;
    private void Start()
    {
        CardGameManager.Instance.OnWin += CardGameManager_OnWin;
        Hide();
    }

    private void CardGameManager_OnWin(object sender, CardGameManager.OnWinEventArgs e)
    {
        if(Player.Instance.IAm() == e.playerEnum)
        {
            matchOutcomeText.text = "YOU WIN!";
            Show();
        }
        else
        {
            matchOutcomeText.text = "YOU LOOSE";
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
