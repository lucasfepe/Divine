using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipTutorial : MonoBehaviour
{
    [SerializeField] private Button skipTutorialButton;

    private void Awake()
    {
        skipTutorialButton.onClick.AddListener(() => SceneLoader.Load(SceneLoader.Scene.MainMenuScene));
    }

}
