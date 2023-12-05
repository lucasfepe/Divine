using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StorySceneUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> sets;
    [SerializeField] private Button nextButton;

    private int activeSet = -1;


    private void Awake()
    {

        nextButton.onClick.AddListener(()=>ShowNextSet());
        ShowNextSet();
    }

    private void ShowNextSet()
    {
        if(activeSet == sets.Count - 1)
        {
            SceneLoader.Load(SceneLoader.Scene.TutorialScene);
            return;
        }
        sets.ElementAt(++activeSet).SetActive(true);
    }
}
