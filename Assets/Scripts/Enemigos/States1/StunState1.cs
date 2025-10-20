using KinematicCharacterController;
using UnityEngine;

public class StunState1 : IEnemyState
{
    private EnemyStateHandler ai;
    private float stunTimer;
    private float stunDuration = 2.0f;
    private Vector3 knockbackDir;

    public StunState1(EnemyStateHandler main)
    {
        ai = main;
    }

    public void SetKnockback(Vector3 dir)
    {
        knockbackDir = dir;
    }

    public void OnEnter()
    {
        stunTimer = 0f;
        ai.ExitCombatMode(); 
        ai.Knockback(knockbackDir);
        Debug.Log($"{ai.name} is stunned!");
    }

    public void Update()
    {
        stunTimer += Time.deltaTime;

        if (stunTimer >= stunDuration)
        {
            if (ai.Target != null && ai.CheckTargetOnView())
                ai.SetState(ai.GetAlertState());
            else
                ai.SetState(ai.GetIdleState());
        }
    }

    public void OnExit()
    {
        Debug.Log($"{ai.name} recovered from stun.");
    }

    public Quaternion UpdateRotation(Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
        => currentRotation;

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList Settings)
        => currentVelocity;
}

