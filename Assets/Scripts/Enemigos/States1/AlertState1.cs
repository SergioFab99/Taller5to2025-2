using KinematicCharacterController;
using UnityEngine;

public class AlertState1 : IEnemyState
{
    private EnemyStateHandler ai;
    private float lostSightTimer;
    private const float maxLostSightTime = 3.0f;

    public AlertState1(EnemyStateHandler main)
    {
        ai = main;
    }

    public void OnEnter()
    {
        ai.EnterCombatMode();
        lostSightTimer = 0f;

        if (ai.agent != null && ai.agent.enabled)
        {
            ai.agent.isStopped = false;
            ai.agent.speed = ai.MoveSpeed();
        }

        Debug.Log($"{ai.name} ALERT");
    }

    public void Update()
    {
        if (ai.TargetPlayer == null)
        {
            ai.SetState(ai.GetIdleState());
            return;
        }

        bool canSeePlayer = ai.CheckTargetOnView();
        float dist = Vector3.Distance(ai.character.transform.position, ai.Target.position);

        if (!canSeePlayer)
        {
            lostSightTimer += Time.deltaTime;
            if (lostSightTimer >= maxLostSightTime)
            {
                ai.ExitCombatMode();
                ai.SetState(ai.GetIdleState());
                return;
            }
        }
        else
        {
            lostSightTimer = 0f;
        }

        if (EnemyAttackOrder.Instance != null)
        {
            if (EnemyAttackOrder.Instance.IsAttacker(ai))
            {
                ai.SetState(ai.GetAttackState());
                return;
            }

            if (dist <= ai.enemySettings.AISettings.attackRange * 1.4f)
                EnemyAttackOrder.Instance.RequestAttack(ai);
        }

        Vector3 dest;
        if (EnemyAttackOrder.Instance != null &&
            EnemyAttackOrder.Instance.TryGetFormationDestination(ai, out dest))
        {
            ai.MoveToPoint(dest); 
        }
        else
        {
            ai.MoveTowardsTarget();  // fallback
        }
    }

    public void OnExit()
    {
        Debug.Log($"{ai.name} exit ALERT");
    }

    public Quaternion UpdateRotation(Quaternion currentRotation, float deltaTime, Vector3 input, KinematicCharacterMotor motor)
    {
        return currentRotation; 
    }

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float dt, KinematicCharacterMotor motor,
        Vector3 input, EnemySettingsList settings, ref float ungrounded)
    {
        return currentVelocity;
    }
}