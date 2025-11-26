using UnityEngine;
using KinematicCharacterController;

public class RecoverState1 : IEnemyState
{
    private EnemyStateHandler ai;

    private float recoverTimer;
    private float recoverDuration = 0.8f;

    private float retreatTimer = 0.45f;
    private Vector3 retreatDir;

    private const float attackCooldown = 1.5f; // fallback 

    public RecoverState1(EnemyStateHandler handler)
    {
        ai = handler;
    }

    public void OnEnter()
    {
        recoverTimer = recoverDuration;

        EnemyAttackOrder.Instance.NotifyAttackFinished(ai);

        ai.attackTagCooldownEndTime = Time.time + attackCooldown;

        if (ai.Target != null)
        {
            retreatDir = (ai.character.transform.position - ai.Target.position).normalized;
            retreatDir.y = 0f;
        }

        if (ai.agent != null && ai.agent.enabled && ai.agent.isOnNavMesh)
        {
            ai.agent.velocity = Vector3.zero;
            ai.agent.isStopped = true;
            ai.agent.ResetPath();
        }

        ai.character.Motor.BaseVelocity = Vector3.zero;

        Debug.Log($"{ai.name} entered RECOVER — retreating");
    }

    public void Update()
    {

        recoverTimer -= Time.deltaTime;

        if (retreatTimer > 0f)
        {
            retreatTimer -= Time.deltaTime;

            ai.character.UpdateInputs(
                new EnemyInput
                {
                    Move = retreatDir * 1.1f,
                    Direction = retreatDir
                },
                ai.GetBehaviourState()
            );

            return;
        }

        if (recoverTimer <= 0f)
        {
            ai.SetState(ai.GetAlertState());
        }
    }


    public void OnExit()
    {
        Debug.Log($"{ai.name} exited RECOVER.");
    }

    public Quaternion UpdateRotation(Quaternion currentRotation, float deltaTime, Vector3 req, KinematicCharacterMotor motor)
        => currentRotation;

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor,
        Vector3 move, EnemySettingsList settings, ref float u)
        => currentVelocity;
}