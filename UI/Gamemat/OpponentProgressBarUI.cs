using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpponentProgressBarUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image positiveProgressImage;
    [SerializeField] private Image negativeProgressImage;

    

    private void Awake()
    {
        progressText.text = 0.ToString();
        positiveProgressImage.fillAmount = 0;
        negativeProgressImage.fillAmount = 0;
    }
    private void Start()
    {
        
        DivineMultiplayer.Instance.OnPlayerProgressChanged += DivineMultiplayer_OnPlayerProgressChanged;
    }

    private void DivineMultiplayer_OnPlayerProgressChanged(object sender, DivineMultiplayer.OnPlayerProgressChangedEventArgs e)
    {
        //if this is player two and player one progress changed then update the opponent progress bar and ivce bersa
        if(e.playerEnum == PlayerEnum.PlayerOne
            && Player.Instance.IAm() == PlayerEnum.PlayerTwo)
        {
            int progress = DivineMultiplayer.Instance.playerOneProgress.Value;
            progressText.text = progress.ToString();
            if (progress > 0)
            {
                negativeProgressImage.fillAmount = 0;
                positiveProgressImage.fillAmount = (float)progress / CardGameManager.PROGRESS_BAR_MAX;
            }
            else
            {
                positiveProgressImage.fillAmount = 0;
                negativeProgressImage.fillAmount = -1 * (float)progress / CardGameManager.PROGRESS_BAR_MAX;
            }
        }
        else if (e.playerEnum == PlayerEnum.PlayerTwo
            && Player.Instance.IAm() == PlayerEnum.PlayerOne)
        {
            int progress = DivineMultiplayer.Instance.playerTwoProgress.Value;
            progressText.text = progress.ToString();
            if (progress > 0)
            {
                negativeProgressImage.fillAmount = 0;
                positiveProgressImage.fillAmount = (float)progress / CardGameManager.PROGRESS_BAR_MAX;
            }
            else
            {
                positiveProgressImage.fillAmount = 0;
                negativeProgressImage.fillAmount = -1 * (float)progress / CardGameManager.PROGRESS_BAR_MAX;
            }
        }
    }

   

    
}
