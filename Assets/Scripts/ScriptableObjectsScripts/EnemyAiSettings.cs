using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAISettings", menuName = "Scriptable Objects/EnemyAISettings")]
public class EnemyAiSettings :ScriptableObject
{
    public float stopingDistance, detectionDistance, attackRange, stunDuration;
}
