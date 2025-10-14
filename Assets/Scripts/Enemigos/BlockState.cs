using KinematicCharacterController;
using UnityEngine;

public class BlockState : IEnemyState
{
    private EnemyStateHandler handler;
    private EnemyMain main;
    private float blockTimer;
    private float blockDuration = 1.0f;  

    public BlockState(EnemyStateHandler handler)
    {
        this.handler = handler;
    }

    public BlockState(EnemyMain main)
    {
        this.main = main;
    }

    public void OnEnter()
    {
        blockTimer = blockDuration;


        Debug.Log($"is blocking");
    }

    public void Update()
    {
        if (handler != null)
        {
            if (handler.Target == null)
            {
                handler.SetState(handler.GetAlertState());
                return;
            }

            blockTimer -= Time.deltaTime;

            if (blockTimer <= 0f)
            {
                handler.SetBehaviourState(EnemyBehaviourState.Default);
            }
            return;
        }

        if (main == null)
        {
            return;
        }

        if (main.target == null)
        {
            main.SetState(main.GetIdleState());
            return;
        }

        blockTimer -= Time.deltaTime;

        if (blockTimer <= 0f)
        {
            main.SetState(main.GetRecoverState());
        }
    }

    public void OnExit()
    {
        Debug.Log("lowered guard.");
    }

    public Quaternion UpdateRotation( Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
    {
        if (handler == null)
        {
            return currentRotation;
        }
        return currentRotation;
    }

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList Settings)
    {
        if (handler == null)
        {
            return currentVelocity;
        }
        return currentVelocity;
    }

}
