using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : RyoMonoBehaviour, IDamageable
{
    [SerializeField] private PlayerStats _playerStats;

    [SerializeField] private CapsuleCollider _capsuleCollider;
    [SerializeField] private Rigidbody _rigidbody;

    [Header("Capsual")]
    [SerializeField] private bool _isOnGround;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float ReduceGravity = 0.2f;
    [SerializeField] private float gravityFall = 60f;


    protected override void LoadComponents()
    {
        base.LoadComponents();

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

    private void Update()
    {
        this.GravityDecreasing();

    }

    private void FixedUpdate()
    {
        this.CheckIsOnGround();
    }


    private void CheckIsOnGround()
    {
        float checkDistance = 0.05f;
        // Bắn một tia xuống dưới từ vị trí groundCheck
        this._isOnGround = Physics.Raycast(this.transform.position, Vector3.down, checkDistance);
    }

    private void GravityDecreasing()
    {
        if (!this._isOnGround)
        {
            this._rigidbody.AddForce(Physics.gravity * (this.gravityFall - this.ReduceGravity) * this._rigidbody.mass);
        }
    }

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


    #region IDamageable

    public void TakeDamage(IAttackable attackable, Vector3 attackDirection, float damage)
    {
        attackDirection.y += 1;
        this._rigidbody.AddForce(attackDirection * this._playerStats.AttackForce);
        Debug.Log("Damage");
    }

    public void HandlePain(Vector3 attackDirection)
    {
        throw new System.NotImplementedException();
    }

    public void HandleDeath()
    {
        throw new System.NotImplementedException();
    }

    #endregion
}
