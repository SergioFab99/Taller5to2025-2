using KinematicCharacterController;
using UnityEngine;

public class ExposedState1 : IEnemyState
{
    private EnemyStateHandler ai;
    private float exposedTimer;
    private readonly float exposedDuration = 1.5f;

    public ExposedState1(EnemyStateHandler main)
    {
        ai = main;
    }

    public void OnEnter()
    {
        exposedTimer = exposedDuration;
        ai.ExitCombatMode(); 
        Debug.Log($"{ai.name} is EXPOSED!");
    }

    public void Update()
    {
        exposedTimer -= Time.deltaTime;

        if (exposedTimer <= 0f)
        {
            if (ai.Target != null && ai.CheckTargetOnView())
                ai.SetState(ai.GetAlertState());
            else
                ai.SetState(ai.GetIdleState());
        }
    }

    public void OnExit()
    {
        Debug.Log($"{ai.name} recovered from exposure.");
    }

    public Quaternion UpdateRotation(Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
        => currentRotation;

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList Settings)
        => currentVelocity;
}
