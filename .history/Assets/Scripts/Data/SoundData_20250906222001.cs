using UnityEngine;

public enum SoundType { SFX, Music, UI, Ambient }

[CreateAssetMenu(fileName = "NewSound", menuName = "Audio/Sound Data")]
public class SoundData : ScriptableObject {
    public string soundId; 
    public AudioClip[] clips; 
    public SoundType type;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(-3f, 3f)] public float pitch = 1f;
    public bool loop = false;
}