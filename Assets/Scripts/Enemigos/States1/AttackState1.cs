using UnityEngine;
using KinematicCharacterController;

public class AttackState1 : IEnemyState
{
    private EnemyStateHandler ai;
    private IEnemyAttack attack;
    private float disengageBuffer = 5.5f;

    public AttackState1(EnemyStateHandler main)
    {
        ai = main;
        attack = ai.GetComponent<IEnemyAttack>();
    }

    public void OnEnter()
    {
        ai.EnterCombatMode(); 
        Debug.Log($"{ai.name} engaging target.");
    }

    public void Update()
    {
        if (ai.Target == null)
        {
            ai.SetState(ai.GetIdleState());
            return;
        }

        float dist = Vector3.Distance(ai.transform.position, ai.Target.position);

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
            ai.ExitCombatMode();
            ai.SetState(ai.GetAlertState());

            if (attack is RangedAttack gun)
                gun.StartReload();
            else if (attack is TommyGunAttack gun2)
                gun2.StartReload();

            return;
        }

        if (dist <= attack.AttackRange)
        {
            attack.Execute();
        }
        else
        {
            Vector3 dir = (ai.Target.position - ai.transform.position).normalized;
            ai.character.UpdateInputs(
                new EnemyInput { Direction = dir, Move = dir },
                ai.GetBehaviourState()
            );
        }
    }

    public void OnExit()
    {
        Debug.Log($"{ai.name} exiting ATTACK state.");
    }
}
