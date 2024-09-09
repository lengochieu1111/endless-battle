using System;
using System.Collections;
using System.Collections.Generic;
using Architecture.MVC;
using UnityEngine;

public class PlayerAnimator : BaseView<BasePlayer>
{
    public readonly float MIN_PARAMETER = 0f;
    public readonly float MIDDLE_PARAMETER = 1f;
    public readonly float MAX_PARAMETER = 2f;

    public event EventHandler<bool> OnPlayerAnimationChangeTrace;
    public event EventHandler OnPlayerAnimationNextAttack;
    public event EventHandler OnPlayerAnimationEndAttack;
    public event EventHandler OnPlayerAnimationEndPain;
    public event EventHandler OnPlayerAnimationEndDeath;

    [SerializeField] private Animator _animator;
    [SerializeField] private BasePlayer _player;
    [SerializeField] private float _basicStateChangeTime = 0.5f;
    [SerializeField] private float _advancedStateChangeTime = 0.1f;
    private float _changeTimer;

    protected override void LoadComponents()
    {
        base.LoadComponents();

        if (this._animator == null)
        {
            this._animator = GetComponent<Animator>();
        }
        
        if (this._player == null)
        {
            this._player = GetComponentInParent<BasePlayer>();
        }

    }

    protected override void Start()
    {
        base.Start();

        this._player.OnChangeDirection += Player_OnChangeDirection; ;
        this._player.OnAttack += Player_OnAttack;
        this._player.OnTakeDamage += Player_OnTakeDamage;
    }



    private void Update()
    {
        this.Update_MovementParameter();

    }

    /*
     * 
     */

    private void Player_OnChangeDirection(object sender, EventArgs e)
    {
        this._changeTimer = 0;
    }

    private void Player_OnAttack(object sender, String e)
    {
        this.PlayAnimation(e, this._advancedStateChangeTime);
    }

    private void Player_OnTakeDamage(object sender, BasePlayer.TakeDamageEventArgs e)
    {
        this.PlayAnimation(e.DamageStateName, this._advancedStateChangeTime);
    }

    /*
     * 
     */

    private void Update_MovementParameter()
    {
        float target_value = MIN_PARAMETER;
        if (this._player.GetIsWalking())
        {
            target_value = MIDDLE_PARAMETER;
            if (this._player.GetIsRunning())
            {
                target_value = MAX_PARAMETER;
            }
        }

        float curernt_value = this._animator.GetFloat(AnimationString.MOVEMENT_PARAMETER_ANIM);

        float new_value = 0;
        if (curernt_value != target_value && this._changeTimer <= this._basicStateChangeTime)
        {
            this._changeTimer += Time.deltaTime;
            new_value = Mathf.Lerp(curernt_value, target_value, this._changeTimer / this._basicStateChangeTime);
        }
        else
        {
            new_value = target_value;
            this._changeTimer = 0;
        }

        this._animator.SetFloat(AnimationString.MOVEMENT_PARAMETER_ANIM, new_value);

    }

    /*
     * 
     */

    private void PlayAnimation(String stateName, float conversionTime)
    {
        this._animator.CrossFade(stateName, conversionTime);
    }

    /*
     * Animation Event
     */

    public void AE_StartTrace()
    {
        OnPlayerAnimationChangeTrace?.Invoke(this, true);
    }
    
    public void AE_EndTrace()
    {
        OnPlayerAnimationChangeTrace?.Invoke(this, false);
    }

    public void AE_NextAttack()
    {
        OnPlayerAnimationNextAttack?.Invoke(this, EventArgs.Empty);
    }
    
    public void AE_EndAttack()
    {
        OnPlayerAnimationEndAttack?.Invoke(this, EventArgs.Empty);
        Debug.Log("End Attack");
    }
    
    public void AE_EndPain()
    {
        OnPlayerAnimationEndPain?.Invoke(this, EventArgs.Empty);
    }
    
    public void AE_EndDeath()
    {
        OnPlayerAnimationEndDeath?.Invoke(this, EventArgs.Empty);
    }

}
