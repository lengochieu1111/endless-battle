using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="PLayer Stats", menuName = "Scriptable Object/PLayer Stats")]

public class PlayerStats : ScriptableObject
{
    public float WalkSpeed = 100f;
    public float RunSpeed = 200f;
    public float RotationSpeed = 15f;


}
