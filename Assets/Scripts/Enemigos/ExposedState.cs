using KinematicCharacterController;
using UnityEngine;

public class ExposedState : IEnemyState
{
    private EnemyStateHandler handler;
    private EnemyMain main;
    private float exposedTimer;
    private float exposedDuration = 1.5f; 

    public ExposedState(EnemyStateHandler handler)
    {
        this.handler = handler;
    }

    public ExposedState(EnemyMain main)
    {
        this.main = main;
    }

    public void OnEnter()
    {
        exposedTimer = exposedDuration;
        Debug.Log("exposed");
    }

    public void Update()
    {
        if (handler != null)
        {
            if (handler.Target == null)
            {
                handler.SetState(handler.GetIdleState());
                return;
            }

            exposedTimer -= Time.deltaTime;

            if (exposedTimer <= 0f)
            {
                handler.SetBehaviourState(EnemyBehaviourState.Default);
            }
            return;
        }

        if (main == null)
        {
            return;
        }

        if (main.target == null)
        {
            main.SetState(main.GetIdleState());
            return;
        }

        exposedTimer -= Time.deltaTime;

        if (exposedTimer <= 0f)
        {
            main.SetState(main.GetRecoverState());
        }
    }

    public void OnExit()
    {
        if (handler != null)
        {
            Debug.Log($"{handler.name} recovered from being exposed.");
        }
        else if (main != null)
        {
            Debug.Log($"{main.name} recovered from being exposed.");
        }
    }

    public Quaternion UpdateRotation(Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
    {
        if (handler == null)
        {
            return currentRotation;
        }
        return currentRotation;
    }

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList Settings, ref float _timeSinceUngrounded)
    {
        if (handler == null)
        {
            return currentVelocity;
        }
        return currentVelocity;
    }

}
