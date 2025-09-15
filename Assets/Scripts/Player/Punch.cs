using System;
using UnityEngine;
using UnityEngine.UI;

public class Punch : MonoBehaviour
{
    bool hitDone = false;
    Vector3 lastPos;
    public float radius = 0.2f;
    public Vector3 dir;
    public LayerMask hitMask;
    private bool isActive;

    public int dmg;

    private void Start()
    {
        lastPos = transform.position;
    }
    public void FixedUpdate()
    {
        Vector3 currentPos = transform.position;
        dir = currentPos - lastPos;
        float dist = dir.magnitude;
        if (dist > 0.0001f)
        {
            RaycastHit hit;

            if (Physics.SphereCast(transform.position, radius, transform.forward, out hit, dist, hitMask, QueryTriggerInteraction.Ignore))
            {
                hitDone = true;
                if (isActive)
                {
                    PerformOnHit(hit.collider, hit.point, hit.normal);

                }
            }
        }
    }

    public void PerformOnHit(Collider col, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (col.gameObject.TryGetComponent<TagContainer>(out TagContainer tagContainer) && tagContainer.HasTag("Damagable") && !tagContainer.HasTag("Player"))
        {
            Debug.Log("Hitted");
            col.gameObject.GetComponent<EnemyLife>().TakeDamage();
        }
    }

    public void ActivateOrDeactivePunch(bool trigger)
    {
        isActive = trigger;
    }
}
