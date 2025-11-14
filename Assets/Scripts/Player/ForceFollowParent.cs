using UnityEngine;

public class ForceFollowParent : MonoBehaviour
{
    [SerializeField] private Transform player;

    private Vector3 offset;
    private Quaternion rotOffset;

    void Start()
    {
        if (!player)
        {
            Debug.LogError("Debes asignar el Player.");
            enabled = false;
            return;
        }

        offset = player.InverseTransformPoint(transform.position);
        rotOffset = Quaternion.Inverse(player.rotation) * transform.rotation;
    }

    void LateUpdate()
    {
        if (!player) return;

        transform.position = player.TransformPoint(offset);
        transform.rotation = player.rotation * rotOffset;
    }
}
