using UnityEngine;

public class BlockState : IEnemyState
{
    private EnemyMain ai;
    private float blockTimer;
    private float blockDuration = 1.0f;  

    public BlockState(EnemyMain main)
    {
        ai = main;
    }

    public void OnEnter()
    {
        blockTimer = blockDuration;

        ai.StopMovement();
        Debug.Log($"is blocking");
    }

    public void Update()
    {
        if (ai.target == null)
        {
            ai.SetState(ai.GetAlertState());
            return;
        }

        blockTimer -= Time.deltaTime;

        if (blockTimer <= 0f)
        {
            ai.SetState(ai.GetAttackState());
        }
    }

    public void OnExit()
    {
        Debug.Log("lowered guard.");
    }
}
