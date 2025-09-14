using UnityEngine;

public class StunState : IEnemyState
{
    private EnemyMain ai;
    private float stunDuration;
    private float stunTimer;
    private Vector3 knockbackDir;

    public StunState(EnemyMain main)
    {
        ai = main;
    }

    public void SetKnockback(Vector3 dir)
    {
        knockbackDir = dir;
    }

    public void OnEnter()
    {
        stunDuration = ai.stunDuration;   
        stunTimer = 0f;

        Debug.Log("stunned");

        ai.StopMovement();
        ai.Knockback(knockbackDir);
    }

    public void Update()
    {
        stunTimer += Time.deltaTime;

        if (stunTimer >= stunDuration)
        {
            if (ai.Watching())
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
        Debug.Log("recovered from stun");
    }
}

