using UnityEngine;
using System.Collections;

[System.Serializable]
public struct MusicZone
{
    public Vector3 center;
    public Vector3 size;

    [Header("Audios de la zona")]
    public AudioClip[] enterClips;
}

public class ManagerAbsoluteZoneMusic : MonoBehaviour
{
    public Transform player;

    [Header("Audios Exteriores (globales)")]
    public AudioClip[] exteriorClips;

    [Header("Zonas")]
    public MusicZone[] zones;

    public float fadeDuration = 1.5f;

    private AudioSource[] exteriorSources;
    private AudioSource[] currentEnterSources;
    private int currentZoneIndex = -1;

    void Start()
    {
      
        exteriorSources = CreateAudioGroup("ExteriorAudio", exteriorClips, true);
        PlayGroup(exteriorSources, 1f);
    }

    void Update()
    {
        int zone = GetCurrentZone(player.position);

        if (zone != currentZoneIndex)
        {
            HandleZoneChange(zone);
            currentZoneIndex = zone;
        }
    }

    int GetCurrentZone(Vector3 playerPos)
    {
        for (int i = 0; i < zones.Length; i++)
        {
            Vector3 globalCenter = transform.position + zones[i].center;
            Bounds b = new Bounds(globalCenter, zones[i].size);

            if (b.Contains(playerPos))
                return i;
        }

        return -1;
    }

    void HandleZoneChange(int newZone)
    {
        StopAllCoroutines();

        if (currentEnterSources != null)
            StartCoroutine(FadeGroup(currentEnterSources, 0f, true));

        if (newZone == -1)
        {
            StartCoroutine(FadeGroup(exteriorSources, 1f, false));
            currentEnterSources = null;
            return;
        }

        MusicZone zone = zones[newZone];

        StartCoroutine(FadeGroup(exteriorSources, 0f, false));

        currentEnterSources = CreateAudioGroup($"Zone_{newZone}_Enter", zone.enterClips, true);
        PlayGroup(currentEnterSources, 0f);
        StartCoroutine(FadeGroup(currentEnterSources, 1f, false));
    }

    AudioSource[] CreateAudioGroup(string name, AudioClip[] clips, bool loop)
    {
        AudioSource[] group = new AudioSource[clips.Length];

        for (int i = 0; i < clips.Length; i++)
        {
            GameObject obj = new GameObject($"{name}_{i}");
            obj.transform.SetParent(transform);

            AudioSource src = obj.AddComponent<AudioSource>();
            src.clip = clips[i];
            src.loop = loop;
            src.playOnAwake = false;
            src.volume = 0;

            group[i] = src;
        }

        return group;
    }

    void PlayGroup(AudioSource[] group, float startVolume)
    {
        foreach (AudioSource s in group)
        {
            if (s == null) continue;
            s.volume = startVolume;
            s.Play();
        }
    }

    IEnumerator FadeGroup(AudioSource[] group, float targetVolume, bool stopAfter)
    {
        float time = 0;

        float[] startVolumes = new float[group.Length];
        for (int i = 0; i < group.Length; i++)
            startVolumes[i] = group[i].volume;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;

            for (int i = 0; i < group.Length; i++)
                group[i].volume = Mathf.Lerp(startVolumes[i], targetVolume, t);

            yield return null;
        }

        for (int i = 0; i < group.Length; i++)
        {
            group[i].volume = targetVolume;
            if (stopAfter && targetVolume == 0)
                group[i].Stop();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (zones == null) return;

        foreach (var zone in zones)
        {
            Gizmos.color = new Color(0, 1, 0, 0.25f);
            Gizmos.DrawCube(transform.position + zone.center, zone.size);

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + zone.center, zone.size);
        }
    }
}
