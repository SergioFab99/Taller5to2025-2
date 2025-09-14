using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "DefaultStanceSettings", menuName = "Scriptable Objects/CharacterMovementSettings/DefaultStanceSettings")]
public class DefaultStanceSettings : ScriptableObject
{
   public float Speed,
                Response;
}
