using UnityEngine;

public class AttackState : IEnemyState
{
    private EnemyMain ai;

    public AttackState(EnemyMain main)
    {
        ai = main;
    }

    public void OnEnter()
    {
        Debug.Log("attacking");
    }

    public void Update()
    {
        if (ai.target == null)
        {
            ai.SetState(ai.GetIdleState());
            return;
        }

       
        Vector3 dir = (ai.target.position - ai.transform.position).normalized;
        ai.transform.position += dir * ai.moveSpeed * Time.deltaTime;

        if (!ai.Watching())
        {
            ai.SetState(ai.GetAlertState());
            return;
        }

        if (ai.Attacking())
        {
            Debug.Log("pew pew or smth");
        }
    }

    public void OnExit()
    {
        Debug.Log("disengaging");
    }
}
