using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "DefaultSMoveSettings", menuName = "Scriptable Objects/CharacterMovementSettings/DefaultSMoveSettings")]
public class DefaultMoveSettings : ScriptableObject
{
   public float Speed,
                Response;
}
