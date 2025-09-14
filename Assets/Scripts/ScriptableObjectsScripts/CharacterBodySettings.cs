using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "CharacterBodySettings", menuName = "Scriptable Objects/CharacterBodySettings")]
public class CharacterBodySettings : ScriptableObject
{
  public float Height,
               CameraHeight;
}
