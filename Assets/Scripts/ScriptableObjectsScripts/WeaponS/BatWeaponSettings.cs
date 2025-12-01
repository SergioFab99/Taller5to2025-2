using UnityEngine;

[CreateAssetMenu(fileName = "BatWeaponSettings", menuName = "Scriptable Objects/BatWeaponSettings")]
public class BatWeaponSettings : MeleeWeaponSettings
{
    public float swingRadius;
    public float swingRange = 1.5f;
    public float swingDuration = 0.3f;
    public float timeBetweenSwings = 0.5f;
    public float AttackDelay;
}
