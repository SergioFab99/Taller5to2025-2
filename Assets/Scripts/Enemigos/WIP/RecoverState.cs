using UnityEngine;
using UnityEngine.AI;
public class RecoverState : IEnemyState
{
    private EnemyMain handler;
    private float recoverTimer;
    private readonly float recoverDuration = 1.0f;

    public RecoverState(EnemyMain handler)
    {
        this.handler = handler;
    }

    public void OnEnter()
    {
        recoverTimer = recoverDuration;
        if (handler.TryGetComponent(out NavMeshAgent agent))
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
        Debug.Log("Enemy recovering...");
    }

    public void Update()
    {
        Transform target = handler.target;
        if (target == null)
        {
            handler.SetState(handler.GetIdleState());
            return;
        }

        recoverTimer -= Time.deltaTime;
        if (recoverTimer > 0f)
            return;

        float dist = Vector3.Distance(handler.transform.position, target.position);

        if (dist <= handler.attackRange + 1f)
        {
            handler.SetState(handler.GetAttackState());
        }
        else if (handler.Watching())
        {
            handler.SetState(handler.GetAlertState());
        }
        else
        {
            handler.SetState(handler.GetIdleState());
        }
    }

    public void OnExit()
    {
        handler.GetComponent<IEnemyAttack>()?.ResetAttackCycle();
        Debug.Log("Finished recovering.");
    }
}
