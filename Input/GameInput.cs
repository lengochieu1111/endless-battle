using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pattern.Singleton;

public class GameInput : Singleton<GameInput>
{
    public event EventHandler<bool> OnRunAction;
    public event EventHandler<bool> OnAttackAction;

    private PlayerInputActions _playerInputActions;

    protected override void Awake()
    {
        base.Awake();

        this._playerInputActions = new PlayerInputActions();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        this._playerInputActions.Player.Enable();

        this._playerInputActions.Player.Run.performed += Run_performed;
        this._playerInputActions.Player.Run.canceled += Run_canceled;

        this._playerInputActions.Player.Attack.performed += Attack_performed;
        this._playerInputActions.Player.Attack.canceled += Attack_canceled; ;
    }

    /*
     * 
     */

    private void Run_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnRunAction?.Invoke(this, true);
    }

    private void Run_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnRunAction?.Invoke(this, false);
    }

    private void Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnAttackAction?.Invoke(this, true);
    }

    private void Attack_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnAttackAction?.Invoke(this, false);
    }

    /*
     * 
     */

    public Vector2 GetMovementInputNormalize()
    {
        Vector2 moveInput = this._playerInputActions.Player.Movement.ReadValue<Vector2>();
        moveInput.Normalize();

        return moveInput;
    }


}
