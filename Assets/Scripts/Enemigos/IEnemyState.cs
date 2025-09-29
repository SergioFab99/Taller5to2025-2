using KinematicCharacterController;
using UnityEngine;

public interface IEnemyState
{
    void OnEnter();
    void Update();
    void OnExit();

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
    {

    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList default_Settings)
    {

    }


}
