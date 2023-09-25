using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Image inspectCardUIBackground;

    private void Start()
    {
        inspectCardUIBackground.gameObject.SetActive(true);
    }

}
