using Pattern.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using Architecture.MVC;
using UnityEngine;
using Unity.VisualScripting;

[RequireComponent(typeof(CharacterController))]

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

    [Header("Game Input")]
    [SerializeField] private GameInput _gameInput;

    [Header("Components")]
    [SerializeField] protected CharacterController _characterController;

    [Header("Gravity")]
    protected float _velocityY;

    [Header("Movement")]
    [SerializeField] protected bool _isWalking;
    [SerializeField] protected bool _isRunning;
    protected Vector3 _movementDirection;
    protected float _movementSpeed;
    private Vector3 _previousMovementDirection;
    private bool _hasNotified;

    [Header("Rotation")]
    private float _currentVelocity;

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

    protected override void LoadComponents()
    {
        base.LoadComponents();

        if (this._characterController == null)
        {
            this._characterController = GetComponent<CharacterController>();
        }
    }

    protected override void SetupValues()
    {
        base.SetupValues();

        this._heath = this.model.PlayerStats.MaxHealth;
    }

    protected override void Start()
    {
        base.Start();

        if (this._gameInput != null)
        {
            this._gameInput.OnRunAction += GameInput_OnRunAction;
            this._gameInput.OnAttackAction += GameInput_OnAttackAction;
        }

        this.view.OnPlayerAnimationChangeTrace += PlayerAnimator_OnPlayerAnimationChangeTrace;
        this.view.OnPlayerAnimationNextAttack += PlayerAnimator_OnPlayerAnimationNextAttack;
        this.view.OnPlayerAnimationEndAttack += PlayerAnimator_OnPlayerAnimationEndAttack;

        this.view.OnPlayerAnimationEndPain += PlayerAnimator_OnPlayerAnimationEndPain;
        this.view.OnPlayerAnimationEndDeath += PlayerAnimator_OnPlayerAnimationEndDeath;

        this.view.OnPlayerAnimationPlayFoodstepLeftSound += PlayerAnimator_OnPlayerAnimationPlayFoodstepLeftSound;
        this.view.OnPlayerAnimationPlayFoodstepRightSound += PlayerAnimator_OnPlayerAnimationPlayFoodstepRightSound; ;

    }

    private void Update()
    {
        this.Update_MovementDirection();

        this.HandleGravity();
        this.HandleRotation();
        this.HandleMovement();

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

    private void PlayerAnimator_OnPlayerAnimationEndPain(object sender, EventArgs e)
    {
        this.EndPain();
    }

    private void PlayerAnimator_OnPlayerAnimationEndDeath(object sender, EventArgs e)
    {
        this.EndDeath();
    }

    private void PlayerAnimator_OnPlayerAnimationPlayFoodstepLeftSound(object sender, EventArgs e)
    {
        this.PlayFootstepLeftSound();
    }

    private void PlayerAnimator_OnPlayerAnimationPlayFoodstepRightSound(object sender, EventArgs e)
    {
        this.PlayFootstepRightSound();
    }

    #endregion

    #region Gravity

    private void HandleGravity()
    {
        if (this._characterController.isGrounded && this._velocityY < 0f)
        {
            this._velocityY = 0f;
        }
        else
        {
            this._velocityY -= this.model.PlayerStats.Gravity * this.model.PlayerStats.GravityMultiplier * Time.deltaTime;
        }

        this._movementDirection.y = _velocityY;
    }

    #endregion

    #region Movement

    private void Update_MovementDirection()
    {
        if (this._gameInput == null) return;

        if (this._isAttacking)
        {
            // No movement when idle attack
            this._movementDirection = Vector3.zero;
        }
        else
        {
            Vector2 moveInput = this._gameInput.GetMovementInputNormalize();
            this._movementDirection = new Vector3(moveInput.x, 0, moveInput.y);
        }
    }

    private void HandleMovement()
    {
        //
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

        // 
        this._movementSpeed = this._isRunning ? this.model.PlayerStats.RunSpeed : this.model.PlayerStats.WalkSpeed;

        // 
        Vector3 point1 = this._characterController.center + Vector3.down * this._characterController.height / 2;
        Vector3 point2 = this._characterController.center + Vector3.up * this._characterController.height / 2;
        float radius = this._characterController.radius;
        float moveDistance = 0.1f;

        bool canMove = !Physics.CapsuleCast(point1, point2, radius, this._movementDirection, moveDistance);

        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(this._movementDirection.x, 0, 0).normalized;
            canMove = moveDirX.x != 0 && !Physics.CapsuleCast(point1, point2, radius, moveDirX, moveDistance);
            if (canMove)
            {
                this._movementDirection = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, this._movementDirection.z).normalized;
                canMove = moveDirZ.z != 0 && !Physics.CapsuleCast(point1, point2, radius, moveDirZ, moveDistance);
                if (canMove)
                {
                    this._movementDirection = moveDirZ;
                }
            }
        }

        this._isWalking = this._movementDirection.x != 0 || this._movementDirection.z != 0;

        this._characterController.Move(this._movementDirection * this._movementSpeed * Time.deltaTime);

    }

    #endregion

    #region Rotation

    private void HandleRotation()
    {
        bool canRotate = !this._isAttacking && (this._movementDirection.x != 0 || this._movementDirection.z != 0);
        if (canRotate)
        {
            float targetAngle = Mathf.Atan2(this._movementDirection.x, this._movementDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(this.transform.eulerAngles.y, targetAngle, ref _currentVelocity, this.model.PlayerStats.SmoothTime);
            this.transform.rotation = Quaternion.Euler(0, angle, 0);
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
                DrawCylinderBetweenPoints(traceTracker.StartTrace.position, traceTracker.EndTrace.position, this.model.PlayerStats.WeaponColliderRadius);
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

    public virtual void Attack()
    {
        OnAttack?.Invoke(this, this.model.PlayerStats.AttackStateNameArray[this._attackIndex]);
        this._attackIndex = (this._attackIndex + 1) % this.model.PlayerStats.AttackStateNameArray.Length;
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

        this.PlayWeaponAttackSound();

    }

    public virtual void Tracing()
    {
        if (this._isTracing)
        {
            foreach (TraceTracker traceTracker in this._traceTracker)
            {
                Collider[] hitColliders = Physics.OverlapCapsule(traceTracker.StartTrace.position, 
                    traceTracker.EndTrace.position, this.model.PlayerStats.WeaponColliderRadius, this.model.PlayerStats.PlayerLayer);

                foreach (Collider collider in hitColliders)
                {
                    if (this._collidersDidDamage.Contains(collider) || collider == this._characterController)
                    {
                        continue;
                    }
                    else
                    {
                        if (collider.TryGetComponent<IDamageable>(out IDamageable damageable))
                        {
                            Vector3 attackDirection = (collider.transform.position - this.transform.position).normalized;
                            this.CauseDamage(damageable, attackDirection, this.model.PlayerStats.Damage);
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
    }

    public virtual void CauseDamage(IDamageable damageable, Vector3 attackDirection, float damage)
    {
        damageable.TakeDamage(this, attackDirection, damage);

        this.PlayWeaponHitSound();
    }

    #endregion

    #region IDamageable

    public bool TakeDamage(IAttackable attackable, Vector3 attackDirection, float damage)
    {
        if (this._isDead) return false;

        this._heath = Math.Clamp(this._heath - damage, 0, this.model.PlayerStats.MaxHealth);

        this.StartPain(attackDirection);

        if (this._heath <= 0)
        {
            this.StartDeath();
        }

        // 

        string damageStateName;

        bool isForwardAttackDirection = Vector3.Dot(attackDirection, this.transform.forward) >= 0;

        if (isForwardAttackDirection)
        {
            if (this._isDead)
            {
                damageStateName = this.model.PlayerStats.DeathBackwardStateName;
            }
            else
            {
                damageStateName = this.model.PlayerStats.HitBackwardStateName;
            }
        }
        else
        {
            if (this._isDead)
            {
                damageStateName = this.model.PlayerStats.DeathForwardStateName;
            }
            else
            {
                damageStateName = this.model.PlayerStats.HitForwardStateName;
            }
        }

        OnTakeDamage?.Invoke(this, new TakeDamageEventArgs
        {
            Health = this._heath,
            MaxHealth = this.model.PlayerStats.MaxHealth,
            DamageStateName = damageStateName
        });

        return true;
    }

    public void StartPain(Vector3 attackDirection)
    {
        this._isPainning = true;
        this.PlayPlayerHitSound();
    }

    public void EndPain()
    {
        this._isPainning = false;
    }

    public void StartDeath()
    {
        this._isDead = true;
        this.PlayPlayerDeathSound();
    }

    public void EndDeath()
    {
        this._isDead = false;

        Destroy(this.gameObject);
    }

    #endregion


    #region Sound

    protected virtual void PlayFootstepLeftSound()
    {
        SoundFXManager.PlaySoundFXClip(this.model.PlayerStats.FootstepLeftSound, this.transform, 1f);
    }

    protected virtual void PlayFootstepRightSound()
    {
        SoundFXManager.PlaySoundFXClip(this.model.PlayerStats.FootstepRightSound, this.transform, 1f);
    }

    protected virtual void PlayWeaponAttackSound()
    {
        SoundFXManager.PlaySoundFXClip(this.model.PlayerStats.WeaponAttackSound[this._attackIndex], this.transform, 1f);
    }

    protected virtual void PlayWeaponHitSound()
    {
        SoundFXManager.PlaySoundFXClip(this.model.PlayerStats.WeaponHitSound[this._attackIndex], this.transform, 1f);
    }
    
    protected virtual void PlayPlayerHitSound()
    {
        SoundFXManager.PlaySoundFXClip(this.model.PlayerStats.PlayerHitSound, this.transform, 1f);
    }

    protected virtual void PlayPlayerDeathSound()
    {
        SoundFXManager.PlaySoundFXClip(this.model.PlayerStats.PlayerDeathSound, this.transform, 1f);
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
