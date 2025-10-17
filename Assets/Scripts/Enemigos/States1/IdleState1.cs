using KinematicCharacterController;
using UnityEngine;

[System.Serializable]
public class IdleState1 : IEnemyState
{
    private EnemyStateHandler ai;

    public IdleState1(EnemyStateHandler main)
    {
        ai = main;
    }

    public void OnEnter()
    {
        ai.ExitCombatMode();
        Debug.Log($"{ai.name} is now idle.");
    }

    public void Update()
    {
        if (ai.Target != null && ai.CheckTargetOnView())
        {
            ai.SetState(ai.GetAlertState());
        }
    }

    public void OnExit()
    {
        Debug.Log($"{ai.name} left idle state.");
    }

    public Quaternion UpdateRotation(Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
    {
        return currentRotation;
    }

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList Settings, ref float _timeSinceUngrounded)
    {
        return Vector3.zero;
    }
}
