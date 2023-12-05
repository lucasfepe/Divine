using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayRankedButton : MonoBehaviour
{
    public event EventHandler OnPlayRankedButtonPressed;
    private void Start()
    {

        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnPlayRankedButtonPressed?.Invoke(this, EventArgs.Empty);
        });
    }

   
}
