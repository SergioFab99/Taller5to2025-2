using UnityEngine;
using KinematicCharacterController;

public class AttackState1 : IEnemyState
{
    private EnemyStateHandler ai;
    public IEnemyAttack attackMode;
    private float disengageBuffer = 5.5f;

    public AttackState1(EnemyStateHandler main)
    {
        ai = main;
        attackMode = ai.GetComponentInParent<IEnemyAttack>();
    }

    public void OnEnter()
    {
        ai.EnterCombatMode();
        Debug.Log($"{ai.name}, {attackMode} engaging");
    }

    public void Update()
    {
        if (ai.Target == null)
        {
            ai.SetState(ai.GetIdleState());
            return;
        }

        float dist = Vector3.Distance(ai.character.transform.position, ai.Target.position);

        if (attackMode.IsFinished)
        {
            ai.QueuedBlock = false;
            if(attackMode.ForceBlocked)
            {
                ai.SetState(ai.GetBlockState());
            }
            else if (attackMode.WasInterrupted)
            {
                ai.SetState(ai.GetStunState());
            }
            else if (attackMode.Missed == true)
            {
                ai.SetState(ai.GetExposedState());
            }
            else
            {
                ai.SetState(ai.GetRecoverState());
            }
            attackMode.ResetAttackCycle();
            return;
        }

        if (attackMode.IsAttacking)
            return;

        if (dist > attackMode.AttackRange + disengageBuffer)
        {
            ai.ExitCombatMode();
            ai.SetState(ai.GetAlertState());

            if (attackMode is RangedAttack gun)
                gun.StartReload();
            else if (attackMode is TommyGunAttack gun2)
                gun2.StartReload();

            return;
        }

        if (dist <= attackMode.AttackRange + 0.5f)
        {
            attackMode.Execute();
        }
        else
        {
            Vector3 dir = (ai.Target.position - ai.character.transform.position).normalized;
            ai.character.UpdateInputs(
                new EnemyInput { Direction = dir, Move = dir },
                ai.GetBehaviourState()
            );
        }
    }

    public void OnExit()
    {
        if(attackMode is MonoBehaviour mb)
        {
            mb.CancelInvoke();
        }
        Debug.Log($"{ai.name} exiting attack state.");
    }

    public Quaternion UpdateRotation(Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
    {
        if (ai.Target == null) return currentRotation;

        Vector3 toTarget = (ai.Target.position - ai.character.transform.position);
        toTarget.y = 0f;
        if (toTarget.sqrMagnitude < 0.001f) return currentRotation;

        Vector3 facing = toTarget.normalized;

        if (ai.HasStatus(StatusEffect.Drunk))
        {
            facing = Quaternion.Euler(ai.drunkRotationOffset) * facing;
        }

        Quaternion desired = Quaternion.LookRotation(facing, motor.CharacterUp);
        return Quaternion.Slerp(currentRotation, desired, deltaTime * 10f);
    }

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList Settings, ref float _timeSinceUngrounded)
    {        
        if (ai.enemySettings.AISettings.attackStyle == AttackStyle.Melee || ai.enemySettings.AISettings.attackStyle == AttackStyle.Thrower)
            return Vector3.zero;

        if (ai.enemySettings.AISettings.attackStyle == AttackStyle.Ranged)
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
        }

        return currentVelocity;
    }
}
