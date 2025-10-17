using UnityEngine;
using KinematicCharacterController;

public class AttackState1 : IEnemyState
{
    private EnemyStateHandler ai;
    private IEnemyAttack attack;
    private float disengageBuffer = 5.5f;

    public AttackState1(EnemyStateHandler main)
    {
        ai = main;
        attack = ai.GetComponent<IEnemyAttack>();
    }

    public void OnEnter()
    {
        ai.EnterCombatMode(); 
        Debug.Log($"{ai.name} engaging target.");
    }

    public void Update()
    {
        if (ai.Target == null)
        {
            ai.SetState(ai.GetIdleState());
            return;
        }

        float dist = Vector3.Distance(ai.transform.position, ai.Target.position);

        if (attack.IsFinished)
        {
            if (attack.WasInterrupted)
            {
                attack.ResetAttackCycle();
                ai.SetState(ai.GetExposedState());
            }
            else
            {
                attack.ResetAttackCycle();
                ai.SetState(ai.GetRecoverState());
            }
            return;
        }

        if (attack.IsAttacking)
            return;

        if (dist > attack.AttackRange + disengageBuffer)
        {
            ai.ExitCombatMode();
            ai.SetState(ai.GetAlertState());

            if (attack is RangedAttack gun)
                gun.StartReload();
            else if (attack is TommyGunAttack gun2)
                gun2.StartReload();

            return;
        }

        if (dist <= attack.AttackRange)
        {
            attack.Execute();
        }
        else
        {
            Vector3 dir = (ai.Target.position - ai.transform.position).normalized;
            ai.character.UpdateInputs(
                new EnemyInput { Direction = dir, Move = dir },
                ai.GetBehaviourState()
            );
        }
    }

    public void OnExit()
    {
        Debug.Log($"{ai.name} exiting ATTACK state.");
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
