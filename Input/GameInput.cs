using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pattern.Singleton;

public class GameInput : RyoMonoBehaviour
{
    public event EventHandler<bool> OnRunAction;
    public event EventHandler OnAttackAction;

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

        this._playerInputActions.Player.Attack.started += Attack_started; ; 
    }


    /*
     * 
     */

    private void Run_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnRunAction?.Invoke(this, true);

        Debug.Log("Move");
    }

    private void Run_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnRunAction?.Invoke(this, false);
    }

    private void Attack_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnAttackAction?.Invoke(this, EventArgs.Empty);
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
