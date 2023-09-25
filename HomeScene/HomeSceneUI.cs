using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeSceneUI : MonoBehaviour
{
    [SerializeField] private Button enterGameButton;
    [SerializeField] private Animator loginPanelAnimator;

    private void Awake()
    {
        enterGameButton.onClick.AddListener(() =>
        {
            loginPanelAnimator.SetTrigger("LoginAppearTrigger");
            //SceneLoader.Load(SceneLoader.Scene.LobbyScene);

        });
       
    }
}
