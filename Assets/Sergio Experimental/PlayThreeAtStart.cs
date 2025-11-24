using UnityEngine;

public class PlayThreeAtStart : MonoBehaviour
{
    [Header("Audios interiores")]
    [SerializeField] private AudioClip clip1;
    [SerializeField] private AudioClip clip2;
    [SerializeField] private AudioClip clip3;

    [Header("Volúmenes (0 - 1)")]
    [Range(0f, 1f)] [SerializeField] private float volume1 = 1f;
    [Range(0f, 1f)] [SerializeField] private float volume2 = 1f;
    [Range(0f, 1f)] [SerializeField] private float volume3 = 1f;

    private AudioSource src1;
    private AudioSource src2;
    private AudioSource src3;

    void Start()
    {
        src1 = gameObject.AddComponent<AudioSource>();
        src2 = gameObject.AddComponent<AudioSource>();
        src3 = gameObject.AddComponent<AudioSource>();

        SetupAndPlay(src1, clip1, volume1);
        SetupAndPlay(src2, clip2, volume2);
        SetupAndPlay(src3, clip3, volume3);
    }

    private void SetupAndPlay(AudioSource src, AudioClip clip, float vol)
    {
        if (clip == null) return;

        src.clip = clip;
        src.loop = true;
        src.playOnAwake = false;
        src.volume = Mathf.Clamp01(vol);
        src.spatialBlend = 0f;
    }
}
