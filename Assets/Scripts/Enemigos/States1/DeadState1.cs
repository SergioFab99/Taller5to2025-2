using KinematicCharacterController;
using UnityEngine;

public class DeadState1 : IEnemyState
{
    private EnemyStateHandler ai;

    public DeadState1(EnemyStateHandler main)
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

    public Quaternion UpdateRotation(Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
    {
        return Quaternion.identity;
    }

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList Settings)
    {
        return currentVelocity;
    }

}
