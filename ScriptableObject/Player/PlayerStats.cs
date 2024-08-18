using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="PLayer Stats", menuName = "Scriptable Object/PLayer Stats")]

public class PlayerStats : ScriptableObject
{
    [Header("Movement")]
    public float ReduceGravity = 0.2f;
    public float Gravity = 40f;
    public LayerMask FloorLayer;

    [Header("Movement")]
    public float WalkSpeed = 100f;
    public float RunSpeed = 200f;
    public float RotationSpeed = 15f;

    [Header("Attack")]
    public LayerMask PlayerLayer;
    public float SwordColliderRadius = 0.12f;
    public float Damage = 50;
    public float AttackForce = 400;

    [Header("Health")]
    public float MaxHealth = 100;

}
