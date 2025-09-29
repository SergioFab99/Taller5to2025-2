using KinematicCharacterController;
using UnityEngine;

public class IdleState : IEnemyState
{
    private EnemyStateHandler ai;

    public IdleState(EnemyStateHandler main)
    {
        ai = main;
    }

    public void OnEnter()
    {
        Debug.Log("is now idle");
    }

    public void Update()
    {
        if (ai.CheckTargetOnView())
        {
            ai.SetState(ai.GetAlertState());
        }
    }

    public void OnExit()
    {
        Debug.Log("no longer idle lmao");
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
    {

    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList default_Settings)
    {

    }


}
