using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_PLAYERANIMATOR : RyoMonoBehaviour
{
    public readonly float MIN_PARAMETER = 0f;
    public readonly float MIDDLE_PARAMETER = 1f;
    public readonly float MAX_PARAMETER = 2f;

    public event EventHandler<bool> OnPlayerAnimationChangeTrace;
    public event EventHandler OnPlayerAnimationNextAttack;
    public event EventHandler OnPlayerAnimationEndAttack;

    [SerializeField] private Animator _animator;
    [SerializeField] private TEST_PLAYER _player;
    [SerializeField] private float _stateChangeTime = 0.1f;
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
            this._player = GetComponentInParent<TEST_PLAYER>();
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
        this.PlayAnimation(e, this._stateChangeTime);
    }

    private void Player_OnTakeDamage(object sender, TEST_PLAYER.TakeDamageEventArgs e)
    {
        this.PlayAnimation(e.DamageStateName, this._stateChangeTime);
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
        if (curernt_value != target_value && this._changeTimer <= this._stateChangeTime * 2)
        {
            this._changeTimer += Time.deltaTime;
            new_value = Mathf.Lerp(curernt_value, target_value, this._changeTimer / this._stateChangeTime * 2);
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
    }

}

