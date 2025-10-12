using KinematicCharacterController;
using UnityEngine;
[System.Serializable]
public class IdleState : IEnemyState
{
    private EnemyStateHandler ai;

    public IdleState(EnemyStateHandler main)
    {
        ai = main;
    }

    public void OnEnter()
    {
        Debug.Log("is now idle");
    }

    public void Update()
    {
        if (ai.CheckTargetOnView(ai.Target))
        {
            ai.SetState(ai.GetAlertState());
        }
    }

    public void OnExit()
    {
        Debug.Log("no longer idle lmao");
    }

    public Quaternion UpdateRotation( Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
    {
        return currentRotation;
    }

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList Settings, ref float _timeSinceUngrounded)
    {
        //Debug.Log("NEW");
        return Vector3.zero;
    }



}
