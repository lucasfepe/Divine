using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LessSkillInfoButtonUI : MonoBehaviour
{

    [SerializeField] private MoreSkillInfoUI moreSkillInfoUI;
    [SerializeField] private SkillButton retractedSkillUI;
    private bool toggleMoreInfo = false;
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            moreSkillInfoUI.Hide();
            retractedSkillUI.Show();
        });
    }

    
}
