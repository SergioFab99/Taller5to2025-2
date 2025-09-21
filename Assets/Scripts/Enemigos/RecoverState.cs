using UnityEngine;

public class RecoverState : IEnemyState
{
    private EnemyMain ai;
    private float recoverTimer;
    private float recoverDuration = 1.0f; 

    public RecoverState(EnemyMain main)
    {
        ai = main;
    }

    public void OnEnter()
    {
        recoverTimer = recoverDuration;
        ai.StopMovement();
        Debug.Log($"recovering");
    }

    public void Update()
    {
        if (ai.target == null)
        {
            ai.SetState(ai.GetIdleState());
            return;
        }

        recoverTimer -= Time.deltaTime;

        if (recoverTimer <= 0f)
        {
            float dist = Vector3.Distance(ai.transform.position, ai.target.position);

            if (dist <= ai.attackRange + 1f) 
            {
                ai.SetState(ai.GetAttackState()); 
            }
            else if (ai.Watching())
            {
                ai.SetState(ai.GetAlertState()); 
            }
            else
            {
                ai.SetState(ai.GetIdleState()); 
            }
        }
    }

    public void OnExit()
    {
        ai.GetComponent<IEnemyAttack>()?.ResetAttackCycle();
        Debug.Log("finished recovering.");
    }
}
