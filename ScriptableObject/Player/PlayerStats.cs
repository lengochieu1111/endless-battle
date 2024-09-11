using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="PLayer Stats", menuName = "Scriptable Object/PLayer Stats")]

public class PlayerStats : ScriptableObject
{
    [Header("Gravity")]
    public float Gravity = 9.81f;
    public float GravityMultiplier = 3f;

    [Header("Movement")]
    public float WalkSpeed = 6f;
    public float RunSpeed = 10f;
    public float RotationSpeed = 5f;
    public float SmoothTime = 0.2f;

    [Header("Attack")]
    public LayerMask PlayerLayer;
    public string[] AttackStateNameArray;
    public float WeaponColliderRadius = 0.12f;
    public float WeaponSpeed = 20f;
    public float Damage = 50;
    public float AttackForce = 400;

    [Header("Health")]
    public float MaxHealth = 100;
    public string HitForwardStateName;
    public string HitBackwardStateName;
    public string DeathForwardStateName;
    public string DeathBackwardStateName;

    [Header("Sound")]
    public AudioClip FootstepLeftSound;
    public AudioClip FootstepRightSound;
    public AudioClip[] WeaponAttackSound;
    public AudioClip[] WeaponHitSound;
    public AudioClip PlayerHitSound;
    public AudioClip PlayerDeathSound;




}
