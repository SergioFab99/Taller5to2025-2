using UnityEngine;

[CreateAssetMenu(fileName = "KnifeWeaponSettings", menuName = "Scriptable Objects/KnifeWeaponSettings")]
public class KnifeWeaponSettings : MeleeWeaponSettings
{
    public float timeBetweenSlash, AttackDelay;
}
