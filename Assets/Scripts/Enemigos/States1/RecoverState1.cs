using UnityEngine;
using UnityEngine.AI;
public class RecoverState1 : IEnemyState
{
    private EnemyStateHandler ai;
    private float recoverTimer;
    private readonly float recoverDuration = 1.0f;

    public RecoverState1(EnemyStateHandler handler)
    {
        ai = handler;
    }

    public void OnEnter()
    {
        recoverTimer = recoverDuration;

        if (ai.TryGetComponent(out NavMeshAgent agent))
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        Debug.Log($"{ai.name} is recovering...");
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
        if (recoverTimer > 0f)
            return;

        float dist = Vector3.Distance(ai.transform.position, target.position);

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