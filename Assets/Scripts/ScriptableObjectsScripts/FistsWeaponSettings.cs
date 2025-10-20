using UnityEngine;

[CreateAssetMenu(fileName = "FistsWeaponSettings", menuName = "Scriptable Objects/WeaponSettings/FistsWeaponSettings")]
public class FistsWeaponSettings : WeaponSettings
{
    public float radius;
    public float timeBetweenAttacks;
    public float timeToDoublePunch;
    public float punchDuration;
}
