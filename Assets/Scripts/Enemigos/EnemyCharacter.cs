using KinematicCharacterController;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.AI;


public enum EnemyBehaviourState
{
    Default,
    Combat,
    Dead,

}

public enum MovementMode
{
    NavMesh,
    KCC
}

[System.Serializable]
public struct EnemyCharacterState
{
    public bool Grounded;
    public MovementState MovementState;
    public Vector3 Velocity;
    public Quaternion Rotation;
    public bool Jump;
}

public class EnemyCharacter : MonoBehaviour, ICharacterController
{
    [SerializeField] private KinematicCharacterMotor motor;
    public KinematicCharacterMotor Motor => motor;

    private EnemySettingsList default_Settings;


    private EnemyBehaviourState enemyBehaviourState;
    private IEnemyState currentState;


    public MovementMode CurrentMode { get; private set; } = MovementMode.NavMesh;

    private Vector3 target;



    public EnemyCharacterState _state;
    private EnemyCharacterState _lastState;
    private EnemyCharacterState _tempState;

    private Vector3 _externalForces;
    private Vector3 _externalExplosiveForces;

    private Vector3 _requestedRotation;
    private Vector3 _requestedMovement;
    private bool _requestedJump;
    [NonSerialized]public float _timeSinceUngrounded;
    private float _timeSinceJumpRequest;
    private bool _ungroundedDueToJump;

    public void Initialize(EnemySettingsList enemySettings, EnemyBehaviourState enemyBehaviourState, IEnemyState EnemyState)
    {
        _lastState = _state;
        motor.CharacterController = this;
        motor.GroundDetectionExtraDistance = 0.1f;
        default_Settings = enemySettings;
        this.enemyBehaviourState = enemyBehaviourState;
        currentState = EnemyState;
    }

    public void UpdateInputs(EnemyInput input, EnemyBehaviourState state)
    {
        _requestedRotation = input.Direction;

        _requestedMovement = input.Move;

        enemyBehaviourState = state;
        var wasResquestedJump = _requestedJump;
        _requestedJump = _requestedJump || input.Jump;
        if (_requestedJump && wasResquestedJump)
        {
            _timeSinceJumpRequest = 0f;
        }
    }

    public void AfterCharacterUpdate(float deltaTime)
    {
        
      _state.Velocity = motor.Velocity;
      _state.Jump = _ungroundedDueToJump && _timeSinceUngrounded < 0.4f;
     _state.MovementState = motor.Velocity.magnitude > 0.1f ? MovementState.Moving : MovementState.Idle;
          
   
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {

    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {

    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {

    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        switch (enemyBehaviourState)
        {
            case EnemyBehaviourState.Default:

                break;
            case EnemyBehaviourState.Combat:
                break;
            case EnemyBehaviourState.Dead:
                break;
        }
    }

    public void PostGroundingUpdate(float deltaTime)
    {

    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
        switch (enemyBehaviourState)
        {
            case EnemyBehaviourState.Default:
                break;
            case EnemyBehaviourState.Combat:
                break;
            case EnemyBehaviourState.Dead:
                break;
        }
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        switch (enemyBehaviourState)
        {
            case EnemyBehaviourState.Default:

                currentRotation = currentState.UpdateRotation(currentRotation, deltaTime, _requestedRotation, motor);

                break;
            case EnemyBehaviourState.Combat:
                currentRotation = currentState.UpdateRotation(currentRotation, deltaTime, _requestedRotation, motor);
                break;
            case EnemyBehaviourState.Dead:
                break;
        }
    }


    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (motor.GroundingStatus.IsStableOnGround)
        {
        _state.Grounded =true;

        }
        else
        {
            _state.Grounded = false;
        }
        switch (enemyBehaviourState)
        {
            case EnemyBehaviourState.Default:

                currentVelocity =  currentState.UpdateVelocity( currentVelocity, deltaTime, motor, _requestedMovement, default_Settings, ref _timeSinceUngrounded);
               




                if (_externalExplosiveForces.magnitude > 0f)
                {
                    motor.ForceUnground();
                    float currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                    float explosiveVerticalSpeed = Vector3.Dot(_externalExplosiveForces, motor.CharacterUp);

                    if (explosiveVerticalSpeed > currentVerticalSpeed)
                    {
                        currentVelocity += motor.CharacterUp * (explosiveVerticalSpeed - currentVerticalSpeed);
                    }

                    Vector3 explosiveHorizontal = Vector3.ProjectOnPlane(_externalExplosiveForces, motor.CharacterUp);
                    currentVelocity += explosiveHorizontal;

                    // Limpia para el siguiente frame
                    _externalExplosiveForces = Vector3.zero;

                }

                if (_externalForces.magnitude > 0)
                {
                    motor.ForceUnground();
                    currentVelocity += _externalForces;
                    _externalForces = Vector3.zero;
                }

                break;

            case EnemyBehaviourState.Combat:

                currentVelocity = currentState.UpdateVelocity(currentVelocity, deltaTime, motor, _requestedMovement, default_Settings, ref _timeSinceUngrounded);

                if (_externalExplosiveForces.magnitude > 0f)
                {
                    motor.ForceUnground();
                    float currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                    float explosiveVerticalSpeed = Vector3.Dot(_externalExplosiveForces, motor.CharacterUp);

                    if (explosiveVerticalSpeed > currentVerticalSpeed)
                    {
                        currentVelocity += motor.CharacterUp * (explosiveVerticalSpeed - currentVerticalSpeed);
                    }

                    Vector3 explosiveHorizontal = Vector3.ProjectOnPlane(_externalExplosiveForces, motor.CharacterUp);
                    currentVelocity += explosiveHorizontal;

                    // Limpia para el siguiente frame
                    _externalExplosiveForces = Vector3.zero;

                }

                if (_externalForces.magnitude > 0)
                {
                    motor.ForceUnground();
                    currentVelocity += _externalForces;
                    _externalForces = Vector3.zero;
                }
                break;

            case EnemyBehaviourState.Dead:
                break;
        }
    }

    public EnemyCharacterState GetState() => _state;

    public EnemyCharacterState GetLastState() => _lastState;

    public void AddExternalExplosiveForce(Vector3 force)
    {
        _externalExplosiveForces += force;
    }

    public void AddExternalForce(Vector3 force)
    {
        _externalForces += force;
    }

    public void SetTarget(Vector3 target)
    {
        this.target = target;
    }

    public void UpdateState(IEnemyState state)
    {
        currentState = state;
    }

    void OnDrawGizmosSelected()
    {
        if (default_Settings != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, default_Settings.AISettings.attackRange);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, default_Settings.AISettings.detectionDistance);

        }
    }

    public void SetMovementMode(MovementMode mode)
    {
        CurrentMode = mode;
        if (TryGetComponent(out NavMeshAgent agent))
            agent.enabled = (mode == MovementMode.NavMesh);
        motor.enabled = (mode == MovementMode.KCC);
    }
}
