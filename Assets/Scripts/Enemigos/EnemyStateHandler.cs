using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class EnemyStateHandler : MonoBehaviour
{
    [Header("Target / Character Refs")]
    public Transform Target;
    public Transform TargetPlayer;
    public Transform TargetPatrol;
    private Transform Character;

    [Header("Behaviour / Settings")]
    public EnemyBehaviourState EnemyBehaviourState = EnemyBehaviourState.Default;
    [NonSerialized] public EnemySettingsList enemySettings;
    [NonSerialized] public IEnemyAttack attackComponent;

    private IEnemyState currentState;
    private bool isTransitioning;
    public bool QueuedBlock { get; set; }

    [Header("States")]
    private IdleState1 idle;
    private AlertState1 alert;
    private AttackState1 attack;
    private RecoverState1 recover;
    private StunState1 stunned;
    private BlockState1 block;
    private ExposedState1 exposed;
    private DeadState1 dead;

    [Header("Movement")]
    public float detectionRange = 10f;
    public float attackRange = 5f;
    [NonSerialized] public float nextAttackTime = 0f;
    public float moveSpeed = 3f;
    public float turnSpeed = 8f;
    public float knockbackForce = 5f;
    public float stunDuration = 2f;
    private float drunkSwayTimer = 0f;
    public Vector3 drunkRotationOffset = Vector3.zero;
    [NonSerialized] public NavMeshAgent agent;
    public EnemyCharacter character;
    public float attackTagCooldownEndTime = 0f;
    [NonSerialized] public float lastAttackTime = -999f;
    public bool lockDirectChase = false;

    private Vector3 _lastCornerPos;
    private float _cornerTimer;

    [Header("Status Effects")]
    private Dictionary<StatusEffect, float> activeEffects = new Dictionary<StatusEffect, float>();
    public float bleedSpeedMultiplier = 0.7f;
    public float bleedDamageMultiplier = 0.8f;
    public float drunkDamageMultiplier = 1.5f;
    public float drunkWeakness = 1.3f;
    public bool isBlind = false;
    [NonSerialized] public bool suppressOnHit = false;

    public void Initialize(EnemySettingsList settings, Transform characterTransform, EnemyCharacter charac, NavMeshAgent agent1)
    {
        Character = characterTransform;
        enemySettings = settings;
        agent = agent1;
        character = charac;
        attackComponent = GetComponent<IEnemyAttack>();

        idle = new IdleState1(this);
        alert = new AlertState1(this);
        attack = new AttackState1(this);
        recover = new RecoverState1(this);
        stunned = new StunState1(this);
        block = new BlockState1(this);
        exposed = new ExposedState1(this);
        dead = new DeadState1(this);

        SetState(idle);

        var hp = GetComponentInChildren<HealthController>();
        if (hp != null)
            hp.OnLifeChangue += HandleHitEvent;
    }

    private void Awake()
    {
        if (character == null)
            character = GetComponentInChildren<EnemyCharacter>();

        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = true;
        }
    }

    private void Start()
    {
        if (Target == null)
            Target = GameObject.FindWithTag("Player")?.transform;

        if (EnemyAttackOrder.Instance != null)
        {
            EnemyAttackOrder.Instance.RegisterEnemy(this);
            Debug.Log($"{name} registered to attack order.");
        }

        if (TargetPlayer == null)
            TargetPlayer = GameObject.FindWithTag("Player")?.transform;
    }

    public void CurrentStateUpdate()
    {
        currentState?.Update();
        StatusTimers();
        HandleFacing();

        Debug.DrawLine(transform.position, transform.position + transform.forward * 2f,
            (character.CurrentMode == MovementMode.KCC) ? Color.red : Color.green);
    }

    private void LateUpdate()
    {
        if (character.CurrentMode == MovementMode.NavMesh && agent != null && agent.enabled && agent.hasPath)
        {
            if (Vector3.Distance(character.transform.position, _lastCornerPos) < 0.05f)
            {
                _cornerTimer += Time.deltaTime;
                if (_cornerTimer > 1f)
                {
                    agent.ResetPath();

                    Vector3 dest;
                    if (EnemyAttackOrder.Instance != null &&
                        EnemyAttackOrder.Instance.TryGetFormationDestination(this, out dest))
                    {
                        agent.SetDestination(dest);
                    }
                    else if (Target != null)
                    {
                        agent.SetDestination(Target.position);
                    }

                    _cornerTimer = 0f;
                }
            }
            else
            {
                _cornerTimer = 0f;
                _lastCornerPos = character.transform.position;
            }
        }
    }

    public void SetState(IEnemyState newState)
    {
        if (isTransitioning || newState == currentState) return;

        lockDirectChase = (newState == alert || newState == recover || newState == block);
        isTransitioning = true;
        currentState?.OnExit();
        currentState = newState;
        StateMovement(newState);
        currentState?.OnEnter();

        var recv = GetComponentInChildren<CombatHitReceiver>();
        if (recv != null)
            recv.isBlocking = (newState == block);

        isTransitioning = false;
        QueuedBlock = false;
    }

    public EnemyBehaviourState GetBehaviourState() => EnemyBehaviourState;
    public void SetBehaviourState(EnemyBehaviourState newBehaviour) => EnemyBehaviourState = newBehaviour;
    public IEnemyState GetCurrentState() => currentState;
    public IEnemyState GetIdleState() => idle;
    public IEnemyState GetAlertState() => alert;
    public IEnemyState GetAttackState() => attack;
    public IEnemyState GetRecoverState() => recover;
    public IEnemyState GetStunState() => stunned;
    public IEnemyState GetBlockState() => block;
    public IEnemyState GetExposedState() => exposed;
    public IEnemyState GetDeadState() => dead;

    public bool CheckTargetOnView(Transform target = null)
    {
        target ??= TargetPlayer;
        if (target == null) return false;
        return Vector3.Distance(Character.position, target.position) <= enemySettings.AISettings.detectionDistance;
    }

    public bool CheckTargetOnAttackRange(Transform target = null)
    {
        target ??= TargetPlayer;
        if (target == null) return false;
        return Vector3.Distance(Character.position, target.position) <= enemySettings.AISettings.attackRange;
    }

    public void EnterCombatMode() => SetBehaviourState(EnemyBehaviourState.Combat);
    public void ExitCombatMode() => SetBehaviourState(EnemyBehaviourState.Default);

    public void StopMovement()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    public void MoveTowardsTarget()
    {
        if (Target == null || agent == null) return;
        if (!agent.enabled || !agent.isOnNavMesh) return;
        if (lockDirectChase) return;

        character.SetMovementMode(MovementMode.NavMesh);
        agent.isStopped = false;
        agent.speed = MoveSpeed();

        Vector3 direction = (Target.position - character.transform.position).normalized;
        direction.y = 0f;

        float distance = Vector3.Distance(character.transform.position, Target.position);
        float stopDistance = enemySettings.AISettings.stopingDistance;
        Vector3 moveInput = (distance > stopDistance + 0.5f) ? direction : Vector3.zero;

        var enemyInput = new EnemyInput
        {
            Direction = direction,
            Move = moveInput
        };
        character.UpdateInputs(enemyInput, EnemyBehaviourState);
        character._state.Velocity = moveInput;
        character._state.MovementState = moveInput.magnitude > 0.01f ? MovementState.Moving : MovementState.Idle;

        if (!agent.hasPath || Vector3.Distance(agent.destination, Target.position) > 0.25f)
            agent.SetDestination(Target.position);
    }

    public void Knockback(Vector3 hitDirection, float forceMultiplier = 1f)
    {
        Debug.Log($"{name}: Knockback called in state {currentState?.GetType().Name}. Direction: {hitDirection}, force: {knockbackForce * forceMultiplier}");

        if (currentState == block)
            forceMultiplier *= 0.8f;

        StopMovement();
        hitDirection.y = 0f;

        Vector3 force = hitDirection.normalized * (knockbackForce * forceMultiplier);
        Debug.Log($"{name}: Adding external force {force}");
        character.AddExternalForce(force);
    }

    public void StateMovement(IEnemyState newState)
    {
        bool useKCC = newState is AttackState1 or RecoverState1 or StunState1 or BlockState1 or ExposedState1;

        if (useKCC)
        {
            if (agent != null && agent.enabled)
            {
                agent.updatePosition = false;
                agent.updateRotation = false;
                agent.velocity = Vector3.zero;
                agent.isStopped = true;
            }

            character.StartCoroutine(Helper());
            character.Motor.SetPosition(character.transform.position);
            character.Motor.ForceUnground();
            character.SetMovementMode(MovementMode.KCC);
        }
        else
        {
            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.updatePosition = true;
                agent.updateRotation = true;
                agent.updateUpAxis = true;
                agent.isStopped = false;
                agent.Warp(character.transform.position);
            }
            character.SetMovementMode(MovementMode.NavMesh);
        }
    }

    private IEnumerator Helper()
    {
        yield return null;
        if (agent != null)
            agent.nextPosition = character.transform.position;
    }

    public void HandleFacing()
    {
        if (Target == null) return;
        if (character.CurrentMode != MovementMode.NavMesh) return;

        Vector3 toPlayer = Target.position - character.transform.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude < 0.01f) return;

        Quaternion desired = Quaternion.LookRotation(toPlayer.normalized, Vector3.up);
        character.transform.rotation = Quaternion.Slerp(
            character.transform.rotation,
            desired,
            Time.deltaTime * turnSpeed
        );
    }

    public Vector3 GetSafeRetreatDirection()
    {
        if (Target == null)
            return -character.transform.forward;

        Vector3 toPlayer = Target.position - character.transform.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude < 0.01f)
            return -character.transform.forward;

        Vector3 facingToPlayer = toPlayer.normalized;
        Vector3 retreat = -facingToPlayer;

        Vector3 tangent = Vector3.Cross(Vector3.up, facingToPlayer).normalized;
        float sideSign = UnityEngine.Random.value > 0.5f ? 1f : -1f;
        retreat += tangent * sideSign * 0.35f;

        var eao = EnemyAttackOrder.Instance;
        if (eao != null)
        {
            foreach (var attacker in eao.LockedAttackers)
            {
                if (attacker == null || attacker == this) continue;

                float dist = Vector3.Distance(attacker.character.transform.position, character.transform.position);
                if (dist < 1.2f)
                {
                    Vector3 away = (character.transform.position - attacker.character.transform.position).normalized;
                    retreat += away * 0.75f;
                }
            }
        }

        retreat.y = 0f;
        if (retreat.sqrMagnitude < 0.001f)
            retreat = -facingToPlayer;

        return retreat.normalized;
    }

    public void ApplyStatus(StatusEffect type, float duration)
    {
        Debug.Log($"{name} ApplyStatus called: {type} for {duration}s");
        activeEffects[type] = duration;

        switch (type)
        {
            case StatusEffect.Bleeding:
                StopCoroutine(nameof(BleedTick));
                StartCoroutine(BleedTick());
                break;
            case StatusEffect.Drunk:
                StartCoroutine(DrunkWobble());
                break;
            case StatusEffect.Blind:
                isBlind = true;
                attackComponent?.ForceCancel(false, false);
                StopMovement();
                if (currentState is AlertState1 or IdleState1) return;
                SetState(idle);
                break;
        }
    }

    public void RemoveStatus(StatusEffect type)
    {
        activeEffects.Remove(type);
        if (type == StatusEffect.Blind)
            isBlind = false;
    }

    public bool HasStatus(StatusEffect type) => activeEffects.ContainsKey(type);

    public void StatusTimers()
    {
        var keys = new List<StatusEffect>(activeEffects.Keys);
        var expired = new List<StatusEffect>();

        foreach (var key in keys)
        {
            if ((activeEffects[key] -= Time.deltaTime) <= 0)
                expired.Add(key);
        }

        expired.ForEach(RemoveStatus);

        if (isBlind)
            StopMovement();

        if (HasStatus(StatusEffect.Drunk))
        {
            if ((drunkSwayTimer -= Time.deltaTime) <= 0f)
            {
                drunkSwayTimer = UnityEngine.Random.Range(0.3f, 0.6f);
                drunkRotationOffset = new Vector3(0f, UnityEngine.Random.Range(-20f, 20f), 0f);
            }
        }
        else
        {
            drunkRotationOffset = Vector3.zero;
        }
    }

    public float MoveSpeed()
    {
        float baseSpeed = enemySettings.AlertEnemySettings.moveSettings.Speed;
        return HasStatus(StatusEffect.Bleeding) ? baseSpeed * bleedSpeedMultiplier : baseSpeed;
    }

    public float EffectiveDamage(float baseDamage)
    {
        float damage = baseDamage;
        if (HasStatus(StatusEffect.Bleeding)) damage *= bleedDamageMultiplier;
        if (HasStatus(StatusEffect.Drunk)) damage *= drunkDamageMultiplier;
        return damage;
    }

    public float DamageTakeMult() => HasStatus(StatusEffect.Drunk) ? drunkWeakness : 1f;

    private void OnDrawGizmosSelected()
    {
        if (enemySettings == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemySettings.AISettings.detectionDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemySettings.AISettings.attackRange);
    }

    private IEnumerator BleedTick()
    {
        var hp = GetComponentInChildren<HealthController>();
        if (hp == null) yield break;

        while (HasStatus(StatusEffect.Bleeding))
        {
            suppressOnHit = true;
            hp.TakeDamague(5);
            suppressOnHit = false;
            yield return new WaitForSeconds(1.0f);
        }
    }

    private IEnumerator DrunkWobble()
    {
        while (HasStatus(StatusEffect.Drunk))
        {
            Vector3 sway = new Vector3(0, UnityEngine.Random.Range(-15f, 15f), 0);
            character.transform.Rotate(sway * Time.deltaTime);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void HandleHitEvent(float delta)
    {
        if (delta >= 0f || suppressOnHit || Target == null) return;
        OnHit((transform.position - Target.position).normalized);
    }

    public void OnHit(Vector3 hitDir)
    {
        Debug.Log($"{name}: OnHit triggered in state {currentState?.GetType().Name}. Direction: {hitDir}");

        switch (currentState)
        {
            case AttackState1:
                if (attackComponent?.TryInterrupt() == true)
                {
                    Debug.Log($"{name}: Attack interrupted → stunned.");
                    SetState(stunned);
                    Knockback(hitDir);
                    return;
                }
                Debug.Log($"{name}: Attack not interruptible → queuing block.");
                QueuedBlock = true;
                attackComponent?.ForceCancel(false, true);
                return;

            case BlockState1:
                Debug.Log($"{name}: Extending block.");
                block.ExtendBlock();
                Knockback(hitDir);
                return;

            case ExposedState1 or RecoverState1:
                Debug.Log($"{name}: Hit while exposed/recovering → stunned.");
                SetState(stunned);
                Knockback(hitDir);
                return;

            case StunState1:
                Debug.Log($"{name}: Already stunned → extending.");
                stunned.ExtendStun(0.8f);
                Knockback(hitDir);
                return;

            default:
                Debug.Log($"{name}: Regular hit → knockback.");
                Knockback(hitDir);
                break;
        }
    }

    private void OnDisable()
    {
        if (TryGetComponent(out HealthController hp))
            hp.OnLifeChangue -= HandleHitEvent;
    }
}