using KinematicCharacterController;
using UnityEngine;
[System.Serializable]
public class IdleState : IEnemyState
{
    private EnemyStateHandler handler;
    private EnemyMain main;

    public IdleState(EnemyStateHandler handler)
    {
        this.handler = handler;
    }

    public IdleState(EnemyMain main)
    {
        this.main = main;
    }

    public void OnEnter()
    {
        Debug.Log("is now idle");
    }

    public void Update()
    {
        if (handler != null)
        {
            if (handler.CheckTargetOnView(handler.Target))
            {
                handler.SetState(handler.GetAlertState());
            }
            return;
        }

        if (main == null) return;

        if (main.target == null)
        {
            return;
        }

        if (main.Watching())
        {
            main.SetState(main.GetAlertState());
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

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList Settings)
    {
        //Debug.Log("NEW");
        return Vector3.zero;
    }


}
