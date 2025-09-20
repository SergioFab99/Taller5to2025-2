using UnityEngine;

public class ExposedState : IEnemyState
{
    private EnemyMain ai;
    private float exposedTimer;
    private float exposedDuration = 1.5f; 

    public ExposedState(EnemyMain main)
    {
        ai = main;
    }

    public void OnEnter()
    {
        exposedTimer = exposedDuration;
        ai.StopMovement();
        Debug.Log("exposed");
    }

    public void Update()
    {
        if (ai.target == null)
        {
            ai.SetState(ai.GetIdleState());
            return;
        }

        exposedTimer -= Time.deltaTime;

        if (exposedTimer <= 0f)
        {
            ai.SetState(ai.GetRecoverState());
        }
    }

    public void OnExit()
    {
        Debug.Log($"{ai.name} recovered from being exposed.");
    }
}
