using UnityEngine;

public class AttackState : IEnemyState
{
    private EnemyMain ai;
    private IEnemyAttack attack;

    private float disengageBuffer = 5.5f;

    public AttackState(EnemyMain main)
    {
        ai = main;
        attack = ai.GetComponent<IEnemyAttack>();
    }

    public void OnEnter()
    {
        Debug.Log("engaging");
    }

    public void Update()
    {
        if (ai.target == null)
        {
            ai.SetState(ai.GetIdleState());
            return;
        }

        float dist = Vector3.Distance(ai.transform.position, ai.target.position);

        if (attack.IsFinished)
        {
            if (attack.WasInterrupted)
            {
                attack.ResetAttackCycle();
                ai.SetState(ai.GetExposedState());
            }
            else
            {
                attack.ResetAttackCycle();
                ai.SetState(ai.GetRecoverState());
            }
            return;
        }

        if (attack.IsAttacking)
            return;

        if (dist > attack.AttackRange + disengageBuffer)
        {
            ai.SetState(ai.GetAlertState());
            return;
        }

        if (dist <= attack.AttackRange)
        {
            attack.Execute();
        }
        else
        {
            ai.MoveTowardsTarget();
        }
    }

    public void OnExit()
    {
        Debug.Log("exiting attack state");
    }
}
