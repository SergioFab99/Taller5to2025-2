using UnityEngine;
using KinematicCharacterController;

public struct CharacterInput
{
    public Quaternion Rotation;
    public Vector3 Move;
    public bool Jump;
    public CrouchInput Crouch;
    public bool Grab;
    public bool Throw;
}

public enum CrouchInput
{
    None,
    Toggle,
    Hold
}

public enum Stance
{
    Stand,
    Crouch,
    Sliding
}

public enum MovementState
{
    Idle,
    Moving,

}

public enum BehaviourState
{
    Default,
}
[System.Serializable]
public struct CharacterState
{
    public bool Grounded;
    public Stance Stance;
    public MovementState MovementState;
    public BehaviourState BehaviourState;
    public Vector3 Velocity;
    public Vector3 Acceleration;
}


public class PlayerCharacter : MonoBehaviour, ICharacterController
{
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] protected Transform cameraTarget; // child of Character
    [Space]

    [FoldoutGroup("DefaultMovementBehaviourSettings")]

    [SerializeReference]
    public DefaultStanceSettings DefaultStandSettings;

    [SerializeReference]
    [FoldoutGroup("DefaultMovementBehaviourSettings")]
    public DefaultStanceSettings DefaultCrouchSettings;

    [SerializeReference]
    [FoldoutGroup("DefaultMovementBehaviourSettings")]
    public DefaultAirSettings DefaultAirSettings;

    [SerializeReference]
    [FoldoutGroup("DefaultMovementBehaviourSettings")]
    public DefaultSlideSettings DefaultSlideSettings;

    [FoldoutGroup("BodySettings")]
    [SerializeField] private CharacterBodySettings BodyStandSettings;
    [FoldoutGroup("BodySettings")]
    [SerializeField] private CharacterBodySettings BodyCrouchSettings;

    [Space]


    [SerializeField] public CharacterState _state;

    private CharacterState _lastState;
    private CharacterState _tempState;


    private Vector3 _externalForces;
    private Vector3 _externalExplosiveForces;

    private Quaternion _requestedRotation;
    private Vector3 _requestedMovement;
    private bool _requestedJump;
    private bool _requestedCrouch;
    private bool _requestedCrouchOnAir;
    private float _timeSinceUngrounded;
    private float _timeSinceJumpRequest;
    private bool _ungroundedDueToJump;

    private Collider[] _uncrouchOverlapResults = new Collider[8];

    public void Initialize()
    {
        _state.Stance = Stance.Stand;
        _lastState = _state;
        motor.CharacterController = this;
        motor.GroundDetectionExtraDistance = 0.1f;
    }

    public void UpdateInput(CharacterInput input)
    {
        _requestedRotation = input.Rotation;
        _requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y);
        _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1f);

        _requestedMovement = input.Rotation * _requestedMovement;

        var wasResquestedJump = _requestedJump;
        _requestedJump = _requestedJump || input.Jump;
        if (_requestedJump && wasResquestedJump)
        {
            _timeSinceJumpRequest = 0f;
        }
        var wasRequestedCrouch = _requestedCrouch;
        _requestedCrouch = input.Crouch switch
        {
            CrouchInput.Toggle => !_requestedCrouch,
            CrouchInput.None => _requestedCrouch,
            _ => _requestedCrouch
        };

        if (_requestedCrouch && !wasRequestedCrouch)
        {
            _requestedCrouchOnAir = !_state.Grounded;
        }
        else if (!_requestedCrouch && wasRequestedCrouch)
        {
            _requestedCrouchOnAir = false;
        }

        
        if (input.Grab)
        {
            
        }
        if (input.Throw)
        {
            
        }
    }
    public void UpdateBody()
    {
        switch(_state.BehaviourState)
        {
            case BehaviourState.Default:

                var currentHeight = motor.Capsule.height;
                var cameraTargetHeight = currentHeight *
                (
                   _state.Stance is Stance.Stand ? BodyStandSettings.CameraHeight : BodyCrouchSettings.CameraHeight
                );

                cameraTarget.localPosition = new Vector3(0f, cameraTargetHeight, 0f);

                break;
        }
       
    }

    public void AfterCharacterUpdate(float deltaTime)
    {
        switch (_state.BehaviourState)
        {
            case BehaviourState.Default:

                //Uncrouch
                if (!_requestedCrouch && _state.Stance is not Stance.Stand)
                {
                    Debug.Log("Uncrouching");
                    motor.SetCapsuleDimensions
                    (
                        radius: motor.Capsule.radius,
                        height: BodyStandSettings.Height,
                        yOffset: BodyStandSettings.Height * 0.5f
                    );

                    Vector3 pos = motor.TransientPosition;
                    Quaternion rot = motor.TransientRotation;
                    LayerMask layers = motor.CollidableLayers;
                    if (motor.CharacterOverlap(pos, rot, _uncrouchOverlapResults, layers, QueryTriggerInteraction.Ignore) > 0)
                    {
                        _requestedCrouch = true;
                        motor.SetCapsuleDimensions
                        (
                            radius: motor.Capsule.radius,
                            height: BodyCrouchSettings.Height,
                            yOffset: BodyCrouchSettings.Height * 0.5f
                        );
                    }
                    else
                    {
                        _state.Stance = Stance.Stand;
                    }
                }
                _state.Grounded = motor.GroundingStatus.IsStableOnGround;
                _state.Velocity = motor.Velocity;
                if (_requestedMovement.magnitude >= 0.1f)
                {
                    _state.MovementState = MovementState.Moving;
                }
                else { _state.MovementState = MovementState.Idle; }
                _lastState = _tempState;


                break;
        }
               

    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
        switch (_state.BehaviourState)
        {
            case BehaviourState.Default:

                _tempState = _state;
                //Crouching
                if (_requestedCrouch && _state.Stance is Stance.Stand)
                {
                    Debug.Log("Crouching");
                    _state.Stance = Stance.Crouch;
                    motor.SetCapsuleDimensions
                    (
                        radius: motor.Capsule.radius,
                        height: BodyCrouchSettings.Height,
                        yOffset: BodyCrouchSettings.Height * 0.5f
                    );
                }

                break;
        }
                
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
       
    }

    public void PostGroundingUpdate(float deltaTime)
    {
        switch (_state.BehaviourState)
        {
            case BehaviourState.Default:

                if (!motor.GroundingStatus.IsStableOnGround && _state.Stance is Stance.Sliding)
                {
                    _state.Stance = Stance.Crouch;
                }

                break;
        }
                
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
       
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        switch (_state.BehaviourState)
        {
            case BehaviourState.Default:

                var forward = Vector3.ProjectOnPlane(
                _requestedRotation * Vector3.forward,
                 motor.CharacterUp
                );

                currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);

                break;
        }
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {

        switch (_state.BehaviourState)
        {
            case BehaviourState.Default:

                _state.Acceleration = Vector3.zero;
                if (motor.GroundingStatus.IsStableOnGround)
                {
                    _timeSinceUngrounded = 0f;
                    _ungroundedDueToJump = false;
                    var groundedMovement = motor.GetDirectionTangentToSurface
                    (
                      direction: _requestedMovement,
                      surfaceNormal: motor.GroundingStatus.GroundNormal
                    ) * _requestedMovement.magnitude;

                    //Start Sliding
                    {
                        bool moving = groundedMovement.sqrMagnitude > 0f;
                        var crouching = _state.Stance is Stance.Crouch;
                        var wasStanding = _lastState.Stance is Stance.Stand;
                        var wasInAir = !_lastState.Grounded;
                        if (moving && crouching && (wasStanding || wasInAir))
                        {

                            Debug.DrawRay(transform.position, _lastState.Velocity, Color.red, 5f);
                            _state.Stance = Stance.Sliding;
                            //The kinematic motor project the velocity in a plane when hit the ground so it nos save all the momentung while slinding and hiting the ground
                            if (wasInAir)
                            {
                                currentVelocity = Vector3.ProjectOnPlane
                                (
                                    vector: _lastState.Velocity,
                                    planeNormal: motor.GroundingStatus.GroundNormal
                                );
                            }

                            var effectiveSlideStartSpeed = DefaultSlideSettings.SlideStartSpeed;
                            if (!_lastState.Grounded && !_requestedCrouchOnAir)
                            {
                                effectiveSlideStartSpeed = 0f;
                                _requestedCrouchOnAir = false;
                            }
                            Debug.Log("IsSliding");
                            var slideSpeed = Mathf.Max(effectiveSlideStartSpeed, currentVelocity.magnitude);
                            currentVelocity = motor.GetDirectionTangentToSurface
                            (
                                direction: currentVelocity,
                                surfaceNormal: motor.GroundingStatus.GroundNormal
                            ) * slideSpeed;

                        }
                    }

                    //Move
                    if (_state.Stance is Stance.Stand or Stance.Crouch)
                    {

                        float speed = _state.Stance is Stance.Stand ? DefaultStandSettings.Speed : DefaultCrouchSettings.Speed;

                        float response = _state.Stance is Stance.Stand ? DefaultStandSettings.Response : DefaultCrouchSettings.Response;

                        Vector3 targetVelocity = groundedMovement * speed;
                        Vector3 moveVelocity = Vector3.Lerp
                            (
                                a: currentVelocity,
                                b: targetVelocity,
                                t: 1f - Mathf.Exp(-response * deltaTime)
                            );
                        _state.Acceleration = moveVelocity - currentVelocity;
                        currentVelocity = moveVelocity;
                    }
                    else // continue sliding
                    {
                        Debug.Log("Continuing sliding");
                        //Friction
                        currentVelocity -= currentVelocity * (DefaultSlideSettings.SlideFriction * deltaTime);

                        //Slope
                        {
                            var force = Vector3.ProjectOnPlane
                            (
                                vector: -motor.CharacterUp,
                                planeNormal: motor.GroundingStatus.GroundNormal
                            ) * DefaultSlideSettings.SlideGravity;

                            currentVelocity -= force * deltaTime;
                        }

                        // Steer
                        {
                            var currentSpeed = currentVelocity.magnitude;
                            var targetVelocity = groundedMovement * currentSpeed;
                            var steerVelocity = currentVelocity;
                            var steerForce = (targetVelocity - steerVelocity) * DefaultSlideSettings.SlideSteerAcceleration * deltaTime;
                            // add steer and no add more force, just redirecting
                            steerVelocity += steerForce;
                            steerVelocity = Vector3.ClampMagnitude(steerVelocity, currentSpeed);

                            _state.Acceleration = (steerVelocity - currentVelocity) / deltaTime;
                            currentVelocity = steerVelocity;
                        }

                        //Stop
                        if (currentVelocity.magnitude < DefaultSlideSettings.SlideEndSpeed)
                        {
                            _state.Stance = Stance.Crouch;
                            Debug.Log("Crouching");
                            Debug.Log("Stop sliding");
                        }
                    }


                }
                else // in the air
                {
                    _timeSinceUngrounded += deltaTime;
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

                        var movementForce = planarMovement * DefaultAirSettings.AirAcceleration * deltaTime;

                        if (currentPlanarVelocity.magnitude < DefaultAirSettings.AirSpeed)
                        {
                            var targetPlanarVelocity = currentPlanarVelocity + movementForce;

                            targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, DefaultAirSettings.AirSpeed);
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
                    currentVelocity += motor.CharacterUp * DefaultAirSettings.Gravity * deltaTime;

                }

                if (_requestedJump)
                {
                    var grounded = motor.GroundingStatus.IsStableOnGround;
                    bool canCoyoteTime = _timeSinceUngrounded < DefaultAirSettings.CoyoteTime && !_ungroundedDueToJump;
                    if (grounded || canCoyoteTime)
                    {
                        Debug.Log("Jumping");
                        _requestedJump = false;
                        _requestedCrouch = false;
                        _requestedCrouchOnAir = false;

                        motor.ForceUnground(time: 0.1f);
                        _ungroundedDueToJump = true;

                        var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                        var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, DefaultAirSettings.JumpSpeed);

                        currentVelocity += motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);

                    }
                    else
                    {
                        _timeSinceJumpRequest += deltaTime;
                        bool canJumpLater = _timeSinceJumpRequest < (DefaultAirSettings.CoyoteTime * 0.16);
                        _requestedJump = canJumpLater;


                    }
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
        }

       
    }

    public void SetPosition(Vector3 position, bool killvelocity = true)
    {
        motor.SetPosition(position);
        if (killvelocity)
        {
            motor.BaseVelocity = Vector3.zero;
        }
    }

    public CharacterState GetState() => _state;

    public CharacterState GetLastState() => _lastState;

    public Vector3 GetCharacterUp() => motor.CharacterUp;

    public Transform GetCameraTarget() => cameraTarget;


    public void AddExternalExplosiveForce(Vector3 force)
    {
        _externalExplosiveForces += force;
    }
    public void AddExternalForce(Vector3 force)
    {
        _externalForces += force;
    }
}
