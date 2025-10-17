using KinematicCharacterController;
using UnityEngine;

public class BlockState1 : IEnemyState
{
    private EnemyStateHandler ai;
    private float blockTimer;
    private float blockDuration = 1.0f;  

    public BlockState1(EnemyStateHandler main)
    {
        ai = main;
    }

    public void OnEnter()
    {
        blockTimer = blockDuration;


        Debug.Log($"is blocking");
    }

    public void Update()
    {
        if (ai.Target != null)
        {
            ai.SetState(ai.GetAlertState());
            return;
        }

        blockTimer -= Time.deltaTime;

        if (blockTimer <= 0f)
        {
        }
    }

    public void OnExit()
    {
        Debug.Log("lowered guard.");
    }

    public Quaternion UpdateRotation( Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
    {
        return currentRotation;
    }

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList Settings, ref float _timeSinceUngrounded)
    {
        return currentVelocity;
    }

}
