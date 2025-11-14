using KinematicCharacterController;
using UnityEngine;

public class DeadState : IEnemyState
{
    private EnemyStateHandler handler;
    private EnemyMain main;

    public DeadState(EnemyStateHandler handler)
    {
        this.handler = handler;
    }

    public DeadState(EnemyMain main)
    {
        this.main = main;
    }

    public void OnEnter()
    {
        Debug.Log("dead lol");
        if (main != null)
        {
            main.StopMovement();
            if (main.TryGetComponent<Collider>(out var col))
            {
                col.enabled = false;
            }
            if (main.TryGetComponent<Rigidbody>(out var body))
            {
                body.isKinematic = true;
                body.linearVelocity = Vector3.zero;
            }
        }
    }

    public void Update()
    {
    }

    public void OnExit()
    {
    }

    public Quaternion UpdateRotation(Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
    {
        if (handler == null)
        {
            return currentRotation;
        }
        return Quaternion.identity;
    }

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList Settings)
    {
        if (handler == null)
        {
            return currentVelocity;
        }
        return currentVelocity;
    }

}
