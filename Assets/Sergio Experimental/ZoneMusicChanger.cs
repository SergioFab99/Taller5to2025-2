using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ZoneMusicChanger : MonoBehaviour
{
    [Header("Zone Settings")]
    public Vector3 zoneSize = new Vector3(5f, 5f, 5f);
    [Tooltip("Duration of the crossfade in seconds")]
    public float fadeDuration = 2.0f;

    [Header("References")]
    public Transform player;

    [Header("Audio")]
    public AudioClip[] enterClips = new AudioClip[3];  
    public AudioClip[] exitClips = new AudioClip[3];

    [Header("Debug")]
    public Color gizmoColor = new Color(0, 1, 0, 0.4f);

    private bool wasInside = false;
    private AudioSource[] enterAudioSources;
    private AudioSource[] exitAudioSources;
    private Coroutine currentFadeCoroutine;

    void Start()
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
                exitAudioSources[i].volume = 1f;
                exitAudioSources[i].Play();
            }
        }

        Debug.Log("ZoneMusicChanger: Iniciado - ExitClips (exteriores) sonando por defecto");
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("ZoneMusicChanger: player no asignado");
            return;
        }

        Vector3 localPos = transform.InverseTransformPoint(player.position);

        bool isInside = Mathf.Abs(localPos.x) <= zoneSize.x / 2f &&
                        Mathf.Abs(localPos.y) <= zoneSize.y / 2f &&
                        Mathf.Abs(localPos.z) <= zoneSize.z / 2f;

        if (isInside != wasInside)
        {
            Debug.Log("ZoneMusicChanger: cambio de estado de zona -> " + (isInside ? "DENTRO" : "FUERA"));
            HandleZoneChange(isInside);
            wasInside = isInside;
        }
    }

    void HandleZoneChange(bool isInside)
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        if (isInside)
        {
            Debug.Log("ZoneMusicChanger: ENTRÓ en la zona - Activando EnterClips, desactivando ExitClips");
            currentFadeCoroutine = StartCoroutine(CrossFade(enterAudioSources, exitAudioSources));
        }
        else
        {
            Debug.Log("ZoneMusicChanger: SALIÓ de la zona - Activando ExitClips, desactivando EnterClips");
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
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(transform.position, zoneSize);

        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.1f);
        Gizmos.DrawCube(transform.position, zoneSize);

#if UNITY_EDITOR
        zoneSize = Handles.ScaleHandle(zoneSize, transform.position, transform.rotation, HandleUtility.GetHandleSize(transform.position));
#endif
    }
}