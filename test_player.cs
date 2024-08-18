using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_player : RyoMonoBehaviour, IAttackable, IDamageable
{
    public event EventHandler<TakeDamageEventArgs> OnTakeDamage;

    public class TakeDamageEventArgs : EventArgs
    {
        public float Health;
        public float MaxHealth;
        public bool IsForwardAttackDirection;
        public bool IsDead;
    }

    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private testPlayerAnimator _playerAnimator;

    [SerializeField] private CapsuleCollider _capsuleCollider;
    [SerializeField] private Rigidbody _rigidbody;

    [Header("Check Is On Ground")]
    [SerializeField] private bool _isOnGround;

    [Header("Movement")]
    [SerializeField] private bool _isWalking;
    [SerializeField] private bool _isRunning;

    [Header("Attack")]
    [SerializeField] private bool _isRequestingAttack;
    [SerializeField] private bool _isAttacking;
    [SerializeField] private bool _isTracing;
    [SerializeField] private EAttackType _attackType;
    [SerializeField] private Transform _startTrace;
    [SerializeField] private Transform _endTrace;
    private List<Collider> _collidersDidDamage = new List<Collider>();

    [Header("Health")]
    [SerializeField] private bool _isPainning;
    [SerializeField] private bool _isDead;
    [SerializeField] private float _heath;


    protected override void LoadComponents()
    {
        base.LoadComponents();

        if (this._playerStats == null)
        {
            string path = "PlayerStats/PlayerStats";
            this._playerStats = Resources.Load<PlayerStats>(path);
        }

        if (this._playerAnimator == null)
        {
            this._playerAnimator = GetComponentInChildren<testPlayerAnimator>();
        }

        if (this._rigidbody == null)
        {
            this._rigidbody = GetComponent<Rigidbody>();

            this._rigidbody.useGravity = false;
            this._rigidbody.freezeRotation = true;
        }

        if (this._capsuleCollider == null)
        {
            this._capsuleCollider = GetComponent<CapsuleCollider>();
        }

    }

    protected override void SetupValues()
    {
        base.SetupValues();

        this._heath = this._playerStats.MaxHealth;
    }

    //protected override void Start()
    //{
    //    base.Start();

    //    GameInput.Instance.OnRunAction += GameInput_OnRunAction;
    //    GameInput.Instance.OnAttackAction += GameInput_OnAttackAction;

    //    this._playerAnimator.OnPlayerAnimationChangeTrace += PlayerAnimator_OnPlayerAnimationChangeTrace;

    //}



    private void Update()
    {
        this.GravityDecreasing();

        // this.HandleMovement();
    }

    private void FixedUpdate()
    {
        this.CheckIsOnGround();
    }

    #region Action

    private void GameInput_OnRunAction(object sender, bool e)
    {
        this._isRunning = e;
    }

    private void GameInput_OnAttackAction(object sender, bool e)
    {
        this._isRequestingAttack = e;

        if (e)
        {
            this.RequestAttack();
        }
    }

    private void PlayerAnimator_OnPlayerAnimationChangeTrace(object sender, bool e)
    {
        if (e)
        {
            this.StartTrace();
        }
        else
        {
            this.EndTrace();
        }
    }

    #endregion

    #region Check Is On Ground

    private void CheckIsOnGround()
    {
        float checkDistance = 0.05f;
        this._isOnGround = Physics.Raycast(this.transform.position, Vector3.down, checkDistance, this._playerStats.FloorLayer);
    }

    private void GravityDecreasing()
    {
        if (!this._isOnGround)
        {
            this._rigidbody.AddForce(Physics.gravity * (this._playerStats.Gravity - this._playerStats.ReduceGravity) * this._rigidbody.mass);
        }
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

        // No movement when idle attack
        if (this._isAttacking && this._attackType == EAttackType.Idle)
        {
            canMove = false;
            moveDir = Vector3.zero;
        }

        this._isWalking = moveDir != Vector3.zero;

        // Move handling
        if (canMove)
        {
            this._rigidbody.velocity = moveDir * moveSpeed * Time.deltaTime;
        }

        // Rotate handling
        if (!this._isAttacking)
        {
            this.transform.forward = Vector3.Slerp(this.transform.forward, moveDir, this._playerStats.RotationSpeed * Time.deltaTime);
        }
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

    public bool GetIsRequestingAttack()
    {
        return this._isRequestingAttack;
    }

    #endregion

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.blue;
    //    float radius = 0.12f;
    //    // Gizmos.DrawSphere(this._startSword.position, radius);
    //    // Gizmos.DrawSphere(this._endSword.position, radius);
    //    DrawCylinderBetweenPoints(this._startSword.position, this._endSword.position, radius);
    //}

    //private void DrawCylinderBetweenPoints(Vector3 start, Vector3 end, float radius)
    //{
    //    float cylinderLength = Vector3.Distance(start, end) * 2f;

    //    Vector3 direction = (end - start).normalized;
    //    Quaternion rotation = Quaternion.LookRotation(direction);

    //    Matrix4x4 originalMatrix = Gizmos.matrix;

    //    // Điều chỉnh ma trận để vẽ hình trụ theo chiều dọc (trục Y) thay vì trục Z
    //    Gizmos.matrix = Matrix4x4.TRS((start + end) / 2, rotation * Quaternion.Euler(90, 0, 0), new Vector3(radius * 2, cylinderLength / 2, radius * 2));

    //    Gizmos.DrawCube(Vector3.zero, new Vector3(1, 1, 1));

    //    Gizmos.matrix = originalMatrix;
    //}

    #region IAttackable

    public void RequestAttack()
    {
        if (this._isWalking)
        {
            this._attackType = EAttackType.Run;
        }
        else
        {
            this._attackType = EAttackType.Idle;
        }
    }

    public void StartAttack()
    {
        this._isAttacking = true;
    }

    public void StartTrace()
    {
        this._isTracing = true;
        this._collidersDidDamage.Clear();
    }

    public void Tracing()
    {
        if (this._isTracing)
        {
            Collider[] hitColliders = Physics.OverlapCapsule(this._startTrace.position, this._endTrace.position, this._playerStats.SwordColliderRadius, this._playerStats.PlayerLayer);

            foreach (Collider collider in hitColliders)
            {
                if (this._collidersDidDamage.Contains(collider) || collider == this._capsuleCollider)
                {
                    continue;
                }
                else
                {
                    if (collider.TryGetComponent<IDamageable>(out IDamageable damageable))
                    {
                        Vector3 attackDirection = (collider.transform.position - this.transform.position).normalized;
                        this.CauseDamage(damageable, attackDirection, this._playerStats.Damage);
                        this._collidersDidDamage.Add(collider);
                    }
                }
            }
        }
    }

    public void EndTrace()
    {
        this._isTracing = false;
    }

    public void EndAttack()
    {
        this._isAttacking = false;
    }

    public void CauseDamage(IDamageable damageable, Vector3 attackDirection, float damage)
    {
        damageable.TakeDamage(this, attackDirection, damage);
    }

    #endregion

    #region IDamageable

    public void TakeDamage(IAttackable attackable, Vector3 attackDirection, float damage)
    {
        this._heath = Math.Clamp(this._heath - damage, 0, this._playerStats.MaxHealth);

        this.HandlePain(attackDirection);

        if (this._heath <= 0)
        {
            this.HandleDeath();
        }

        bool isForwardAttackDirection = Vector3.Dot(attackDirection, this.transform.forward) >= 0;

        OnTakeDamage?.Invoke(this, new TakeDamageEventArgs
        {
            Health = this._heath,
            MaxHealth = this._playerStats.MaxHealth,
            IsForwardAttackDirection = isForwardAttackDirection,
            IsDead = this._isDead
        });

    }

    public void HandlePain(Vector3 attackDirection)
    {
        this._isPainning = true;

        attackDirection.y += 1;
        this._rigidbody.AddForce(attackDirection * this._playerStats.AttackForce);
    }

    public void HandleDeath()
    {
        this._isDead = true;
    }



    #endregion


}
