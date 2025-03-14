using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleAuraAnimator : MonoBehaviour
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
