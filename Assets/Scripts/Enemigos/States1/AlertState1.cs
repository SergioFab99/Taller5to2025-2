using KinematicCharacterController;
using UnityEngine;

public class AlertState1 : IEnemyState
{
    private EnemyStateHandler ai;
    private float alertTimer;
    private const float maxAlertTime = 3f;
    private float repositionCooldown = 0.5f;
    private float nextRepositionTime = 0f;
    private float phaseOffset;

    public AlertState1(EnemyStateHandler main)
    {
        ai = main;
        phaseOffset = (Mathf.Abs(main.GetHashCode()) % 1000) * 0.01f;
    }

    public void OnEnter()
    {
        alertTimer = 0f;
        ai.ExitCombatMode();
        Debug.Log($"{ai.name} entered ALERT.");

        if (ai.Target != null && ai.character != null)
        {
            Vector3 direction = (ai.Target.position - ai.character.transform.position).normalized;
            direction.y = 0f;

            Vector3 moveInput = direction * 0.1f;

            var enemyInput = new EnemyInput
            {
                Direction = direction,
                Move = moveInput
            };
            
            ai.character.UpdateInputs(enemyInput, ai.GetBehaviourState());
            ai.character._state.Velocity = moveInput;
            ai.character._state.MovementState = MovementState.Moving;
        }
    }

    public void Update()
    {
        alertTimer += Time.deltaTime;

        if (ai.Target == null || !ai.CheckTargetOnView())
        {
            if (alertTimer >= maxAlertTime)
                ai.SetState(ai.GetIdleState());
            return;
        }

        var coord = EnemyAttackOrder.Instance;
        if (coord == null)
        {
            return;
        }

        float dist = Vector3.Distance(ai.character.transform.position, ai.Target.position);

        if (coord.IsAttacker(ai))
        {
            TryEnterAttack(dist);
            return;
        }

        if (ai.agent == null || !ai.agent.enabled || !ai.agent.isOnNavMesh)
            return;

        Vector3 direction = (ai.Target.position - ai.character.transform.position).normalized;
        direction.y = 0f;

        float stopDistance = ai.enemySettings.AISettings.stopingDistance;
        Vector3 moveInput = (dist > stopDistance + 0.5f) ? direction : Vector3.zero;

        var enemyInput = new EnemyInput
        {
            Direction = direction,
            Move = moveInput
        };
        ai.character.UpdateInputs(enemyInput, ai.GetBehaviourState());
        ai.character._state.Velocity = moveInput;
        ai.character._state.MovementState = moveInput.magnitude > 0.01f ? MovementState.Moving : MovementState.Idle;

        if (coord.TryGetFormationDestination(ai, out Vector3 dest))
        {
            if (Time.time >= nextRepositionTime)
            {
                nextRepositionTime = Time.time + repositionCooldown;

                ai.agent.isStopped = false;
                ai.agent.speed = ai.MoveSpeed();
                ai.agent.SetDestination(dest);
            }
        }
        else
        {
            ai.MoveTowardsTarget();
        }
    }

    public void OnExit()
    {
        ai.StopMovement();
        Debug.Log($"{ai.name} exited ALERT.");
    }

    private void TryEnterAttack(float dist)
    {
        var eo = EnemyAttackOrder.Instance;
        if (eo == null || !eo.IsAttacker(ai))
            return;

        if (dist <= ai.attackRange + 0.35f && Time.time >= ai.nextAttackTime)
        {
            Debug.Log($"{ai.name} -> ATTACK (slot granted, dist={dist:F2})");
            ai.EnterCombatMode();
            ai.SetState(ai.GetAttackState());
        }
        else
        {
            Vector3 forward = (ai.Target.position - ai.character.transform.position).normalized;
            Vector3 newDest = ai.character.transform.position + forward * 0.6f;

            ai.agent.SetDestination(newDest);
        }
    }

    public Quaternion UpdateRotation(Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
    {
        if (ai.Target == null) return currentRotation;

        Vector3 toTarget = ai.Target.position - ai.character.transform.position;
        toTarget.y = 0f;
        if (toTarget.sqrMagnitude < 0.001f) return currentRotation;

        Quaternion desired = Quaternion.LookRotation(toTarget.normalized, motor.CharacterUp);
        return Quaternion.Slerp(currentRotation, desired, deltaTime * 10f);
    }

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList Settings, ref float _timeSinceUngrounded)
    {
        return currentVelocity;
    }
}