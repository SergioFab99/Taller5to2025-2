using UnityEngine;

public enum AttackStyle
{
    Melee,
    Ranged,
    Thrower   
}

[CreateAssetMenu(fileName = "EnemyAISettings", menuName = "Scriptable Objects/EnemyAISettings")]
public class EnemyAiSettings : ScriptableObject
{
    public float stopingDistance;
    public float detectionDistance;
    public float attackRange;
    public float stunDuration;

    [Header("Behaviour")]
    public AttackStyle attackStyle = AttackStyle.Melee;
}