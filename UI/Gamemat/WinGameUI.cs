using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinGameUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI matchOutcomeText;
    [SerializeField] private Image matchOutcomeImage;
    private void Start()
    {
        CardGameManager.Instance.OnWin += CardGameManager_OnWin;
        Hide();
    }

    private void CardGameManager_OnWin(object sender, CardGameManager.OnWinEventArgs e)
    {
        if(Player.Instance.IAm() == e.playerEnum)
        {
            matchOutcomeText.text = "YOU\nWIN!";
            matchOutcomeImage.sprite = Resources.Load<Sprite>("yellowStoneShinyPanel");
            Show();
            LambdaManager.Instance.WinMatchLambda();
        }
        else
        {
            matchOutcomeText.text = "YOU\nLOOSE";
            matchOutcomeImage.sprite = Resources.Load<Sprite>("circleSilver");
            Show();
            LambdaManager.Instance.LooseMatchLambda();
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
