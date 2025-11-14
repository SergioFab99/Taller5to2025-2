using KinematicCharacterController;
using UnityEngine;
using System;
using System.Collections;


public class AlertState : IEnemyState
{
    private EnemyStateHandler handler;
    private EnemyMain main;
    private float alertTimer;
    private float maxAlertTime = 3f;
    private bool canAttack = true;
    public AlertState(EnemyStateHandler handler)
    {
        this.handler = handler;
    }

    public AlertState(EnemyMain main)
    {
        this.main = main;
    }

    public void OnEnter()
    {
        alertTimer = 0f;
        canAttack = true;
        Debug.Log("spotting");
    }

    public void Update()
    {
        if (handler != null)
        {
            alertTimer += Time.deltaTime;
            if (handler.CheckTargetOnAttackRange(handler.Target))
            {
                Debug.Log("onAttackRange");
                if (canAttack)
                {
                    Debug.Log("trygettingcomponetnAttacking");

                    if (handler.Target.TryGetComponent<HealthController>(out HealthController ht))
                    {
                        Debug.Log("Attacking target");
                        ht.TakeDamage(10);
                        canAttack = false;
                        handler.StartCoroutine(ResetCanAttack(1.0f));
                    }
                    else
                    {
                        Debug.Log("No HealthController found on target.");
                    }

                }

            }

            if (!handler.CheckTargetOnView(handler.Target))
            {

                if (alertTimer >= maxAlertTime)
                {
                    handler.SetState(handler.GetIdleState());
                }

            }

            return;
        }

        if (main == null) return;

        alertTimer += Time.deltaTime;
        if (main.target == null)
        {
            main.SetState(main.GetIdleState());
            return;
        }

        if (!main.Watching())
        {
            if (alertTimer >= maxAlertTime)
            {
                main.SetState(main.GetIdleState());
            }
            return;
        }

        alertTimer = 0f;
        main.MoveTowardsTarget();

        if (main.Attacking())
        {
            main.SetState(main.GetAttackState());
        }

    }

    public Quaternion UpdateRotation(Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
    {
        if (handler == null)
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
        if (handler == null)
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


    public void OnExit()
    {
        Debug.Log("no longer spotting");
    }

    IEnumerator ResetCanAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        canAttack = true;
    }
}
