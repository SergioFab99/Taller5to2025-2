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
    public AudioSource audioSource; 
    public AudioClip[] enterClips = new AudioClip[3];  
    public AudioClip[] exitClips = new AudioClip[3];    

    [Header("Debug")]
    public Color gizmoColor = new Color(0, 1, 0, 0.4f);

    private bool wasInside = false;

    void Start()
    {
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
            Debug.Log("ZoneMusicChanger: cambio de estado de zona -> " + isInside);
            HandleZoneChange(isInside);
            wasInside = isInside;
        }
    }

    void HandleZoneChange(bool isInside)
    {
        if (isInside)
        {
            Debug.Log("ZoneMusicChanger: ENTRÓ en la zona");

            PlayClip(enterClips);
        }
        else
        {
            Debug.Log("ZoneMusicChanger: SALIÓ de la zona");

            PlayClip(exitClips);
        }
    }

    void PlayClip(AudioClip[] clips)
    {
        if (audioSource == null || clips == null || clips.Length == 0)
            return;

        AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
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
