using UnityEngine;

public class IdleState : IEnemyState
{
    private EnemyMain ai;

    public IdleState(EnemyMain main)
    {
        ai = main;
    }

    public void OnEnter()
    {
        Debug.Log("is now idle");
    }

    public void Update()
    {
        if (ai.Watching())
        {
            ai.SetState(ai.GetAlertState());
        }
    }

    public void OnExit()
    {
        Debug.Log("no longer idle lmao");
    }
}
