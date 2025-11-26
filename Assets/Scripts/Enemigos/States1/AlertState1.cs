using KinematicCharacterController;
using UnityEngine;

public class AlertState1 : IEnemyState
{
    private EnemyStateHandler ai;
    private float alertTimer;
    private const float maxAlertTime = 3f;

    private float frontDistance = 6f;
    private float rearDistance = 8f;
    private float lateralAmplitudeFront = 1.5f;
    private float lateralAmplitudeRear = 2.0f;
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

        if (ai.Target != null && ai.CheckTargetOnView())
            ai.MoveTowardsTarget();
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
            ai.MoveTowardsTarget();
            return;
        }

        float dist = Vector3.Distance(ai.character.transform.position, ai.Target.position);

        if (coord.IsAttacker(ai))
        {
            TryEnterAttack(dist);
            return;
        }

        if (coord.IsFront(ai))
        {
            FrontFormationMove(coord.player);
            return;
        }

        if (coord.IsRear(ai))
        {
            RearFormationMove(coord.player);
            return;
        }

        FrontFormationMove(coord.player); //fallback
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
            ai.MoveTowardsTarget();
        }
    }
    private void FrontFormationMove(Transform player)
    {
        if (ai.agent == null || player == null) return;

        if (Time.time < nextRepositionTime)
            return;

        nextRepositionTime = Time.time + repositionCooldown;

        Vector3 toEnemy = ai.transform.position - player.position;
        if (toEnemy.sqrMagnitude < 0.01f)
            toEnemy = -player.forward;

        Vector3 forwardFromPlayer = -toEnemy.normalized; 
        Vector3 tangent = Vector3.Cross(Vector3.up, forwardFromPlayer).normalized;

        float lateral = Mathf.Sin(Time.time * 0.8f + phaseOffset) * lateralAmplitudeFront;

        Vector3 targetPos =
            player.position +
            forwardFromPlayer * frontDistance +
            tangent * lateral;

        ai.agent.SetDestination(targetPos);
    }

    private void RearFormationMove(Transform player)
    {
        if (ai.agent == null || player == null) return;

        if (Time.time < nextRepositionTime)
            return;

        nextRepositionTime = Time.time + repositionCooldown;

        Vector3 backDir = -player.forward;
        Vector3 tangent = Vector3.Cross(Vector3.up, backDir).normalized;

        float lateral = Mathf.Sin(Time.time * 0.6f + phaseOffset) * lateralAmplitudeRear;

        Vector3 targetPos =
            player.position +
            backDir.normalized * rearDistance +
            tangent * lateral;

        ai.agent.SetDestination(targetPos);
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