using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeWeaponSettings", menuName = "Scriptable Objects/WeaponSettings/MeleeWeaponSettings")]
public class MeleeWeaponSettings : WeaponSettings
{
    public int durability;

    public Vector3 position;

    public Vector3 rotation;
}
