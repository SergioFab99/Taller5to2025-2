using KinematicCharacterController;
using UnityEngine;

public class StunState : IEnemyState
{
    private EnemyStateHandler handler;
    private EnemyMain main;
    private float stunDuration;
    private float stunTimer;
    private Vector3 knockbackDir;

    public StunState(EnemyStateHandler handler)
    {
        this.handler = handler;
    }

    public StunState(EnemyMain main)
    {
        this.main = main;
    }

    public void SetKnockback(Vector3 dir)
    {
        knockbackDir = dir;
    }

    public void OnEnter()
    {
        if (handler != null)
        {
            stunDuration = handler.enemySettings.AISettings.stunDuration;
            stunTimer = 0f;
            Debug.Log("stunned");
            return;
        }

        if (main == null) return;

        stunDuration = main.stunDuration;
        stunTimer = 0f;
        main.StopMovement();
        Debug.Log("stunned");
    }

    public void Update()
    {
        stunTimer += Time.deltaTime;

        if (stunTimer >= stunDuration)
        {
            if (handler != null)
            {
                if (handler.CheckTargetOnView(handler.Target))
                {
                    handler.SetState(handler.GetAlertState());
                }
                else
                {
                    handler.SetState(handler.GetIdleState());
                }
                return;
            }

            if (main == null) return;

            if (main.Watching())
            {
                main.SetState(main.GetAlertState());
            }
            else
            {
                main.SetState(main.GetIdleState());
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

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList Settings)
    {
        if (handler == null)
        {
            return currentVelocity;
        }

        if (handler.knockbackForce >0.1f)
        {
            motor.ForceUnground();
            currentVelocity += (handler.knockbackForce * knockbackDir);
            handler.knockbackForce = 0f;

        }
        return currentVelocity;
        
    }


}

