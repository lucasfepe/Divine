using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Message : MonoBehaviour
{
    private Animator animator;
    private TextMeshProUGUI messageText;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        messageText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void ShowMessage()
    {
        animator.SetTrigger("Show");
    }
    private void HideMessage()
    {
        animator.SetTrigger("Hide");
    }

    public void SetMessageText(string message)
    {
        messageText.fontSize = 36;
        if (message.Length > 50)
        {
            messageText.fontSize = 25;
        }
        messageText.text = message;
        ShowMessage();
    }
}
