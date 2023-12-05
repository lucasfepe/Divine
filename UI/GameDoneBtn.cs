using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDoneBtn : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>()
            .onClick.AddListener(() => SceneLoader.Load(SceneLoader.Scene.MainMenuScene));
    }
}
