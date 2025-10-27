using UnityEngine;
using UnityEngine.AI;
public class RecoverState1 : IEnemyState
{
    private EnemyStateHandler ai;
    private float recoverTimer;
    private float recoverDuration = 1.0f;

    public RecoverState1(EnemyStateHandler handler)
    {
        ai = handler;
    }

    public void OnEnter()
    {
        recoverTimer = recoverDuration;


        if (ai.agent != null && ai.agent.enabled && ai.agent.isOnNavMesh)
        {
            ai.agent.velocity = Vector3.zero;
            ai.agent.isStopped = true;
            ai.agent.ResetPath();
        }

        ai.character.Motor.BaseVelocity = Vector3.zero;
        ai.character.UpdateInputs(new EnemyInput { Move = Vector3.zero, Direction = Vector3.zero }, ai.GetBehaviourState());

        Debug.Log($"{ai.name} is recovering... for {recoverTimer}");
    }

    public void Update()
    {
        Transform target = ai.Target;
        if (target == null)
        {
            ai.SetState(ai.GetIdleState());
            return;
        }

        recoverTimer -= Time.deltaTime;
        if (recoverTimer > 0f) return;

        float dist = Vector3.Distance(ai.character.transform.position, target.position);

        if (dist <= ai.enemySettings.AISettings.attackRange + 1f)
        {
            ai.EnterCombatMode();
            ai.SetState(ai.GetAttackState());
        }
        else if (ai.CheckTargetOnView())
        {
            ai.ExitCombatMode();
            ai.SetState(ai.GetAlertState());
        }
        else
        {
            ai.ExitCombatMode();
            ai.SetState(ai.GetIdleState());
        }
    }

    public void OnExit()
    {
        ai.GetComponent<IEnemyAttack>()?.ResetAttackCycle();
        Debug.Log($"{ai.name} finished recovering.");
    }
}