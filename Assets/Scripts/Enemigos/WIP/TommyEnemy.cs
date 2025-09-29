using UnityEngine;

public class TommyEnemy : EnemyMain
{
    public override void Movement()
    {
        if (target == null || agent == null) return;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist < attackRange * 0.7f)
        {
            // too close, backpedal
            Vector3 retreat = transform.position - (target.position - transform.position).normalized * 5f;
            agent.SetDestination(retreat);
        }
        else
        {
            MoveTowardsTarget();
        }
    }
}
