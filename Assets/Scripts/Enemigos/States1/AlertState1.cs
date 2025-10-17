using KinematicCharacterController;
using UnityEngine;
using System;
using System.Collections;


public class AlertState1 : IEnemyState
{
    private EnemyStateHandler ai;
    private float alertTimer;
    private float maxAlertTime = 3f;

    public AlertState1(EnemyStateHandler main)
    {
        ai = main;
    }

    public void OnEnter()
    {
        alertTimer = 0f;
        ai.ExitCombatMode(); 
        Debug.Log($"{ai.name} entered ALERT state.");
    }

    public void Update()
    {
        alertTimer += Time.deltaTime;

        if (ai.Target == null || !ai.CheckTargetOnView())
        {
            if (alertTimer >= maxAlertTime)
            {
                ai.SetState(ai.GetIdleState());
            }
            return;
        }

        ai.MoveTowardsTarget();

        if (ai.CheckTargetOnAttackRange())
        {
            ai.EnterCombatMode(); 
            ai.SetState(ai.GetAttackState());
        }
    }

    public void OnExit()
    {
        ai.StopMovement();
        Debug.Log($"{ai.name} exited ALERT state.");
    }
}
