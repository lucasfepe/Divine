using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeSceneUI : MonoBehaviour
{
    [SerializeField] private Button enterGameButton;
    [SerializeField] private Animator loginPanelAnimator;

    private void Start()
    {
        enterGameButton.onClick.AddListener(() =>
        {
            loginPanelAnimator.SetTrigger("LoginAppearTrigger");
            UIInputManager.Instance.SelectFirstField();
            Destroy(enterGameButton.gameObject);
            //SceneLoader.Load(SceneLoader.Scene.LobbyScene);

        });
       
    }
}
