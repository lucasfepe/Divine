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
                    if (isIncreasing)
                    {
                        text.text = (++value).ToString();
                    }else
                    {
                        text.text = (--value).ToString();
                    }
                    subTimerCounter = subTimer;
                }
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
