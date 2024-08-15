using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public abstract void TakeDamage(IAttackable attackable, Vector3 attackDirection, float damage);
    public abstract void HandlePain(Vector3 attackDirection);
    public abstract void HandleDeath();

}
