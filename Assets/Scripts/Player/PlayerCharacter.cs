using UnityEngine;
using KinematicCharacterController;
using System.Collections;

public struct CharacterInput
{
    public Quaternion Rotation;
    public Vector3 Move;
    public bool Jump;
    public bool Dash;
}



public enum Stance
{
    Stand,
    Block    
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
    private Transform _cameraTransform; 

    
    [Space]

    [FoldoutGroup("DefaultMovementBehaviourSettings")]

    [SerializeReference]
    public DefaultMoveSettings DefaultStandSettings;

    [SerializeReference]
    [FoldoutGroup("DefaultMovementBehaviourSettings")]
    public DefaultBlockDodgeCounterSettings DefaultBlockSettings;

    [SerializeReference]
    [FoldoutGroup("DefaultMovementBehaviourSettings")]
    public DefaultAirSettings DefaultAirSettings;  
    

    
    [SerializeField] private CharacterBodySettings BodyStandSettings;
    [FoldoutGroup("BodySettings")]
    

    [Space]
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] protected Transform cameraTarget; 


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

    public void Initialize(Transform cameraTransform = null)
    {
        _state.Stance = Stance.Stand;
        _lastState = _state;        
        motor.CharacterController = this;
        motor.GroundDetectionExtraDistance = 0.1f;
        if (cameraTransform != null)
            _cameraTransform = cameraTransform;
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
        Transform cam = _cameraTransform != null ? _cameraTransform : cameraTarget;
    }
    public void UpdateBody()
    {
        switch(_state.BehaviourState)
        {
            case BehaviourState.Default:
                var currentHeight = motor.Capsule.height;
                var cameraTargetHeight = currentHeight * BodyStandSettings.CameraHeight;
                
                cameraTarget.localPosition = new Vector3(0f, cameraTargetHeight, 0f);
                break;
        }
       
    }

    public void AfterCharacterUpdate(float deltaTime)
    {
        switch (_state.BehaviourState)
        {
            case BehaviourState.Default:
            

                


                break;
        }
               

    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
        switch (_state.BehaviourState)
        {
            case BehaviourState.Default:
            

                _tempState = _state;
                

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
                    if (_state.Stance is Stance.Stand or Stance.Block)
                    {

                        float speed = _state.Stance is Stance.Stand ? DefaultStandSettings.Speed : DefaultBlockSettings.Speed;

                        float response = _state.Stance is Stance.Stand ? DefaultStandSettings.Response : DefaultBlockSettings.Response;

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
                }
                else 
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

                        if (motor.GroundingStatus.FoundAnyGround)
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
    
    public IEnumerator PerformArcMoveCoroutine(Transform target, float dodgeRange, float dodgeDuration, PlayerCamera cameraTransform, System.Action onComplete, int directionOverride = 0)
    {
        if (target == null)
        {
            onComplete?.Invoke();
            yield break;
        }

        float playerY = transform.position.y;
        Vector3 targetPos = target.position;

        Vector3 rel = transform.position - targetPos;
        Vector3 relHorizontal = new Vector3(rel.x, 0f, rel.z);
        float startAngle = Mathf.Atan2(relHorizontal.z, relHorizontal.x);

        float targetDistance = relHorizontal.magnitude;
        float rawRadius = targetDistance * 0.6f;
        float minRadius = 0.5f;
        float maxRadius = Mathf.Max(1f, dodgeRange * 0.9f);
        float radius = Mathf.Clamp(rawRadius, minRadius, maxRadius);

    float signed = Vector3.SignedAngle(transform.forward, relHorizontal.normalized, Vector3.up);
    int dirSign = directionOverride != 0 ? directionOverride : (signed >= 0f ? 1 : -1);

        float normalized = Mathf.Clamp01(targetDistance / dodgeRange);
        float closeness = 1f - normalized;
        float minAngle = Mathf.PI * 0.5f;
        float maxAngle = Mathf.PI;
        float angleSpan = Mathf.Lerp(minAngle, maxAngle, closeness);
        float endAngle = startAngle + dirSign * angleSpan;

        float elapsed = 0f;
        int largeDiffFrames = 0;
        const float largeDiffThreshold = 0.6f; 
        const int framesToWarn = 6;

        while (elapsed < dodgeDuration)
        {
            float dt = Mathf.Max(Time.deltaTime, 0.0001f);
            float t = elapsed / dodgeDuration;
            float angle = Mathf.Lerp(startAngle, endAngle, t);

            Vector3 desiredPos = targetPos + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            desiredPos.y = playerY;

            
            Vector3 currentMotorPos = motor.TransientPosition;
            Vector3 desiredVel = (desiredPos - currentMotorPos) / dt;

            
           

           
            float diff = Vector3.Distance(desiredPos, currentMotorPos);
            if (diff > largeDiffThreshold)
            {
                largeDiffFrames++;
            }
            else
            {
                largeDiffFrames = 0;
            }
            if (largeDiffFrames >= framesToWarn)
            {
                
                largeDiffFrames = 0;
            }

            
            motor.BaseVelocity = desiredVel;

            

            elapsed += Time.deltaTime;
            yield return null;
        }

        float finalAngle = endAngle;
        Vector3 finalPos = targetPos + new Vector3(Mathf.Cos(finalAngle) * radius, 0f, Mathf.Sin(finalAngle) * radius);
        finalPos.y = playerY;

        
        motor.BaseVelocity = Vector3.zero;
        SetPosition(finalPos, killvelocity: true);

        Vector3 lookDir = targetPos - transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
        }
        
        onComplete?.Invoke();
    }


    // New directional dash: moves the player along a given horizontal direction for the given distance and duration.
    // This is used for a player-aimed dash (e.g. SHIFT + movement) instead of the arc-dodge around a target.
    public IEnumerator PerformDirectionalDashCoroutine(Vector3 direction, float distance, float duration, PlayerCamera cameraTransform, System.Action onComplete)
    {
        // Ensure only horizontal movement
        Vector3 startPos = transform.position;
        startPos.y = transform.position.y;

        Vector3 dirHorizontal = new Vector3(direction.x, 0f, direction.z);
        if (dirHorizontal.sqrMagnitude < 0.0001f)
        {
            onComplete?.Invoke();
            yield break;
        }

        Vector3 normDir = dirHorizontal.normalized;
        Vector3 targetPos = startPos + normDir * distance;
        targetPos.y = startPos.y;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float dt = Mathf.Max(Time.deltaTime, 0.0001f);
            float t = Mathf.Clamp01(elapsed / duration);

            // simple ease (smoothstep)
            float easeT = t * t * (3f - 2f * t);
            Vector3 desiredPos = Vector3.LerpUnclamped(startPos, targetPos, easeT);

            Vector3 currentMotorPos = motor.TransientPosition;
            Vector3 desiredVel = (desiredPos - currentMotorPos) / dt;

            motor.BaseVelocity = desiredVel;

            elapsed += Time.deltaTime;
            yield return null;
        }

        motor.BaseVelocity = Vector3.zero;
        SetPosition(targetPos, killvelocity: true);

        // Face the dash direction
        Vector3 lookDir = normDir;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
        }

        onComplete?.Invoke();
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

    public void setState (bool request)
    {
        if (request)
        {
            _state.Stance = Stance.Block;
        }
        else
        {
            _state.Stance = Stance.Stand;
        }        
        
    }
}
