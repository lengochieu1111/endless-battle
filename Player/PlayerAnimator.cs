using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : RyoMonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Player _player;

    protected override void LoadComponents()
    {
        base.LoadComponents();

        if (this._animator == null)
        {
            this._animator = GetComponent<Animator>();
        }
        
        if (this._player == null)
        {
            this._player = GetComponentInParent<Player>();
        }

    }

    private void Update()
    {
        this.UpdateWalkAnimation();
        this.UpdateRunAnimation();
        this.UpdateAttackAnimation();
        
    }

    private void UpdateWalkAnimation()
    {
        this._animator.SetBool(AnimationString.WALK_ANIM, this._player.GetIsWalking());
    }
    
    private void UpdateRunAnimation()
    {
        this._animator.SetBool(AnimationString.RUN_ANIM, this._player.GetIsRunning());
    }
    
    private void UpdateAttackAnimation()
    {
        this._animator.SetBool(AnimationString.ATTACK_ANIM, this._player.GetIsAttacking());
    }

    /*
     * 
     */



}
