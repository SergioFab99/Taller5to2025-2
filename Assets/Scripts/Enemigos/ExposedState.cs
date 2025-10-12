using KinematicCharacterController;
using UnityEngine;

public class ExposedState : IEnemyState
{
    private EnemyStateHandler ai;
    private float exposedTimer;
    private float exposedDuration = 1.5f; 

    public ExposedState(EnemyStateHandler main)
    {
        ai = main;
    }

    public void OnEnter()
    {
        exposedTimer = exposedDuration;
        Debug.Log("exposed");
    }

    public void Update()
    {
        if (ai.Target == null)
        {
            ai.SetState(ai.GetIdleState());
            return;
        }

        exposedTimer -= Time.deltaTime;

        if (exposedTimer <= 0f)
        {
            ai.SetBehaviourState(EnemyBehaviourState.Default);
        }
    }

    public void OnExit()
    {
        Debug.Log($"{ai.name} recovered from being exposed.");
    }

    public Quaternion UpdateRotation(Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
    {
        return currentRotation;
    }

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList Settings, ref float _timeSinceUngrounded)
    {
        return currentVelocity;
    }

}
