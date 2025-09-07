using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    [Header("Mixer Groups")]
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup uiGroup;
    [SerializeField] private AudioMixerGroup ambientGroup;

    [Header("Sound Database")]
    [SerializeField] private List<SoundData> sounds;

    private Dictionary<string, SoundData> soundDict = new();
    private List<AudioSource> sfxPool = new();
    private int poolSize = 10;

    private void Awake() {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Cargar sonidos en diccionario
        foreach (var s in sounds) {
            soundDict[s.soundId] = s;
        }

        // Crear pool de audiosources para SFX
        for (int i = 0; i < poolSize; i++) {
            var src = gameObject.AddComponent<AudioSource>();
            sfxPool.Add(src);
        }
    }

    private void OnEnable() {
        AudioEvents.OnPlaySound += PlaySound;
        AudioEvents.OnStopSound += StopSound;
    }

    private void OnDisable() {
        AudioEvents.OnPlaySound -= PlaySound;
        AudioEvents.OnStopSound -= StopSound;
    }

    private void PlaySound(string id, Vector3? pos) {
        if (!soundDict.TryGetValue(id, out var data)) {
            Debug.LogWarning($"[AudioManager] Sound {id} not found!");
            return;
        }

        AudioSource source = GetSource(data.type, pos);

        if (data.clips.Length > 0) {
            var clip = data.clips[Random.Range(0, data.clips.Length)]; 
            source.clip = clip;
        }

        source.volume = data.volume;
        source.pitch = data.pitch;
        source.loop = data.loop;
        source.outputAudioMixerGroup = GetGroup(data.type);

        source.Play();
    }

    private void StopSound(string id) {
        foreach (var src in sfxPool) {
            if (src.isPlaying && src.clip != null && src.clip.name == id) {
                src.Stop();
                break;
            }
        }
    }

    private AudioSource GetSource(SoundType type, Vector3? pos) {
        AudioSource src = null;

        
        if (type == SoundType.Music || type == SoundType.UI || type == SoundType.Ambient) {
            src = gameObject.AddComponent<AudioSource>();
        }
        else {
            
            src = sfxPool.Find(s => !s.isPlaying);
            if (src == null) {
                src = gameObject.AddComponent<AudioSource>();
                sfxPool.Add(src);
            }
        }

        
        if (pos.HasValue) {
            src.transform.position = pos.Value;
            src.spatialBlend = 1f; 
        } else {
            src.spatialBlend = 0f; 
        }

        return src;
    }

    private AudioMixerGroup GetGroup(SoundType type) => type switch {
        SoundType.Music => musicGroup,
        SoundType.SFX => sfxGroup,
        SoundType.UI => uiGroup,
        SoundType.Ambient => ambientGroup,
        _ => null
    };
}
