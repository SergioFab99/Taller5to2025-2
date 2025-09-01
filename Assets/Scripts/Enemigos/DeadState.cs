using UnityEngine;

public class DeadState : IEnemyState
{
    private EnemyMain ai;

    public DeadState(EnemyMain main)
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
