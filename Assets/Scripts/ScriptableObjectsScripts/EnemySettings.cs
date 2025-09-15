using UnityEngine;

[CreateAssetMenu(fileName = "EnemySettings", menuName = "Scriptable Objects/EnemySettings")]
public class EnemySettings : ScriptableObject
{
    public float walkSpeed,
                 runSpeed;
    public float AirSpeed,
                 AirAcceleration,
                 JumpSpeed,
                 Gravity;

public float stopingDistance;
}
