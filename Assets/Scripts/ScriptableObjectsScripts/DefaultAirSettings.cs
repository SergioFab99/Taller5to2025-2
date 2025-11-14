using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "DefaultAirSettings", menuName = "Scriptable Objects/CharacterMovementSettings/DefaultAirSettings")]
public class DefaultAirSettings : ScriptableObject
{
    public float AirSpeed,
                 AirAcceleration, 
                 JumpSpeed, 
                 Gravity,
                 CoyoteTime;
}
