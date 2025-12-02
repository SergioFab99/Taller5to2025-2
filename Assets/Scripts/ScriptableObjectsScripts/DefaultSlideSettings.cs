using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "DefaultSlideSettings", menuName = "Scriptable Objects/CharacterMovementSettings/DefaultSlideSettings")]
public class DefaultSlideSettings : ScriptableObject
{
    public float SlideStartSpeed,
                 SlideEndSpeed,
                 SlideFriction,
                 SlideSteerAcceleration, 
                 SlideGravity;
}
