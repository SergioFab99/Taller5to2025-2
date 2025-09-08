using UnityEngine;

public class AttackState : IEnemyState
{
    private EnemyMain ai;
    private IEnemyAttack attack;

    private float attackCooldown = 0.5f; 
    private float attackTimer = 0f;
    private float disengageBuffer = 1.5f; // para tener consistencia xd

    public AttackState(EnemyMain main)
    {
        ai = main;
        attack = ai.GetComponent<IEnemyAttack>();
    }

    public void OnEnter()
    {
        attackTimer = 0f;
        Debug.Log("engaging");
    }

    public void Update()
    {
        if (ai.target == null)
        {
            ai.SetState(ai.GetIdleState());
            return;
        }

        if (attack == null)
        {
            ai.SetState(ai.GetAlertState());
            return;
        }

        float distanceToTarget = Vector3.Distance(ai.transform.position, ai.target.position);

        if (distanceToTarget > attack.AttackRange + disengageBuffer)
        {
            ai.SetState(ai.GetAlertState());
            return;
        }

        if (distanceToTarget > attack.AttackRange)
        {
            Vector3 dir = (ai.target.position - ai.transform.position).normalized;
            dir.y = 0f;
            ai.transform.position += dir * ai.moveSpeed * Time.deltaTime;
            return;
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            attack.Execute();
            attackTimer = attackCooldown;
        }
    }

    public void OnExit()
    {
        Debug.Log("disengaging");
    }
}