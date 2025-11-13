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
        if (ai.Target != null && ai.CheckTargetOnView())
            ai.MoveTowardsTarget();
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

        var eeo = EnemyAttackOrder.Instance;
        if (eeo == null)
        {
            ai.MoveTowardsTarget();
            return;
        }

        float dist = Vector3.Distance(ai.character.transform.position, ai.Target.position);

        if (eeo.IsAttacking(ai))
        {
            if (dist <= ai.attackRange + 0.3f && Time.time >= ai.nextAttackTime)
            {
                ai.EnterCombatMode();
                Debug.Log($"{ai.name} entering ATTACK from alert (zone granted)");
                ai.SetState(ai.GetAttackState());
                return;
            }
            else
            {
                ai.MoveTowardsTarget(); 
                return;
            }
        }

        if (eeo.IsStandby(ai))
        {
            MaintainFormation(eeo.player);
            return;
        }

        ai.MoveTowardsTarget();
    }

    public void OnExit()
    {
        ai.StopMovement();
        Debug.Log($"{ai.name} exited ALERT state.");
    }
    private void MaintainFormation(Transform player)
    {
        if (ai.agent == null || player == null) return;
        float orbitRadius = EnemyAttackOrder.Instance.standbyZoneRadius * 0.9f;

        Vector3 dir = (ai.transform.position - player.position).normalized;
        Vector3 offset = Quaternion.Euler(0f, Time.time * 30f, 0f) * dir * orbitRadius;

        ai.agent.SetDestination(player.position + offset);
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
