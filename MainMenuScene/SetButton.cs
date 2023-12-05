using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SetButton : MonoBehaviour
{

    [SerializeField] private SelectDecksAreaContent container;
    

    public event EventHandler OnSetNewActiveDeckButtonPressed;

    private void Awake()
    {
        
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnSetNewActiveDeckButtonPressed?.Invoke(this, EventArgs.Empty);
        });
    }
}
