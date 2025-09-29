using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAISettings", menuName = "Scriptable Objects/EnemyAISettings")]
public class EnemyAiSettings : ScriptableObject
{
    [Header("Distances / Ranges")] public float stopingDistance, detectionDistance, attackRange;

    [Header("Timings")] public float stunDuration = 1f;
    [Tooltip("Tiempo (segundos) entre ataques básicos cuando está en estado Alert.")]
    public float attackCooldown = 1.0f;
}
