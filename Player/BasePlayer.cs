using Pattern.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using Architecture.MVC;
using UnityEngine;

[System.Serializable]
public class TraceTracker
{
    public Transform StartTrace;
    public Transform EndTrace;
}

public class BasePlayer : BaseController<PlayerModel, PlayerAnimator>, IAttackable, IDamageable
{
    public event EventHandler OnChangeDirection;
    public event EventHandler<string> OnAttack;
    public event EventHandler<TakeDamageEventArgs> OnTakeDamage;

    public class TakeDamageEventArgs : EventArgs
    {
        public float Health;
        public float MaxHealth;
        public string DamageStateName;
    }

    [SerializeField] protected PlayerModel _playerModel;
    [SerializeField] protected PlayerAnimator _playerAnimator;

    [SerializeField] protected CapsuleCollider _capsuleCollider;
    [SerializeField] protected Rigidbody _rigidbody;

    [Header("Check Is On Ground")]
    [SerializeField] protected bool _isOnGround;

    [Header("Movement")]
    [SerializeField] protected bool _isWalking;
    [SerializeField] protected bool _isRunning;
    [SerializeField] protected Vector3 _movementDirection;
    private Vector3 _previousMovementDirection;
    private bool _hasNotified;

    [Header("Attack")]
    [SerializeField] protected bool _isAttacking;
    [SerializeField] protected bool _savedAttack;
    [SerializeField] protected bool _isTracing;
    [SerializeField] protected int _attackIndex;
    [SerializeField] protected List<TraceTracker> _traceTracker;
    protected List<Collider> _collidersDidDamage = new List<Collider>();

    [Header("Health")]
    [SerializeField] protected bool _isPainning;
    [SerializeField] protected bool _isDead;
    [SerializeField] protected float _heath;

    [SerializeField] protected float radius = 0.12f;


    protected override void LoadComponents()
    {
        base.LoadComponents();

        if (this._playerModel == null)
        {
            this._playerModel = GetComponentInChildren<PlayerModel>();
        }

        if (this._playerAnimator == null)
        {
            this._playerAnimator = GetComponentInChildren<PlayerAnimator>();
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

        this._isOnGround = true;

        this._heath = this._playerModel.PlayerStats.MaxHealth;
    }

    protected override void Start()
    {
        base.Start();

        GameInput.Instance.OnRunAction += GameInput_OnRunAction;
        GameInput.Instance.OnAttackAction += GameInput_OnAttackAction; ;

        this._playerAnimator.OnPlayerAnimationChangeTrace += PlayerAnimator_OnPlayerAnimationChangeTrace;
        this._playerAnimator.OnPlayerAnimationNextAttack += PlayerAnimator_OnPlayerAnimationNextAttack;
        this._playerAnimator.OnPlayerAnimationEndAttack += PlayerAnimator_OnPlayerAnimationEndAttack; ;

    }


    private void Update()
    {
        this.GravityDecreasing();

        this.HandleMovement();

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

    private void GameInput_OnAttackAction(object sender, EventArgs e)
    {
        this.RequestAttack();
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

    private void PlayerAnimator_OnPlayerAnimationNextAttack(object sender, EventArgs e)
    {
        this.NextAttack();
    }

    private void PlayerAnimator_OnPlayerAnimationEndAttack(object sender, EventArgs e)
    {
        this.EndAttack();
    }

    #endregion

    #region Check Is On Ground

    private void CheckIsOnGround()
    {
        float bufferCheckDistance = 0.2f;

        RaycastHit hit;
        this._isOnGround = Physics.Raycast(this.transform.position, -this.transform.up, out hit, bufferCheckDistance, this._playerModel.PlayerStats.FloorLayer);
    }

    private void GravityDecreasing()
    {
        if (!this._isOnGround)
        {
            this._rigidbody.AddForce(Physics.gravity * (this._playerModel.PlayerStats.Gravity - this._playerModel.PlayerStats.ReduceGravity) * this._rigidbody.mass);
        }
    }

    #endregion

    #region Movement

    private void HandleMovement()
    {
        Vector2 moveInput = GameInput.Instance.GetMovementInputNormalize();
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);

        this._movementDirection = moveDir;

        if (!Mathf.Approximately(this._movementDirection.x, this._previousMovementDirection.x)
            || !Mathf.Approximately(this._movementDirection.z, this._previousMovementDirection.z))
        {
            if (!this._hasNotified)
            {
                this._hasNotified = true;
                this._previousMovementDirection = this._movementDirection;

                OnChangeDirection?.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            this._hasNotified = false;
        }

        float moveSpeed = this._isRunning ? this._playerModel.PlayerStats.RunSpeed : this._playerModel.PlayerStats.WalkSpeed;

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
        if (this._isAttacking)
        {
            canMove = false;
            // moveDir = Vector3.zero;
        }

        this._isWalking = moveDir != Vector3.zero;

        // Move handling
        if (canMove)
        {
            this._rigidbody.velocity = moveDir * moveSpeed * Time.deltaTime;
        }
        else
        {
            this._rigidbody.velocity = Vector3.zero;
        }

        // Rotate handling
        if (!this._isAttacking)
        {
            this.transform.forward = Vector3.Slerp(this.transform.forward, moveDir, this._playerModel.PlayerStats.RotationSpeed * Time.deltaTime);
        }
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        if (this._isTracing)
        {
            foreach (TraceTracker traceTracker in this._traceTracker)
            {
                DrawCylinderBetweenPoints(traceTracker.StartTrace.position, traceTracker.EndTrace.position, radius);
            }
        }

    }

    private void DrawCylinderBetweenPoints(Vector3 start, Vector3 end, float radius)
    {
        float cylinderLength = Vector3.Distance(start, end) * 2f;

        Vector3 direction = (end - start).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);

        Matrix4x4 originalMatrix = Gizmos.matrix;

        // Điều chỉnh ma trận để vẽ hình trụ theo chiều dọc (trục Y) thay vì trục Z
        Gizmos.matrix = Matrix4x4.TRS((start + end) / 2, rotation * Quaternion.Euler(90, 0, 0), new Vector3(radius * 2, cylinderLength / 2, radius * 2));

        Gizmos.DrawCube(Vector3.zero, new Vector3(1, 1, 1));

        Gizmos.matrix = originalMatrix;
    }

    #region IAttackable

    public virtual void Attack()
    {
        OnAttack?.Invoke(this, this._playerModel.PlayerStats.AttackStateNameArray[this._attackIndex]);
        this._attackIndex = (this._attackIndex + 1) % this._playerModel.PlayerStats.AttackStateNameArray.Length;
    }

    public virtual void RequestAttack()
    {
        if (this._isAttacking)
        {
            this._savedAttack = true;
        }
        else
        {
            this.StartAttack();

            this.Attack();
        }
    }

    public virtual void StartAttack()
    {
        this._savedAttack = false;
        this._isAttacking = true;
    }

    public void NextAttack()
    {
        if (this._savedAttack)
        {
            this.Attack();
            this._savedAttack = false;
        }
    }

    public virtual void StartTrace()
    {
        this._collidersDidDamage.Clear();
        this._isTracing = true;
    }

    public virtual void Tracing()
    {
        if (this._isTracing)
        {
            foreach (TraceTracker traceTracker in this._traceTracker)
            {
                Collider[] hitColliders = Physics.OverlapCapsule(traceTracker.StartTrace.position, traceTracker.EndTrace.position, this._playerModel.PlayerStats.WeaponColliderRadius, this._playerModel.PlayerStats.PlayerLayer);

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
                            this.CauseDamage(damageable, attackDirection, this._playerModel.PlayerStats.Damage);
                            this._collidersDidDamage.Add(collider);
                        }
                    }
                }
            }
        }
    }

    public virtual void EndTrace()
    {
        this._isTracing = false;
    }

    public virtual void EndAttack()
    {
        this._attackIndex = 0;
        this._savedAttack = false;
        this._isAttacking = false;
        Debug.Log("End Atack");
    }

    public virtual void CauseDamage(IDamageable damageable, Vector3 attackDirection, float damage)
    {
        damageable.TakeDamage(this, attackDirection, damage);
    }

    #endregion

    #region IDamageable

    public void TakeDamage(IAttackable attackable, Vector3 attackDirection, float damage)
    {
        this._heath = Math.Clamp(this._heath - damage, 0, this._playerModel.PlayerStats.MaxHealth);

        this.HandlePain(attackDirection);

        if (this._heath <= 0)
        {
            this.HandleDeath();
        }

        string damageStateName = this._playerModel.PlayerStats.HitForwardStateName;

        bool isForwardAttackDirection = Vector3.Dot(attackDirection, this.transform.forward) >= 0;
        if (isForwardAttackDirection)
        {
            damageStateName = this._playerModel.PlayerStats.HitBackwardStateName;
        }

        OnTakeDamage?.Invoke(this, new TakeDamageEventArgs
        {
            Health = this._heath,
            MaxHealth = this._playerModel.PlayerStats.MaxHealth,
            DamageStateName = damageStateName
        });

    }

    public void HandlePain(Vector3 attackDirection)
    {
        this._isPainning = true;

        attackDirection.y += 1;
        this._rigidbody.AddForce(attackDirection * this._playerModel.PlayerStats.AttackForce);
    }

    public void HandleDeath()
    {
        this._isDead = true;
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

    #endregion

}
