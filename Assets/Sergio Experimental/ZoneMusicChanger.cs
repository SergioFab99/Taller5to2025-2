using UnityEngine;
using System.Collections.Generic;

public class ZoneMusicChanger : MonoBehaviour
{
    [Header("Zone Settings")]
    public Vector3 zoneSize = new Vector3(5f, 5f, 5f);

    [Header("References")]
    public Transform player;

    [Header("Scripts que se ACTIVAN al entrar")]
    public List<MonoBehaviour> scriptsToEnableOnEnter = new List<MonoBehaviour>();

    [Header("Scripts que se DESACTIVAN al entrar")]
    public List<MonoBehaviour> scriptsToDisableOnEnter = new List<MonoBehaviour>();

    [Header("Debug")]
    public Color gizmoColor = new Color(0, 1, 0, 0.4f);

    private bool wasInside = false;

    void Update()
    {
        if (player == null) return;

        Vector3 localPos = transform.InverseTransformPoint(player.position);

        bool isInside = Mathf.Abs(localPos.x) <= zoneSize.x / 2f &&
                        Mathf.Abs(localPos.y) <= zoneSize.y / 2f &&
                        Mathf.Abs(localPos.z) <= zoneSize.z / 2f;

        if (isInside != wasInside)
        {
            HandleZoneChange(isInside);
            wasInside = isInside;
        }
    }

    void HandleZoneChange(bool isInside)
    {
        if (isInside)
        {
            SetScriptsState(scriptsToEnableOnEnter, true);
            SetScriptsState(scriptsToDisableOnEnter, false);
        }
        else
        {
            SetScriptsState(scriptsToEnableOnEnter, false);
            SetScriptsState(scriptsToDisableOnEnter, true);
        }
    }

    void SetScriptsState(List<MonoBehaviour> list, bool state)
    {
        foreach (var script in list)
        {
            if (script != null)
                script.enabled = state;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(Vector3.zero, zoneSize);

        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.1f);
        Gizmos.DrawCube(Vector3.zero, zoneSize);
    }
}
