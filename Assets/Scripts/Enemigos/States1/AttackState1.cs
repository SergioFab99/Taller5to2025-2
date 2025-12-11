using UnityEngine;
using KinematicCharacterController;

public class AttackState1 : IEnemyState
{
    private EnemyStateHandler ai;
    private IEnemyAttack attackMode;
    private float disengageBuffer = 5.5f;

    public AttackState1(EnemyStateHandler main)
    {
        ai = main;
    }

    public void OnEnter()
    {
        ai.EnterCombatMode();
        ai.character.SetMovementMode(MovementMode.KCC);
        ai.StopMovement(); 

        if (attackMode == null)
            attackMode = ai.GetComponentInChildren<IEnemyAttack>();

        if (attackMode == null)
        {
            Debug.LogError($"{ai.name} ATTACK STATE: No IEnemyAttack found");
            ai.SetState(ai.GetAlertState());
            return;
        }

        attackMode.ResetAttackCycle();
        attackMode.BeginAttack(ai);

        Debug.Log($"{ai.name} ATTACK ENTER mode={attackMode.GetType().Name}");
    }

    public void Update()
    {
        if (attackMode == null)
        {
            ai.SetState(ai.GetAlertState());
            return;
        }

        attackMode.ManualUpdate();

        if (ai.Target == null)
        {
            ai.SetState(ai.GetIdleState());
            return;
        }

        float dist = Vector3.Distance(ai.character.transform.position, ai.Target.position);

        if (attackMode.IsFinished)
        {
            if (EnemyAttackOrder.Instance != null)
                EnemyAttackOrder.Instance.NotifyAttackFinished(ai);

            if (attackMode.ForceBlocked)
                ai.SetState(ai.GetBlockState());
            else if (attackMode.WasInterrupted)
                ai.SetState(ai.GetStunState());
            else if (attackMode.Missed)
                ai.SetState(ai.GetExposedState());
            else
                ai.SetState(ai.GetRecoverState());

            attackMode.ResetAttackCycle();
            return;
        }

        if (attackMode.IsAttacking)
            return;

        if (dist > attackMode.AttackRange + disengageBuffer)
        {
            ai.SetState(ai.GetAlertState());
            return;
        }

        if (dist <= attackMode.AttackRange + 0.8f)
        {
            attackMode.Execute();
            return;
        }

        Vector3 dir = (ai.Target.position - ai.character.transform.position).normalized;
        dir.y = 0f;

        ai.character.UpdateInputs(
            new EnemyInput
            {
                Direction = dir,
                Move = dir
            },
            ai.GetBehaviourState()
        );
    }

    public void OnExit()
    {
        Debug.Log($"{ai.name} ATTACK EXIT");
    }

    public Quaternion UpdateRotation(Quaternion currentRotation, float dt, Vector3 _requestedRotation, KinematicCharacterMotor motor)
    {
        if (ai.Target == null) return currentRotation;

        Vector3 toTarget = (ai.Target.position - ai.character.transform.position);
        toTarget.y = 0;
        if (toTarget.sqrMagnitude < 0.001f) return currentRotation;

        Vector3 facing = toTarget.normalized;

        if (ai.HasStatus(StatusEffect.Drunk))
            facing = Quaternion.Euler(ai.drunkRotationOffset) * facing;

        Quaternion desired = Quaternion.LookRotation(facing, motor.CharacterUp);
        return Quaternion.Slerp(currentRotation, desired, dt * 10f);
    }

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float dt, KinematicCharacterMotor motor, Vector3 input, EnemySettingsList settings, ref float ungrounded)
    {
        var style = ai.enemySettings.AISettings.attackStyle;

        if (style == AttackStyle.Melee || style == AttackStyle.Thrower)
        {
            if (attackMode != null && attackMode.IsAttacking)
                return Vector3.zero;

            return RangedMovement(currentVelocity, dt, motor, input, settings, ref ungrounded);
        }

        if (style == AttackStyle.Ranged)
        {
            return RangedMovement(currentVelocity, dt, motor, input, settings, ref ungrounded);
        }

        return currentVelocity;
    }

    private Vector3 RangedMovement(Vector3 vel, float dt, KinematicCharacterMotor motor, Vector3 input, EnemySettingsList settings, ref float ungrounded)
    {
        if (motor.GroundingStatus.IsStableOnGround)
        {
            var groundedMovement = motor.GetDirectionTangentToSurface(input, motor.GroundingStatus.GroundNormal) * input.magnitude;

            vel = groundedMovement * settings.AlertEnemySettings.moveSettings.Speed;
            ungrounded = 0f;
        }
        else
        {
            ungrounded += dt;

            if (input.sqrMagnitude > 0f)
            {
                var planar = Vector3.ProjectOnPlane(input, motor.CharacterUp) * input.magnitude;
                var planarVel = Vector3.ProjectOnPlane(vel, motor.CharacterUp);

                var acc = planar * settings.AlertEnemySettings.airSettings.AirAcceleration * dt;

                if (planarVel.magnitude < settings.AlertEnemySettings.airSettings.AirSpeed)
                {
                    var target = planarVel + acc;
                    target = Vector3.ClampMagnitude(target, settings.AlertEnemySettings.airSettings.AirSpeed);
                    acc = target - planarVel;
                }

                if (motor.GroundingStatus.FoundAnyGround)
                {
                    if (Vector3.Dot(acc, vel + acc) > 0f)
                    {
                        Vector3 obstructNormal = Vector3.Cross(
                            motor.CharacterUp,
                            Vector3.Cross(motor.CharacterUp, motor.GroundingStatus.GroundNormal)
                        ).normalized;
                        acc = Vector3.ProjectOnPlane(acc, obstructNormal);
                    }
                }

                vel += acc;
            }

            vel += motor.CharacterUp * settings.AlertEnemySettings.airSettings.Gravity * dt;
        }

        return vel;
    }
}