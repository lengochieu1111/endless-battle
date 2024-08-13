using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : RyoMonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private PlayerAnimator _playerAnimator;

    [SerializeField] private CapsuleCollider _capsuleCollider;
    [SerializeField] private Rigidbody _rigidbody;

    [SerializeField] private bool _isWalking;
    [SerializeField] private bool _isRunning;
    [SerializeField] private bool _isAttacking;

    protected override void LoadComponents()
    {
        base.LoadComponents();

        if (this._playerStats == null)
        {
            string path = "PlayerStats/PlayerStats";
            this._playerStats = Resources.Load<PlayerStats>(path);
        }

        if (this._playerAnimator == null )
        {
            this._playerAnimator = GetComponentInChildren<PlayerAnimator>();
        }
        
        if (this._rigidbody == null )
        {
            this._rigidbody = GetComponent<Rigidbody>();

            this._rigidbody.useGravity = false;
            this._rigidbody.freezeRotation = true;
        }
        
        if (this._capsuleCollider == null )
        {
            this._capsuleCollider = GetComponent<CapsuleCollider>();
        }

    }

    protected override void Start()
    {
        base.Start();

        GameInput.Instance.OnRunAction += GameInput_OnRunAction;
        GameInput.Instance.OnAttackAction += GameInput_OnAttackAction; ;

    }

    private void Update()
    {
        this.HandleMovement();

    }

    #region Action

    private void GameInput_OnRunAction(object sender, bool e)
    {
        this._isRunning = e;
    }

    private void GameInput_OnAttackAction(object sender, bool e)
    {
        this._isAttacking = e;
    }

    #endregion

    #region Movement

    private void HandleMovement()
    {
        Vector2 moveInput = GameInput.Instance.GetMovementInputNormalize();
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);

        float moveSpeed = this._isRunning ? this._playerStats.RunSpeed : this._playerStats.WalkSpeed;

        Vector3 point1 = this._capsuleCollider.bounds.center + Vector3.down * this._capsuleCollider.height / 2;
        Vector3 point2 = this._capsuleCollider.bounds.center + Vector3.up * this._capsuleCollider.height / 2;
        float radius = this._capsuleCollider.radius;
        float moveDistance = 0.1f;

        bool canMove = !Physics.CapsuleCast(point1, point2, radius, moveDir, moveDistance);

        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = moveDirX.x != 0 && !Physics.CapsuleCast(point1, point2, radius, moveDirX, moveDistance);
            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = moveDirZ.z != 0 && !Physics.CapsuleCast(point1, point2, radius, moveDirZ, moveDistance);
                if (canMove)
                {
                    moveDir = moveDirZ;
                }
            }
        }

        this._isWalking = moveDir != Vector3.zero;

        // Move handling
        if (canMove)
        {
            this._rigidbody.velocity = moveDir * moveSpeed * Time.deltaTime;
        }

        // Rotate handling
        this.transform.forward = Vector3.Slerp(this.transform.forward, moveDir, this._playerStats.RotationSpeed * Time.deltaTime);
    }

    #endregion

    #region Setter Gettter

    public bool GetIsWalking()
    {
        return this._isWalking;
    }
    
    public bool GetIsRunning()
    {
        return this._isRunning;
    }
    
    public bool GetIsAttacking()
    {
        return this._isAttacking;
    }

    #endregion

}
