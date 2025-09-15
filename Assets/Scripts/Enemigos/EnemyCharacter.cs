using KinematicCharacterController;
using UnityEngine;


public enum EnemyBehaviourState
{
    Default,
    Combat,
    Dead,
    
}

[System.Serializable]
public struct EnemyCharacterState
{
    public EnemyBehaviourState BehaviourState;
    public MovementState MovementState;
}

public class EnemyCharacter : MonoBehaviour, ICharacterController
{
    [SerializeField] private KinematicCharacterMotor motor;

    public EnemySettings default_Settings;





    private Vector3 target;

    public EnemyCharacterState _state;
    private EnemyCharacterState _lastState;
    private EnemyCharacterState _tempState;

    private Vector3 _externalForces;
    private Vector3 _externalExplosiveForces;

    private Vector3 _requestedRotation;
    private Vector3 _requestedMovement;


    public void Initialize()
    {
        _lastState = _state;
        motor.CharacterController = this;
        motor.GroundDetectionExtraDistance = 0.1f;
    }

    public void UpdateInputs(EnemyInput input)
    {
        _requestedRotation = input.Direction;
       
        _requestedMovement = input.Move;
        Debug.Log("Requested Movement: " + _requestedMovement.magnitude);
    }

    public void AfterCharacterUpdate(float deltaTime)
    {
        switch (_state.BehaviourState)
        {
            case EnemyBehaviourState.Default:
                break;
            case EnemyBehaviourState.Combat:
                break;
            case EnemyBehaviourState.Dead:
                break;
        }
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
        switch (_state.BehaviourState)
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
        switch (_state.BehaviourState)
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
       switch(_state.BehaviourState)
        {
            case EnemyBehaviourState.Default:

                switch (_state.MovementState)
                {
                    case MovementState.Idle:
                        break;
                    case MovementState.Moving:

                        var forward = Vector3.ProjectOnPlane(
                                      _requestedRotation,
                                      motor.CharacterUp);

                       

                        currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
                        break;      
                }

                break;
            case EnemyBehaviourState.Combat:
                break;
            case EnemyBehaviourState.Dead:
                break;
        }
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        switch (_state.BehaviourState)
        {
            case EnemyBehaviourState.Default:

                switch (_state.MovementState)
                {
                    case MovementState.Idle:
                        break;
                    case MovementState.Moving:

                        if (motor.GroundingStatus.IsStableOnGround)
                        {

                                var groundedMovement = motor.GetDirectionTangentToSurface
                                (
                                    direction: _requestedMovement,
                                    surfaceNormal: motor.GroundingStatus.GroundNormal
                                ) * _requestedMovement.magnitude;    

                                currentVelocity = groundedMovement * default_Settings.walkSpeed;


                        }
                        else
                        {
                            if (_requestedMovement.sqrMagnitude > 0f)
                            {
                                var planarMovement = Vector3.ProjectOnPlane
                                (
                                    vector: _requestedMovement,
                                    planeNormal: motor.CharacterUp
                                ) * _requestedMovement.magnitude;

                                var currentPlanarVelocity = Vector3.ProjectOnPlane
                                (
                                    vector: currentVelocity,
                                    planeNormal: motor.CharacterUp
                                );

                                var movementForce = planarMovement * default_Settings.AirAcceleration * deltaTime;

                                if (currentPlanarVelocity.magnitude < default_Settings.AirSpeed)
                                {
                                    var targetPlanarVelocity = currentPlanarVelocity + movementForce;

                                    targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, default_Settings.AirSpeed);
                                    movementForce = targetPlanarVelocity - currentPlanarVelocity;
                                }

                                else if (Vector3.Dot(currentPlanarVelocity, movementForce) > 0f)
                                {
                                    var contrainedMovementForce = Vector3.ProjectOnPlane
                                    (
                                        vector: movementForce,
                                        planeNormal: currentPlanarVelocity.normalized
                                    );
                                    movementForce = contrainedMovementForce;

                                }

                                if (motor.GroundingStatus.FoundAnyGround) // prevent wall climbing in the air
                                {
                                    if (Vector3.Dot(movementForce, currentVelocity + movementForce) > 0f)
                                    {
                                        var obstructedNormal = Vector3.Cross
                                        (
                                            motor.CharacterUp,
                                            Vector3.Cross
                                            (
                                                motor.CharacterUp,
                                                motor.GroundingStatus.GroundNormal
                                            )
                                        ).normalized;
                                        movementForce = Vector3.ProjectOnPlane(movementForce, obstructedNormal);
                                    }
                                }


                                currentVelocity += movementForce;
                            }


                            currentVelocity += motor.CharacterUp * default_Settings.Gravity * deltaTime;

                        }
                




                        break;
                }





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
}
