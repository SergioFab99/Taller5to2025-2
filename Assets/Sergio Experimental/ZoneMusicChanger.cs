using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ZoneMusicChanger : MonoBehaviour
{
    [Header("Zone Settings")]
    public Vector3 zoneSize = new Vector3(5f, 5f, 5f);

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

    void Start()
    {
        // Inicializar AudioSources para enterClips
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
            }
        }

        // Inicializar AudioSources para exitClips
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
                exitAudioSources[i].playOnAwake = true; // Los exitClips empiezan sonando
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
        if (isInside)
        {
            Debug.Log("ZoneMusicChanger: ENTRÓ en la zona - Activando EnterClips, desactivando ExitClips");
            
            // Activar enterClips
            foreach (AudioSource source in enterAudioSources)
            {
                if (source != null && !source.isPlaying)
                {
                    source.Play();
                }
            }

            // Desactivar exitClips
            foreach (AudioSource source in exitAudioSources)
            {
                if (source != null && source.isPlaying)
                {
                    source.Stop();
                }
            }
        }
        else
        {
            Debug.Log("ZoneMusicChanger: SALIÓ de la zona - Activando ExitClips, desactivando EnterClips");
            
            // Activar exitClips
            foreach (AudioSource source in exitAudioSources)
            {
                if (source != null && !source.isPlaying)
                {
                    source.Play();
                }
            }

            // Desactivar enterClips
            foreach (AudioSource source in enterAudioSources)
            {
                if (source != null && source.isPlaying)
                {
                    source.Stop();
                }
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