using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "DefaultGrab&ThrowSettings", menuName = "Scriptable Objects/CharacterCombatSettings/DefaultGrab&ThrowSettings")]
public class DefaultGrabThrowSettings : ScriptableObject
{
    [SerializeField] public float grabRange = 2f;
    [SerializeField] public float throwForce = 10f;
}
    

