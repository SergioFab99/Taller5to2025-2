using KinematicCharacterController;
using UnityEngine;

public class StunState : IEnemyState
{
    private EnemyStateHandler ai;
    private float stunDuration;
    private float stunTimer;
    private Vector3 knockbackDir;

    public StunState(EnemyStateHandler main)
    {
        ai = main;
    }

    public void SetKnockback(Vector3 dir)
    {
        knockbackDir = dir;
    }

    public void OnEnter()
    {
        stunDuration = ai.enemySettings.AISettings.stunDuration;   
        stunTimer = 0f;

        Debug.Log("stunned");

        
    }

    public void Update()
    {
        stunTimer += Time.deltaTime;

        if (stunTimer >= stunDuration)
        {
            if (ai.CheckTargetOnView(ai.Target))
            {
                ai.SetState(ai.GetAlertState());
            }
            else
            {
                ai.SetState(ai.GetIdleState());
            }
                
        }
    }

    public void OnExit()
    {
        Debug.Log("recovered from stun");
    }

    public Quaternion UpdateRotation(Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
    {
        return currentRotation;
    }

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList Settings, ref float _timeSinceUngrounded)
    {
        return currentVelocity;
    }

}

