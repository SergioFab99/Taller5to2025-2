using KinematicCharacterController;
using System.Collections;
using UnityEngine;

public class BlockState1 : IEnemyState
{
    private readonly EnemyStateHandler ai;
    private float blockTimer;
    private const float baseBlockDuration = 2f;
    private const float extensionOnHit = 1f;

    public BlockState1(EnemyStateHandler main) => ai = main;

    public void OnEnter()
    {
        blockTimer = baseBlockDuration;

        if (ai.agent != null && ai.agent.enabled && ai.agent.isOnNavMesh)
        {
            ai.agent.velocity = Vector3.zero;
            ai.agent.isStopped = true;
            ai.agent.ResetPath();
        }

        ai.character.Motor.BaseVelocity = Vector3.zero;
        ai.character.UpdateInputs(new EnemyInput { Move = Vector3.zero, Direction = Vector3.zero }, ai.GetBehaviourState());

        Debug.Log($"{ai.name} raised guard");
    }

    public void Update()
    {
        blockTimer -= Time.deltaTime;

        if (blockTimer <= 0f)
        {
            ai.StartCoroutine(ExitBlockDelay());
        }
    }

    public void ExtendBlock()
    {
        blockTimer += extensionOnHit;
        Debug.Log($"{ai.name} extended block ({blockTimer:F2}s remaining)");
    }

    public void OnExit()
    {
        Debug.Log($"{ai.name} lowered guard.");
    }

    private IEnumerator ExitBlockDelay()
    {
        blockTimer = float.MaxValue;
        yield return new WaitForSeconds(0.2f); 
        if (ai.Target != null && ai.CheckTargetOnView())
            ai.SetState(ai.GetAlertState());
        else
            ai.SetState(ai.GetIdleState());
    }

    public Quaternion UpdateRotation(Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
    {
        if (ai.Target == null) return currentRotation;
        Vector3 toTarget = (ai.Target.position - ai.character.transform.position);
        toTarget.y = 0f;
        if (toTarget.sqrMagnitude < 0.001f) return currentRotation;
        Quaternion desired = Quaternion.LookRotation(toTarget.normalized, motor.CharacterUp);
        return Quaternion.Slerp(currentRotation, desired, deltaTime * 10f);
    }

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor,
        Vector3 _requestedMovement, EnemySettingsList settings, ref float _timeSinceUngrounded)
    {
        return Vector3.zero;
    }
}
