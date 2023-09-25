using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{

    private void Start()
    {
        OpponentHandResponsive.Instance.OnLoad += OpponentHandResponsive_OnLoad;
    }

    private void OpponentHandResponsive_OnLoad(object sender, System.EventArgs e)
    {
        Hide();
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
