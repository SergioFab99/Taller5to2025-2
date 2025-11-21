using UnityEngine;
using System.Collections.Generic;

public class ZoneMusicChanger : MonoBehaviour
{
    [Header("Zone Settings")]
    [Tooltip("Dimensions of the detection zone (Width, Height, Depth)")]
    public Vector3 zoneSize = new Vector3(5f, 5f, 5f);

    [Header("References")]
    [Tooltip("The player transform to track")]
    public Transform player;

    [Tooltip("List of scripts to enable when inside the zone, and disable when outside")]
    public List<MonoBehaviour> scriptsToToggle = new List<MonoBehaviour>();

    [Header("Debug")]
    [Tooltip("Color of the zone gizmo")]
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
            ToggleScripts(isInside);
            wasInside = isInside;
        }
    }

    void ToggleScripts(bool enable)
    {
        foreach (var script in scriptsToToggle)
        {
            if (script != null)
            {
                script.enabled = enable;
            }
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