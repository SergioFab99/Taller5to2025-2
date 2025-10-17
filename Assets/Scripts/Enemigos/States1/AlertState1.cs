using KinematicCharacterController;
using UnityEngine;
using System;
using System.Collections;


public class AlertState1 : IEnemyState
{
    private EnemyStateHandler ai;
    private float alertTimer;
    private float maxAlertTime = 3f;

    public AlertState1(EnemyStateHandler main)
    {
        ai = main;
    }

    public void OnEnter()
    {
        alertTimer = 0f;
        ai.ExitCombatMode(); 
        Debug.Log($"{ai.name} entered ALERT state.");
    }

    public void Update()
    {
        alertTimer += Time.deltaTime;

        if (ai.Target == null || !ai.CheckTargetOnView())
        {
            if (alertTimer >= maxAlertTime)
            {
                ai.SetState(ai.GetIdleState());
            }
            return;
        }

        ai.MoveTowardsTarget();

        if (ai.CheckTargetOnAttackRange())
        {
            ai.EnterCombatMode(); 
            ai.SetState(ai.GetAttackState());
        }
    }

    public void OnExit()
    {
        ai.StopMovement();
        Debug.Log($"{ai.name} exited ALERT state.");
    }

    public Quaternion UpdateRotation(Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
    {
        if (ai == null)
        {
            return currentRotation;
        }

        var forward = Vector3.ProjectOnPlane(
                                      _requestedRotation,
                                      motor.CharacterUp);

        return currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
    }

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList Settings, ref float _timeSinceUngrounded)
    {
        if (ai == null)
        {
            return currentVelocity;
        }

        if (motor.GroundingStatus.IsStableOnGround)
        {

            var groundedMovement = motor.GetDirectionTangentToSurface
            (
                direction: _requestedMovement,
                surfaceNormal: motor.GroundingStatus.GroundNormal
            ) * _requestedMovement.magnitude;

            currentVelocity = groundedMovement * Settings.AlertEnemySettings.moveSettings.Speed;

            _timeSinceUngrounded = 0f;
        }

        else
        {
            _timeSinceUngrounded += deltaTime;
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

                var movementForce = planarMovement * Settings.AlertEnemySettings.airSettings.AirAcceleration * deltaTime;

                if (currentPlanarVelocity.magnitude < Settings.AlertEnemySettings.airSettings.AirSpeed)
                {
                    var targetPlanarVelocity = currentPlanarVelocity + movementForce;

                    targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, Settings.AlertEnemySettings.airSettings.AirSpeed);
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

        currentVelocity += motor.CharacterUp * Settings.AlertEnemySettings.airSettings.Gravity * deltaTime;

        }
        //Debug.Log("Alert");
        return currentVelocity;
    }
}
