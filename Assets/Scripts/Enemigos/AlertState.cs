using UnityEngine;

public class AlertState : IEnemyState
{
    private EnemyMain ai;
    private float alertTimer;
    private float maxAlertTime = 3f;

    public AlertState(EnemyMain main)
    {
        ai = main;
    }

    public void OnEnter()
    {
        alertTimer = 0f;
        Debug.Log("spotting");
    }

    public void Update()
    {
        alertTimer += Time.deltaTime;

        if (!ai.Watching())
        {
            if (alertTimer >= maxAlertTime)
            {
                ai.SetState(ai.GetIdleState());
            }
            return;
        }

        ai.MoveTowardsTarget();

        if (ai.Attacking())
        {
            if (!(ai.GetCurrentState() is AttackState))
            {
                ai.SetState(ai.GetAttackState());
            }

        }
    }

    public void OnExit()
    {
        Debug.Log("no longer spotting");
    }
}
