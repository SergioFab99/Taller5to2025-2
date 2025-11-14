using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EnemySettingsList", menuName = "Scriptable Objects/EnemySettingsList")]
public class EnemySettingsList: ScriptableObject
{
    public EnemySettings DefaultEnemySettings;
    public EnemySettings AlertEnemySettings;

    public EnemyAiSettings AISettings;
}
