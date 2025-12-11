using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public struct MusicZone
{
    [Tooltip("Centro de la zona, relativo a la posición del GameObject principal.")]
    public Vector3 center;
    [Tooltip("Tamaño de la zona (Largo, Alto, Profundidad).")]
    public Vector3 size;
    [Tooltip("Color del Gizmo para esta zona en el Editor.")]
    public Color gizmoColor;
}

public class ManagerAbsoluteZoneMusic : MonoBehaviour 
{
    [Header("Global Settings")]
    [Tooltip("Duración del crossfade en segundos")]
    public float fadeDuration = 2.0f;
    
    [Tooltip("La Transformación del jugador que será monitoreado.")]
    public Transform player;

    [Header("Multiple Zones")]
    public MusicZone[] zones;

    [Header("Audio Clips")]
    public AudioClip[] enterClips = new AudioClip[3];
    public AudioClip[] exitClips = new AudioClip[3];

    [Header("Gizmo Settings")]
    [Range(0f, 1f)]
    public float gizmoFillAlpha = 0.85f;

    private bool wasInsideAnyZone = false;
    private AudioSource[] enterAudioSources;
    private AudioSource[] exitAudioSources;
    private Coroutine currentFadeCoroutine;
    void Start()
    {
        InitializeAudioSources();
        if (exitAudioSources.Length > 0 && exitAudioSources[0] != null)
        {
             foreach (AudioSource source in exitAudioSources)
            {
                if (source != null)
                {
                    source.volume = 1f;
                    source.Play();
                }
            }
        }
        Debug.Log("ManagerAbsoluteZoneMusic: Iniciado - Múltiples zonas configuradas.");
    }

    void InitializeAudioSources()
    {
        enterAudioSources = new AudioSource[enterClips.Length];
        for (int i = 0; i < enterClips.Length; i++)
        {
            if (enterClips[i] != null)
            {
                GameObject enterObj = new GameObject($"EnterAudio_{i}");
                enterObj.transform.SetParent(transform);
                enterAudioSources[i] = enterObj.AddComponent<AudioSource>();
                enterAudioSources[i].clip = enterClips[i];
                enterAudioSources[i].loop = true;
                enterAudioSources[i].playOnAwake = false;
                enterAudioSources[i].volume = 0f;
            }
        }
        exitAudioSources = new AudioSource[exitClips.Length];
        for (int i = 0; i < exitClips.Length; i++)
        {
            if (exitClips[i] != null)
            {
                GameObject exitObj = new GameObject($"ExitAudio_{i}");
                exitObj.transform.SetParent(transform);
                exitAudioSources[i] = exitObj.AddComponent<AudioSource>();
                exitAudioSources[i].clip = exitClips[i];
                exitAudioSources[i].loop = true;
                exitAudioSources[i].playOnAwake = true;
                exitAudioSources[i].volume = 0f;
            }
        }
    }


    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("ManagerAbsoluteZoneMusic: player no asignado. No se puede verificar la zona.");
            return;
        }

        bool isInsideNow = CheckIfPlayerIsInsideAnyZone(player.position);

        if (isInsideNow != wasInsideAnyZone)
        {
            Debug.Log("ManagerAbsoluteZoneMusic: Cambio de estado de zona MÚLTIPLE -> " + (isInsideNow ? "DENTRO DE UNA ZONA" : "FUERA DE TODAS LAS ZONAS"));
            HandleZoneChange(isInsideNow);
            wasInsideAnyZone = isInsideNow;
        }
    }

    bool CheckIfPlayerIsInsideAnyZone(Vector3 playerPosition)
    {
        Vector3 playerPosGlobal = playerPosition;

        foreach (MusicZone zone in zones)
        {
            Vector3 zoneCenterGlobal = transform.position + zone.center;
            Bounds bounds = new Bounds(zoneCenterGlobal, zone.size);
            if (bounds.Contains(playerPosGlobal))
            {
                return true;
            }
        }
        return false;
    }

    void HandleZoneChange(bool isInside)
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        if (isInside)
        {
            currentFadeCoroutine = StartCoroutine(CrossFade(enterAudioSources, exitAudioSources));
        }
        else
        {
            currentFadeCoroutine = StartCoroutine(CrossFade(exitAudioSources, enterAudioSources));
        }
    }

    IEnumerator CrossFade(AudioSource[] toFadeIn, AudioSource[] toFadeOut)
    {
        float timer = 0f;

        foreach (AudioSource source in toFadeIn)
        {
            if (source != null)
            {
                if (!source.isPlaying) source.Play();
            }
        }

        float[] startVolIn = new float[toFadeIn.Length];
        for (int i = 0; i < toFadeIn.Length; i++)
        {
            if (toFadeIn[i] != null) startVolIn[i] = toFadeIn[i].volume;
        }

        float[] startVolOut = new float[toFadeOut.Length];
        for (int i = 0; i < toFadeOut.Length; i++)
        {
            if (toFadeOut[i] != null) startVolOut[i] = toFadeOut[i].volume;
        }

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            for (int i = 0; i < toFadeIn.Length; i++)
            {
                if (toFadeIn[i] != null)
                    toFadeIn[i].volume = Mathf.Lerp(startVolIn[i], 1f, t);
            }

            for (int i = 0; i < toFadeOut.Length; i++)
            {
                if (toFadeOut[i] != null)
                    toFadeOut[i].volume = Mathf.Lerp(startVolOut[i], 0f, t);
            }

            yield return null;
        }

        foreach (AudioSource source in toFadeIn)
        {
            if (source != null) source.volume = 1f;
        }

        foreach (AudioSource source in toFadeOut)
        {
            if (source != null)
            {
                source.volume = 0f;
                source.Stop();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (zones == null) return;

        for (int i = 0; i < zones.Length; i++)
        {
            Vector3 zoneCenterGlobal = transform.position + zones[i].center;
            Gizmos.color = zones[i].gizmoColor; 
            Gizmos.DrawWireCube(zoneCenterGlobal, zones[i].size);

            Color fillColor = zones[i].gizmoColor;
            fillColor.a = Mathf.Max(zones[i].gizmoColor.a, gizmoFillAlpha);
            Gizmos.color = fillColor;
            Gizmos.DrawCube(zoneCenterGlobal, zones[i].size);
            
#if UNITY_EDITOR
            Handles.Label(zoneCenterGlobal + new Vector3(0, zones[i].size.y / 2f + 0.5f, 0), $"Zone {i}", EditorStyles.boldLabel);
            MusicZone tempZone = zones[i];
            tempZone.center = Handles.PositionHandle(zoneCenterGlobal, Quaternion.identity) - transform.position;
            zones[i] = tempZone;
#endif
        }
    }
}