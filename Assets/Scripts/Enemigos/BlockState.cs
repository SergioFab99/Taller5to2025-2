using UnityEngine;

public class BlockState : IEnemyState
{
    private EnemyStateHandler ai;
    private float blockTimer;
    private float blockDuration = 1.0f;  

    public BlockState(EnemyStateHandler main)
    {
        ai = main;
    }

    public void OnEnter()
    {
        blockTimer = blockDuration;


        Debug.Log($"is blocking");
    }

    public void Update()
    {
        if (ai.Target == null)
        {
            ai.SetState(ai.GetAlertState());
            return;
        }

        blockTimer -= Time.deltaTime;

        if (blockTimer <= 0f)
        {
            ai.SetBehaviourState(EnemyBehaviourState.Combat);
        }
    }

    public void OnExit()
    {
        Debug.Log("lowered guard.");
    }
}
