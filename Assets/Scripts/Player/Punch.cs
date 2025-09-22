using System;
using UnityEditor;
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
        if (hitDone)
        {
            return;
        }
        Vector3 currentPos = transform.position;
        dir = currentPos - lastPos;
        float dist = dir.magnitude;
        if (dist > 0.0001f)
        {
            RaycastHit hit;

            if (Physics.SphereCast(transform.position, radius, dir.normalized, out hit, dist, hitMask, QueryTriggerInteraction.Ignore))
            {
                if (isActive)
                {
                    PerformOnHit(hit.collider, hit.point, hit.normal);

                }
            }
        }
        lastPos = currentPos;
        
    }

    public void PerformOnHit(Collider col, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (col.gameObject.TryGetComponent<TagContainer>(out TagContainer tagContainer) && tagContainer.HasTag("Damagable") && !tagContainer.HasTag("Player"))
        {
            Debug.Log("Hitted");
            col.gameObject.GetComponent<EnemyLife>().TakeDamage();
            hitDone = true;
        }
    }

    public void ActivateOrDeactivePunch(bool trigger)
    {
        isActive = trigger;
        if (isActive)
        {
            hitDone = false;
            var col = Physics.OverlapSphere(transform.position, radius, hitMask.value, QueryTriggerInteraction.Ignore);
            if (col != null && col.Length > 0)
            {
                var distance = (col[0].ClosestPoint(transform.position) - transform.position).normalized;
                PerformOnHit(col[0], col[0].ClosestPoint(transform.position), distance);
            }


        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

         // Dibuja la esfera inicial
        Gizmos.DrawWireSphere(transform.position, radius);

        // Dibuja el tubo del SphereCast usando la última dirección y distancia calculada
        Vector3 direction = dir.normalized;
        float dist = dir.magnitude;
         int steps = 10;
         
        for (int i = 1; i <= steps; i++)
        {
            float t = (dist / steps) * i;
            Vector3 center = transform.position + direction * t;
            Gizmos.DrawWireSphere(center, radius);
        }
    }
}
