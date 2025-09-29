using UnityEngine;

public class ExposedState : IEnemyState
{
    private EnemyStateHandler ai;
    private float exposedTimer;
    private float exposedDuration = 1.5f; 

    public ExposedState(EnemyStateHandler main)
    {
        ai = main;
    }

    public void OnEnter()
    {
        exposedTimer = exposedDuration;
        Debug.Log("exposed");
    }

    public void Update()
    {
        if (ai.Target == null)
        {
            ai.SetState(ai.GetIdleState());
            return;
        }

        exposedTimer -= Time.deltaTime;

        if (exposedTimer <= 0f)
        {
            ai.SetBehaviourState(EnemyBehaviourState.Combat);
        }
    }

    public void OnExit()
    {
        Debug.Log($"{ai.name} recovered from being exposed.");
    }
}
