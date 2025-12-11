using KinematicCharacterController;
using System.Collections;
using UnityEngine;

public class BlockState1 : IEnemyState
{
    private readonly EnemyStateHandler ai;
    private float blockTimer;
    private const float baseBlockDuration = 2f;
    private const float extensionOnHit = 1f;

    private bool exiting = false;

    public BlockState1(EnemyStateHandler main) => ai = main;

    public void OnEnter()
    {
        exiting = false;
        blockTimer = baseBlockDuration;

        if (ai.agent != null && ai.agent.enabled && ai.agent.isOnNavMesh)
        {
            ai.agent.velocity = Vector3.zero;
            ai.agent.isStopped = true;
            ai.agent.ResetPath();
        }

        ai.character.Motor.BaseVelocity = Vector3.zero;
        ai.character.UpdateInputs(new EnemyInput {Move = Vector3.zero, Direction = Vector3.zero}, ai.GetBehaviourState());

        Debug.Log($"{ai.name} raised guard");
    }

    public void Update()
    {
        if (exiting) return;

        blockTimer -= Time.deltaTime;

        if (blockTimer <= 0f)
        {
            exiting = true;
            ai.StartCoroutine(ExitBlockDelay());
        }
    }

    public void ExtendBlock()
    {
        if (!exiting)
            blockTimer += extensionOnHit;

        Debug.Log($"{ai.name} extended block ({blockTimer:F2}s remaining)");
    }

    public void OnExit()
    {
        exiting = false;
        Debug.Log($"{ai.name} lowered guard.");
    }

    private IEnumerator ExitBlockDelay()
    {
        yield return new WaitForSeconds(0.15f);

        if (ai.Target == null || !ai.CheckTargetOnView())
        {
            ai.SetState(ai.GetIdleState());
            yield break;
        }

        ai.SetState(ai.GetAlertState());
    }

    public Quaternion UpdateRotation(Quaternion currentRotation, float deltaTime, Vector3 _reqRot, KinematicCharacterMotor motor)
    {
        if (ai.Target == null) return currentRotation;

        Vector3 toPlayer = ai.Target.position - ai.character.transform.position;
        toPlayer.y = 0;

        if (toPlayer.sqrMagnitude < 0.01f)
            return currentRotation;

        Quaternion desired = Quaternion.LookRotation(toPlayer.normalized, motor.CharacterUp);

        return Quaternion.Slerp(currentRotation, desired, deltaTime * 3f);
    }

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor,
        Vector3 _requestedMovement, EnemySettingsList settings, ref float _ungrounded)
    {
        return Vector3.zero;
    }
}