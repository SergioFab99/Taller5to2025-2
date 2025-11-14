using KinematicCharacterController;
using UnityEngine;

public interface IEnemyState
{
    void OnEnter();
    void Update();
    void OnExit();

    public Quaternion UpdateRotation( Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
    {
        return currentRotation;
       
    }

    public Vector3 UpdateVelocity( Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList default_Settings, ref float _timeSinceUngrounded)
    {
        return currentVelocity;
    }


}
