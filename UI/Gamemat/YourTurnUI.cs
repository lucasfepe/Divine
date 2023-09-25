using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YourTurnUI : MonoBehaviour
{
    private float showTimer = 0f;
    private float showTimerMax = 1f;
    private bool isShowing = false;

    private void Start()
    {
        CardGameManager.Instance.OnBeginTurn += CardGameManager_OnStartTurn;
        CardGameManager.Instance.OnWin += CardGameManager_OnWin;
        Hide();
    }

    private void CardGameManager_OnWin(object sender, CardGameManager.OnWinEventArgs e)
    {
        Hide();
    }

    private void Update()
    {
        if (isShowing)
        {
            showTimer += Time.deltaTime;
            if(showTimer > showTimerMax )
            {
                showTimer = 0f;
                isShowing = false;
                Hide();
            }
        }
    }
    //Is it fine to assign same function to two subscribers?
    
    private void CardGameManager_OnStartTurn(object sender, System.EventArgs e)
    {
        if (CardGameManager.Instance.IsMyTurn())
        {
            if (!CardGameManager.Instance.IsGameOver()) { 
            Show();
            isShowing = true;
            }
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
