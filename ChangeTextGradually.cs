using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class ChangeTextGradually : MonoBehaviour
{
    private TextMeshProUGUI text;

    private float totalTimer;
    private float subTimer;
    private float subTimerCounter;
    private bool isChanging = false;
    private bool isIncreasing = false;
    private int value=0;
    private int newValue=0;
    public bool AdjustFontSizeBool = false;

    public event EventHandler OnUpdateStatTextGradually;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        
            if (newValue != value) { 
                subTimerCounter -= Time.deltaTime;
                if(subTimerCounter <= 0)
                {
                int lengthb4change = text.text.Length;
                    if (isIncreasing)
                    {
                        
                        text.text = (++value).ToString();
                        
                    }
                else
                    {
                        text.text = (--value).ToString();
                    }
                int lengthafterchange = text.text.Length;
                if(AdjustFontSizeBool && lengthafterchange !=  lengthb4change)
                {
                    AdjustFontSize();
                }
                
                if (newValue == value) {
                OnUpdateStatTextGradually?.Invoke(this, EventArgs.Empty);
                }
                subTimerCounter = subTimer;
                }
        }
        


    }
    private void AdjustFontSize()
    {
        if (text.text.Length > 1)
        {
            text.fontSize = 45;
            text.characterSpacing = -10;
        }
        else if (text.text.Length == 1)
        {
            text.fontSize = 60;
            text.characterSpacing = 0;
        }
    }
    public void SetText(float duration, int newValue)
    {
        this.newValue = newValue;
        totalTimer = duration;
        value = System.Int32.Parse(text.text);
        int difference = Mathf.Abs(newValue - value);
        subTimer = totalTimer / (difference + 1);
        subTimerCounter = subTimer;
        isChanging = true;
        if(newValue > value)
        {
            isIncreasing = true;
        }else if(newValue < value)
        {
            isIncreasing = false;
        }
        
    }

    




}
