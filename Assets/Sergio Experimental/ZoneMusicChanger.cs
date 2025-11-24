using UnityEngine;
using System.Collections.Generic;

public class ZoneMusicChanger : MonoBehaviour
{
    [Header("Zone Settings")]
    public Vector3 zoneSize = new Vector3(5f, 5f, 5f);

    [Header("References")]
    public Transform player;

    [Header("Audio")]
    public AudioSource audioSource; 
    public AudioClip enterClip;  
    public AudioClip exitClip;    

    [Header("Scripts que se ACTIVAN al entrar")]
    public List<MonoBehaviour> scriptsToEnableOnEnter = new List<MonoBehaviour>();

    [Header("Scripts que se DESACTIVAN al entrar")]
    public List<MonoBehaviour> scriptsToDisableOnEnter = new List<MonoBehaviour>();

    [Header("Debug")]
    public Color gizmoColor = new Color(0, 1, 0, 0.4f);

    private bool wasInside = false;

    void Start()
    {
        SetScriptsState(scriptsToEnableOnEnter, false);
        SetScriptsState(scriptsToDisableOnEnter, true);
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
            SetScriptsState(scriptsToEnableOnEnter, true);
            SetScriptsState(scriptsToDisableOnEnter, false);

            PlayClip(enterClip);
        }
        else
        {
            Debug.Log("ZoneMusicChanger: SALIÓ de la zona");
            SetScriptsState(scriptsToEnableOnEnter, false);
            SetScriptsState(scriptsToDisableOnEnter, true);

            PlayClip(exitClip);
        }
    }

    void PlayClip(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.clip = clip;
        audioSource.Play();

    }

    void SetScriptsState(List<MonoBehaviour> list, bool state)
    {
        foreach (var script in list)
        {
            if (script != null)
            {
                script.enabled = state;
                Debug.Log($"ZoneMusicChanger: set {script.GetType().Name} en {script.gameObject.name} a {state}");
            }
            else
            {
                Debug.LogWarning("ZoneMusicChanger: referencia de script nula en la lista");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(transform.position, zoneSize);

        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.1f);
        Gizmos.DrawCube(transform.position, zoneSize);
    }
}
