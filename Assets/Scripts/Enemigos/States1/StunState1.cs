using KinematicCharacterController;
using System.Collections;
using UnityEngine;

public class StunState1 : IEnemyState
{
    private EnemyStateHandler ai;
    private float stunTimer;
    private const float stunDuration = 1.5f;

    public StunState1(EnemyStateHandler handler)
    {
        ai = handler;
    }

    public void OnEnter()
    {
        stunTimer = stunDuration;
        ai.ExitCombatMode();
        ai.attackComponent.ResetAttackCycle();

        if (ai.TryGetComponent(out UnityEngine.AI.NavMeshAgent agent))
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        ai.character.Motor.BaseVelocity = Vector3.zero;
        ai.character.UpdateInputs(new EnemyInput { Move = Vector3.zero, Direction = Vector3.zero }, ai.GetBehaviourState());

        Debug.Log($"{ai.name} is stunned");
    }

    public void Update()
    {
        stunTimer -= Time.deltaTime;

        if (stunTimer <= 0f)
        {
            ai.StartCoroutine(ExitStunDelay());
        }
    }

    private IEnumerator ExitStunDelay()
    {
        stunTimer = float.MaxValue; 
        yield return new WaitForSeconds(0.25f); 
        ai.SetState(ai.GetBlockState());
    }

    public void OnExit()
    {
        ai.nextAttackTime = Time.time + Random.Range(0.8f, 1.4f);
        Debug.Log($"{ai.name} recovered from stun.");
    }

    public Quaternion UpdateRotation(Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
        => currentRotation;

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList Settings, ref float _timeSinceUngrounded)
    {
        Vector3 vel = motor.BaseVelocity;
        vel = Vector3.Lerp(vel, Vector3.zero, deltaTime * 2f); 
        return vel;
    }
}

