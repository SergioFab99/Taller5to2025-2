using UnityEngine;

public class PistolEnemy : EnemyMain
{
    public float preferredMinDistance = 5f;
    public float preferredMaxDistance = 12f;
    public float sidestepDistance = 3f;

    public Transform firePoint;

    public override void Movement() //fuck u mauricio
    {
        if (target == null || agent == null) return;

        float dist = Vector3.Distance(transform.position, target.position);

        bool lineBlocked = false;
        if (firePoint != null)
        {
            Vector3 dir = (target.position - firePoint.position).normalized;
            if (Physics.Raycast(firePoint.position, dir, out RaycastHit hit, attackRange))
            {
                if (hit.collider.CompareTag("Enemy") && hit.collider.transform != this.transform)
                {
                    lineBlocked = true;
                }
            }
        }

        if (lineBlocked)
        {
            Vector3 sidestepDir = Random.value > 0.5f ? transform.right : -transform.right;
            Vector3 sidestepPos = transform.position + sidestepDir * sidestepDistance;
            agent.isStopped = false;
            agent.SetDestination(sidestepPos);
            return;
        }

        if (dist < preferredMinDistance)
        {
            Vector3 retreatDir = (transform.position - target.position).normalized;
            Vector3 retreatPos = transform.position + retreatDir * 3f;
            agent.isStopped = false;
            agent.SetDestination(retreatPos);
        }
        else if (dist > preferredMaxDistance)
        {
            agent.isStopped = false;
            MoveTowardsTarget();
        }
        else
        {
            agent.isStopped = true;
        }
    }
}
