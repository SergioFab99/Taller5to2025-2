using UnityEngine;

[CreateAssetMenu(fileName = "BatWeaponSettings", menuName = "Scriptable Objects/BatWeaponSettings")]
public class BatWeaponSettings : MeleeWeaponSettings
{
    
    public float timeBetweenSwings = 0.5f;
    public float AttackDelay;
}
