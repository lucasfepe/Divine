using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingTextUI : MonoBehaviour
{
    private void Start()
    {
        Show();
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public bool IsActive()
    {
        return gameObject.activeInHierarchy;
    }
}
