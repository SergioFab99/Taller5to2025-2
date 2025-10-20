using KinematicCharacterController;
using UnityEngine;

public class ExposedState1 : IEnemyState
{
    private EnemyStateHandler ai;
    private float timer;
    private const float duration = 1.5f;

    public ExposedState1(EnemyStateHandler main)
    {
        ai = main;
    }

    public void OnEnter()
    {
        timer = duration;
        ai.ExitCombatMode();

        if (ai.agent != null && ai.agent.enabled && ai.agent.isOnNavMesh)
        {
            ai.agent.velocity = Vector3.zero;
            ai.agent.isStopped = true;
            ai.agent.ResetPath();
        }

        ai.character.Motor.BaseVelocity = Vector3.zero;
        ai.character.UpdateInputs(new EnemyInput { Move = Vector3.zero, Direction = Vector3.zero }, ai.GetBehaviourState());

        Debug.Log($"{ai.name} is EXPOSED!");
    }

    public void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        if (ai.Target != null && ai.CheckTargetOnView())
            ai.SetState(ai.GetAlertState());
        else
            ai.SetState(ai.GetIdleState());
    }

    public void OnExit()
    {
        Debug.Log($"{ai.name} recovered from exposure.");
    }

    public Quaternion UpdateRotation(Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
        => currentRotation;

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList Settings, ref float _timeSinceUngrounded)
    {
        return currentVelocity;
    }
}
