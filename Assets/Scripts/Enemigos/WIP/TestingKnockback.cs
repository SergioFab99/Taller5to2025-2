using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingKnockback : MonoBehaviour
{
    public float punchRange = 2f;
    public float punchCooldown = 0.5f;
    public float damage = 10f;
    private float cooldownTimer;

    private HashSet<Collider> _recentHits = new HashSet<Collider>();

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q) && cooldownTimer <= 0f)
        {
            cooldownTimer = punchCooldown;
            DoPunch();
        }
    }

    private void DoPunch()
    {
        Vector3 origin = transform.position + Vector3.up * 1f;
        Collider[] hits = Physics.OverlapSphere(origin, punchRange);

        if (hits.Length > 0)
        {
            foreach (Collider c in hits)
            {
                if (_recentHits.Contains(c)) continue;
                _recentHits.Add(c);

                if (c.TryGetComponent(out HealthController hp))
                    hp.TakeDamague(damage);

                if (c.TryGetComponent(out EnemyStateHandler handler))
                {
                    Vector3 dir = (c.transform.position - transform.position).normalized;
                    handler.OnHit(dir);
                }
            }

            StartCoroutine(ClearRecentHits());
        }
        else
        {
            Debug.Log("Player punch missed.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 1f, punchRange);
    }


    private IEnumerator ClearRecentHits()
    {
        yield return null;
        _recentHits.Clear();
    }
}