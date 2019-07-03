using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animator_ResetInteger : StateMachineBehaviour
{
    public string reset;
    public int with = 0;

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger(reset,with);
    }

}
