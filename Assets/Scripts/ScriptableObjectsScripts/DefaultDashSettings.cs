using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "DefaultDashSettings" , menuName = "Scriptable Objects/CharacterMovementSettings/DefaultDashSettings")]
public class DefaultDashSettings : ScriptableObject
{
    [SerializeField] public float dashSpeed = 80f;
    [SerializeField] public float dashDuration = 1f;
    public bool _isDashing;
    public float _dashTimeRemaining;
    public Vector3 _dashDirection;
}
