using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class SkillDescScriptUI : MonoBehaviour
{
    [SerializeField] private Animator skillDescAnimator;
    private bool isShowing = false;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (isShowing)
            {
                skillDescAnimator.ResetTrigger("OnShow");
                skillDescAnimator.SetTrigger("OnHide");
                isShowing = false;
            }
            else
            {
                skillDescAnimator.ResetTrigger("OnHide");
                skillDescAnimator.SetTrigger("OnShow");
                isShowing = true;
            }
        });
    }
    public void Hide()
    {
        isShowing = false;
        skillDescAnimator.gameObject.SetActive(false);
    }
    public void Show()
    {
        skillDescAnimator.gameObject.SetActive(true);
    }
    //private void OnMouseEnter()
    //{

    //    skillDescAnimator.ResetTrigger("OnHide");
    //    skillDescAnimator.SetTrigger("OnShow");
    //}

    //private void OnMouseExit()
    //{

    //    skillDescAnimator.ResetTrigger("OnShow");
    //    skillDescAnimator.SetTrigger("OnHide");
    //}
}
