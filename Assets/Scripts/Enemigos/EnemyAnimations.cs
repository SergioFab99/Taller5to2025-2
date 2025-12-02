using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EnemyAnimations : MonoBehaviour
{
    private enum GaitState
    {
        Idle,
        Walk,
        Run,
        Sprint
    }

    #region Animation Variable Hashes

    private readonly int _moveSpeedHash = Animator.StringToHash("MoveSpeed");
    private readonly int _currentGaitHash = Animator.StringToHash("CurrentGait");

    private readonly int _isJumpingAnimHash = Animator.StringToHash("IsJumping");
    private readonly int _fallingDurationHash = Animator.StringToHash("FallingDuration");

    private readonly int _inclineAngleHash = Animator.StringToHash("InclineAngle");

    private readonly int _strafeDirectionXHash = Animator.StringToHash("StrafeDirectionX");
    private readonly int _strafeDirectionZHash = Animator.StringToHash("StrafeDirectionZ");

    private readonly int _forwardStrafeHash = Animator.StringToHash("ForwardStrafe");
    private readonly int _isStrafingHash = Animator.StringToHash("IsStrafing");

    private readonly int _isTurningInPlaceHash = Animator.StringToHash("IsTurningInPlace");

   

    private readonly int _isWalkingHash = Animator.StringToHash("IsWalking");
    private readonly int _isStoppedHash = Animator.StringToHash("IsStopped");
    private readonly int _isStartingHash = Animator.StringToHash("IsStarting");

    private readonly int _isGroundedHash = Animator.StringToHash("IsGrounded");

    
    private readonly int _leanValueHash = Animator.StringToHash("LeanValue");
    private readonly int _headLookXHash = Animator.StringToHash("HeadLookX");
    private readonly int _headLookYHash = Animator.StringToHash("HeadLookY");

    private readonly int _bodyLookXHash = Animator.StringToHash("BodyLookX");
    private readonly int _bodyLookYHash = Animator.StringToHash("BodyLookY");

    private readonly int _locomotionStartDirectionHash = Animator.StringToHash("LocomotionStartDirection");

    #endregion
    #region Lean Settings

    [Header("Player Lean")]
    [Tooltip("Flag indicating if leaning is enabled.")]
    [SerializeField]
    private bool _enableLean = true;
    [Tooltip("Delay for leaning.")]
    [SerializeField]
    private float _leanDelay;
    [Tooltip("Current value for leaning.")]
    [SerializeField]
    private float _leanValue;
    [Tooltip("Curve for leaning.")]
    [SerializeField]
    private AnimationCurve _leanCurve;
    [Tooltip("Delay for head leaning looks.")]
    [SerializeField]
    private float _leansHeadLooksDelay;
    [Tooltip("Flag indicating if an animation clip has ended.")]
    [SerializeField]
    private bool _animationClipEnd;

    #endregion

    #region Runtime Properties
    private bool _isStopped;
    private bool _isGrounded = true;
    private Vector3 _currentRotation = new Vector3(0f, 0f, 0f);
    private Vector3 _moveDirection;
    private bool _isJumping;
    private float _moveSpeed;
    private GaitState _currentGait;
    public float runThreshold;
    public float sprintThreshold;
    public float fallDuration;
    #endregion

    [SerializeField]
    private Animator _animator;

    public void Initialize()
    {
        _animator = GetComponent<Animator>();
        _isGrounded = true;
        _animator.SetBool(_isGroundedHash, _isGrounded);
    }

    public void AnimUpdate(float deltaTime, EnemyCharacter character)
    {
        UpdateProperties(character);
        UpdateAnimatorController(character);
    }


    void UpdateProperties(EnemyCharacter character)
    {
        _isStopped = character._state.MovementState == MovementState.Idle ? true : false;
        _moveDirection = character._state.Velocity;
        _isGrounded = character._state.Grounded;
        _currentRotation = character._state.Rotation.eulerAngles;
        _isJumping = character._state.Jump;
        _moveSpeed = _moveDirection.magnitude;
        if (_moveSpeed < 0.01)
        {
            _currentGait = GaitState.Idle;
        }
        else if (_moveSpeed < runThreshold)
        {
            _currentGait = GaitState.Walk;
        }
        else if (_moveSpeed < sprintThreshold)
        {
            _currentGait = GaitState.Run;
        }
        else
        {
            _currentGait = GaitState.Sprint;
        }
        fallDuration = character._timeSinceUngrounded;
    }

    void UpdateAnimatorController( EnemyCharacter character)
    {
        _animator.SetBool(_isGroundedHash,_isGrounded);
        _animator.SetBool(_isStoppedHash, _isStopped);
        _animator.SetFloat(_moveSpeedHash, _moveSpeed);
        _animator.SetInteger(_currentGaitHash, (int)_currentGait);
        _animator.SetFloat(_fallingDurationHash, fallDuration);

    }
    public void CheckIsStopped(EnemyCharacter character)
    {
       
    }
}
