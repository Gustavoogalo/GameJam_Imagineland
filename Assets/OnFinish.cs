using UnityEngine;

public class OnFinish : StateMachineBehaviour
{
    [SerializeField] private string animation;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Player_Moviment>().ChangeAnimation(animation, 0.1f,stateInfo.length);
    }
}
