using Sirenix.OdinInspector;
using UnityEngine;



[System.Serializable]
public abstract class WeaponSettings : ScriptableObject
{
    public float damague;

    public TagSO[] properties;
    public float shakeForce;

}
