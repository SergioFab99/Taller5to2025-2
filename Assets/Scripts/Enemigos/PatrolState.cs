using KinematicCharacterController;
using UnityEngine;

public class PatrolState : IEnemyState
{
    public void OnEnter()
    {
        Debug.Log("Patroling");
    }

    public void OnExit()
    {
     
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime,Vector3 _requestedRotation, KinematicCharacterMotor motor)
    {
        var forward = Vector3.ProjectOnPlane(
                                      _requestedRotation,
                                      motor.CharacterUp);

        currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, EnemySettingsList Settings, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement)
    {

        if (motor.GroundingStatus.IsStableOnGround)
        {

            var groundedMovement = motor.GetDirectionTangentToSurface
            (
                direction: _requestedMovement,
                surfaceNormal: motor.GroundingStatus.GroundNormal
            ) * _requestedMovement.magnitude;

            currentVelocity = groundedMovement * Settings.DefaultEnemySettings.moveSettings.Speed;


        }
        else
        {
            if (_requestedMovement.sqrMagnitude > 0f)
            {
                var planarMovement = Vector3.ProjectOnPlane
                (
                    vector: _requestedMovement,
                    planeNormal: motor.CharacterUp
                ) * _requestedMovement.magnitude;

                var currentPlanarVelocity = Vector3.ProjectOnPlane
                (
                    vector: currentVelocity,
                    planeNormal: motor.CharacterUp
                );

                var movementForce = planarMovement * Settings.DefaultEnemySettings.airSettings.AirAcceleration * deltaTime;

                if (currentPlanarVelocity.magnitude < Settings.DefaultEnemySettings.airSettings.AirSpeed)
                {
                    var targetPlanarVelocity = currentPlanarVelocity + movementForce;

                    targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, Settings.DefaultEnemySettings.airSettings.AirSpeed);
                    movementForce = targetPlanarVelocity - currentPlanarVelocity;
                }

                else if (Vector3.Dot(currentPlanarVelocity, movementForce) > 0f)
                {
                    var contrainedMovementForce = Vector3.ProjectOnPlane
                    (
                        vector: movementForce,
                        planeNormal: currentPlanarVelocity.normalized
                    );
                    movementForce = contrainedMovementForce;

                }

                if (motor.GroundingStatus.FoundAnyGround) // prevent wall climbing in the air
                {
                    if (Vector3.Dot(movementForce, currentVelocity + movementForce) > 0f)
                    {
                        var obstructedNormal = Vector3.Cross
                        (
                            motor.CharacterUp,
                            Vector3.Cross
                            (
                                motor.CharacterUp,
                                motor.GroundingStatus.GroundNormal
                            )
                        ).normalized;
                        movementForce = Vector3.ProjectOnPlane(movementForce, obstructedNormal);
                    }
                }


                currentVelocity += movementForce;
            }


            currentVelocity += motor.CharacterUp * Settings.DefaultEnemySettings.airSettings.Gravity * deltaTime;

        }

    }


    void IEnemyState.Update()
    {
       
    }
}
