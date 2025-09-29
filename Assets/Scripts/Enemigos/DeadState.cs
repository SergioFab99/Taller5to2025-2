using UnityEngine;

public class DeadState : IEnemyState
{
    private EnemyStateHandler ai;

    public DeadState(EnemyStateHandler main)
    {
        ai = main;
    }

    public void OnEnter()
    {
        Debug.Log("dead lol");
    }

    public void Update()
    {
    }

    public void OnExit()
    {
    }
}
