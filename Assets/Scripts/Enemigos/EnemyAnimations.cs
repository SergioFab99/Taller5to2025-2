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
    [SerializeField] private bool _enableLean = true;
    [Tooltip("Delay for leaning.")]
    [SerializeField] private float _leanDelay;
    [Tooltip("Current value for leaning.")]
    [SerializeField] private float _leanValue;
    [Tooltip("Curve for leaning.")]
    [SerializeField] private AnimationCurve _leanCurve;
    [Tooltip("Delay for head leaning looks.")]
    [SerializeField] private float _leansHeadLooksDelay;
    [Tooltip("Flag indicating if an animation clip has ended.")]
    [SerializeField] private bool _animationClipEnd;

    #endregion

    #region Debug Settings

    [Header("Debug")]
    [Tooltip("Enable debug logs in Play Mode")]
    [SerializeField] private bool _enableDebug = false;

    #endregion

    #region Runtime Properties
    private bool _isStopped;
    private bool _isGrounded = true;
    private Vector3 _currentRotation = Vector3.zero;
    private Vector3 _moveDirection;
    private bool _isJumping;
    private float _moveSpeed;
    private GaitState _currentGait;

    private bool _isAttacking;

    private bool _isBlocking;


    [Header("Speed Thresholds")]
    public float runThreshold = 1.0f;
    public float sprintThreshold = 3.0f;
    public float fallDuration;

    // Umbral mínimo para considerar "movimiento válido"
    private const float minSpeedToAnimate = 0.05f;

    #endregion

    [SerializeField] private Animator _animator;

    public void Initialize()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("[EnemyAnimations] Animator not assigned!");
            return;
        }

        _isGrounded = true;
        _animator.SetBool(_isGroundedHash, _isGrounded);
    }

    public void AnimUpdate(float deltaTime, EnemyCharacter character, MeleeAttack melee, BatAttack bat, EnemyStateHandler handler)
    {
        UpdateProperties(character, melee,bat,handler);
        UpdateAnimatorController(character);
    }

    void UpdateProperties(EnemyCharacter character, MeleeAttack melee, BatAttack bat,EnemyStateHandler handler)
    {
        if(melee != null)
        {
            _isAttacking = melee.IsAttacking;
        }
        else
        {
            _isAttacking = bat.IsAttacking;
        }
        if(handler.GetCurrentState() == handler.GetBlockState())
        {
            _isBlocking = true;
        }
        // Obtenemos el estado actual del personaje
        _isStopped = character._state.MovementState == MovementState.Idle;
        _moveDirection = character._state.Velocity;
        _isGrounded = character._state.Grounded;
        _currentRotation = character._state.Rotation.eulerAngles;
        _isJumping = character._state.Jump;
        _moveSpeed = _moveDirection.magnitude;
        fallDuration = character._timeSinceUngrounded;

        // 🔥 SOLUCIÓN CRÍTICA: Forzar animación si hay movimiento detectado
        // Aunque el estado sea Idle, si hay velocidad > umbral, animamos
        if (_moveSpeed > minSpeedToAnimate)
        {
            _isStopped = false; // Forzar que no esté detenido

            if (_moveSpeed < runThreshold)
                _currentGait = GaitState.Walk;
            else if (_moveSpeed < sprintThreshold)
                _currentGait = GaitState.Run;
            else
                _currentGait = GaitState.Sprint;
        }
        else
        {
            _currentGait = GaitState.Idle;
        }

        // Debug opcional
        if (_enableDebug && Application.isPlaying)
        {
            string debugInfo = $"[Anim] Speed: {_moveSpeed:F3}, Gait: {_currentGait}, " +
                               $"Stopped: {_isStopped}, Grounded: {_isGrounded}, " +
                               $"State: {character._state.MovementState}";
            Debug.Log(debugInfo, this);
        }
    }

    void UpdateAnimatorController(EnemyCharacter character)
    {
        if (_animator == null) return;

        // ✅ Actualizamos TODOS los parámetros existentes (evita errores)
        var a = Random.Range(0, 1);
        if(_isAttacking)
        {
            var name = a == 0 ? "Attacking1" : "Attacking2";
            _animator.SetBool(name,_isAttacking);


        }
        _animator.SetBool("Blocking", _isBlocking);
        _animator.SetBool(_isGroundedHash, _isGrounded);
        _animator.SetBool(_isStoppedHash, _isStopped);
        _animator.SetFloat(_moveSpeedHash, _moveSpeed);
        _animator.SetInteger(_currentGaitHash, (int)_currentGait);
        _animator.SetFloat(_fallingDurationHash, fallDuration);

        // Parámetros adicionales (mantenidos para compatibilidad)
        _animator.SetBool(_isJumpingAnimHash, _isJumping);
        // Los demás (strafe, lean, etc.) puedes añadirlos si los usas
    }

    public void CheckIsStopped(EnemyCharacter character)
    {
        // Puedes usar esto para forzar sync si es necesario
    }
}