using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "DefaultBlockDodgeCounterSettings", menuName = "Scriptable Objects/CharacterCombatSettings/DefaultBlockDodgeCounterSettings")]
public class DefaultBlockDodgeCounterSettings : ScriptableObject
{
    [Header("Dodge Settings")]
    public float dodgeDistance = 3f;
    public float dodgeDuration = 0.2f;    
    private float dodgeTimer = 0f;

    [Header("Block Settings")]
    public float blockAngle = 120f;
    public float blockDamageMultiplier = 0.5f;

    public float Speed = 0f;

    public float Response = 0f;


    [Header("Counter Settings")]
    public float counterWindow = 0.5f;    
    private float counterTimer = 0f;
}
