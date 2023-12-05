using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAnimationEndInvoke : StateMachineBehaviour
{

    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.parent.GetComponentInParent<ExpertCardUI>().CallOnEndAnimation();
    }


}
