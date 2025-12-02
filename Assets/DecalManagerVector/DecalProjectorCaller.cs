using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalProjectorCaller : MonoBehaviour
{
    [SerializeField] private DecalProjector decalProjectorPrefab;
    [SerializeField] private float maxDistance = 20f;

    public void ProyectarDecal(Vector3 origin, Vector3 direction)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance))
        {
            DecalProjector decal = Instantiate(decalProjectorPrefab);
            decal.transform.position = hit.point;
            decal.transform.rotation = Quaternion.LookRotation(-hit.normal);
            decal.enabled = true;
        }
    }
}
