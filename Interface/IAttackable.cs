using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IAttackable
{
    public abstract void StartTrace();
    public abstract void Tracing();
    public abstract void EndTrace();
    public abstract void CauseDamage(IDamageable damageable, Vector3 attackDirection, float damage);

}
