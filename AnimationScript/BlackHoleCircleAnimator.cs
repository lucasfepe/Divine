using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleCircleAnimator : MonoBehaviour
{

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Disappear()
    {
        animator.SetTrigger("Disappear");
    }

}
