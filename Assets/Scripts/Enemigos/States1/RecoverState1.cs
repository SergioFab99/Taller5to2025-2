using UnityEngine;
using KinematicCharacterController;
using System.Linq;

public class RecoverState1 : IEnemyState
{
    private EnemyStateHandler ai;

    private float recoverTimer;
    private const float recoverDuration = 0.8f;

    private float retreatTimer;
    private const float retreatDuration = 0.45f;

    private bool shouldRetreat = false;

    public RecoverState1(EnemyStateHandler handler)
    {
        ai = handler;
    }

    public void OnEnter()
    {
        recoverTimer = recoverDuration;
        retreatTimer = retreatDuration;

        shouldRetreat = DetermineIfRetreatIsSafe();

        if (ai.agent != null && ai.agent.enabled && ai.agent.isOnNavMesh)
        {
            ai.agent.velocity = Vector3.zero;
            ai.agent.isStopped = true;
            ai.agent.ResetPath();
        }

        ai.character.Motor.BaseVelocity = Vector3.zero;

        Debug.Log($"{ai.name} RECOVER (retreat = {shouldRetreat})");
    }

    private bool DetermineIfRetreatIsSafe()
    {
        var order = EnemyAttackOrder.Instance;
        if (order == null) return true; 

        var frontliners =
            order.formation
            .Where(kv => kv.Value.ringIndex == 0 &&
                         kv.Value.inFrontArc &&
                         kv.Key != ai &&
                         kv.Key != null &&
                         !order.IsAttacker(kv.Key))
            .ToList();

        return frontliners.Count > 0;
    }

    public void Update()
    {
        recoverTimer -= Time.deltaTime;

        if (shouldRetreat && retreatTimer > 0f)
        {
            retreatTimer -= Time.deltaTime;

            Vector3 dir = ai.GetSafeRetreatDirection();
            ai.character.Motor.BaseVelocity = dir * 2.2f;

            return;
        }

        ai.character.Motor.BaseVelocity = Vector3.zero;

        if (recoverTimer <= 0f)
        {
            ai.SetState(ai.GetAlertState());
        }
    }

    public void OnExit()
    {
        ai.character.Motor.BaseVelocity = Vector3.zero;
        Debug.Log($"{ai.name} exited RECOVER.");
    }

    public Quaternion UpdateRotation(Quaternion currentRotation, float deltaTime, Vector3 req, KinematicCharacterMotor motor)
        => currentRotation;

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor,
        Vector3 move, EnemySettingsList settings, ref float u)
        => currentVelocity;
}