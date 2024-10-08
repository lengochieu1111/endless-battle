using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AE_Attack : StateMachineBehaviour
{
    private IAttackable _attackable;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this._attackable = animator.GetComponentInParent<IAttackable>();
        // this._attackable?.StartAttack();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this._attackable?.Tracing();
    }

    //public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    base.OnStateExit(animator, stateInfo, layerIndex);

    //    this._attackable?.EndAttack();
    //}

}
