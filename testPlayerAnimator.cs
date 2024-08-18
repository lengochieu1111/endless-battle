using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testPlayerAnimator : RyoMonoBehaviour
{
    public readonly float MIN_PARAMETER = -1f;
    public readonly float MAX_PARAMETER = 1f;

    public event EventHandler<bool> OnPlayerAnimationChangeTrace;

    [SerializeField] private Animator _animator;
    [SerializeField] private test_player _player;

    protected override void LoadComponents()
    {
        base.LoadComponents();

        if (this._animator == null)
        {
            this._animator = GetComponent<Animator>();
        }

        if (this._player == null)
        {
            this._player = GetComponentInParent<test_player>();
        }

    }

    protected override void Start()
    {
        base.Start();

        this._player.OnTakeDamage += Player_OnTakeDamage;
    }

    private void Player_OnTakeDamage(object sender, test_player.TakeDamageEventArgs e)
    {
        if (e.IsDead)
        {
            this.DeathTriggerAnimation(e.IsForwardAttackDirection);
        }
        else
        {
            this.HitTriggerAnimation(e.IsForwardAttackDirection);
        }
    }

    private void Update()
    {
        this.Update_IsMoving();
        this.Update_MovementParameter();
        this.Update_IsRequestingAttack();

    }

    private void Update_IsMoving()
    {
        this._animator.SetBool(AnimationString.IS_MOVING_ANIM, this._player.GetIsWalking());
    }

    private void Update_MovementParameter()
    {
        if (this._player.GetIsRunning())
        {
            this._animator.SetFloat(AnimationString.MOVEMENT_PARAMETER_ANIM, MAX_PARAMETER);
        }
        else
        {
            this._animator.SetFloat(AnimationString.MOVEMENT_PARAMETER_ANIM, MIN_PARAMETER);
        }
    }

    private void Update_IsRequestingAttack()
    {
        this._animator.SetBool(AnimationString.IS_REQUESTING_ATTACK_ANIM, this._player.GetIsRequestingAttack());
    }

    public void HitTriggerAnimation(bool isForwardAttackDirection)
    {
        if (isForwardAttackDirection)
        {
            this._animator.SetTrigger(AnimationString.HIT_BACKWARD_TRIGGER_ANIM);
        }
        else
        {
            this._animator.SetTrigger(AnimationString.HIT_FORWARD_TRIGGER_ANIM);
        }
    }

    public void DeathTriggerAnimation(bool isForwardAttackDirection)
    {
        if (isForwardAttackDirection)
        {
            this._animator.SetTrigger(AnimationString.DEAD_BACKWARD_TRIGGER_ANIM);
        }
        else
        {
            this._animator.SetTrigger(AnimationString.DEAD_FORWARD_TRIGGER_ANIM);
        }
    }

    // Animation Event

    public void AE_StartTrace()
    {
        OnPlayerAnimationChangeTrace?.Invoke(this, true);
    }

    public void AE_EndTrace()
    {
        OnPlayerAnimationChangeTrace?.Invoke(this, false);
    }
}
