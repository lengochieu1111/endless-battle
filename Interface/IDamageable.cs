using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public abstract bool TakeDamage(IAttackable attackable, Vector3 attackDirection, float damage);
    public abstract void StartPain(Vector3 attackDirection);
    public abstract void EndPain();
    public abstract void StartDeath();
    public abstract void EndDeath();

}
