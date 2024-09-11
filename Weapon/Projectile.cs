using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : RyoMonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private SphereCollider _sphereCollider;
    private IAttackable _attackableParent;
    private IDamageable _damageableParent;
    private float _damage;

    protected override void LoadComponents()
    {
        base.LoadComponents();

        if (this._rigidbody == null)
        {
            this._rigidbody = GetComponent<Rigidbody>();
            this._rigidbody.useGravity = false;
        }
        
        if (this._sphereCollider == null)
        {
            this._sphereCollider = GetComponent<SphereCollider>();
            this._sphereCollider.isTrigger = true;
        }
    }

    public void ActivateSelf(IAttackable attackableParent, IDamageable damageableParent, Vector3 direction, float speed, float damage)
    {
        this._attackableParent = attackableParent;
        this._damageableParent = damageableParent;
        this._damage = damage;

        this.transform.forward = direction;
        this._rigidbody.velocity = this.transform.forward * speed;
    }

    public void DestroySelf()
    {
        WeaponSpawner.Instance.DestroyObjectPooling(this.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning(other.transform.name);

        if (other.TryGetComponent(out IDamageable damageable))
        {
            if (damageable != this._damageableParent)
            {
                this._attackableParent.CauseDamage(damageable, this.transform.forward, this._damage);
            }
        }

        this.DestroySelf();
    }

}
