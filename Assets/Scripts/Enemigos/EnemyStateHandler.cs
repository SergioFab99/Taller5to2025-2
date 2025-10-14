using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyStateHandler : MonoBehaviour
{
    [Header("Target / Character Refs")]
    public Transform Target;
    private Transform Character;

    [Header("Behaviour / Settings")]
    public EnemyBehaviourState EnemyBehaviourState = EnemyBehaviourState.Default;
    [NonSerialized] public EnemySettingsList enemySettings;
    [NonSerialized] public IEnemyAttack attackComponent;

    private IEnemyState currentState;
    private bool isTransitioning;


    private IdleState1 idle;
    private AlertState1 alert;
    private AttackState1 attack;
    private RecoverState1 recover;
    private StunState1 stunned;
    private BlockState1 block;
    private ExposedState1 exposed;
    private DeadState1 dead;

    public float detectionRange = 10f;
    public float attackRange = 5f;
    public float moveSpeed = 3f;
    public float turnSpeed = 8f;
    public float knockbackForce = 5f;
    public float stunDuration = 2f;


    private Dictionary<StatusEffect, float> activeEffects = new Dictionary<StatusEffect, float>();
    public float bleedSpeedMultiplier = 0.7f;
    public float bleedDamageMultiplier = 0.8f;
    public float drunkDamageMultiplier = 1.5f;
    public float drunkWeakness = 1.3f;
    public bool isBlind = false;

    private NavMeshAgent agent;
    [NonSerialized] public EnemyCharacter character;


    public void Initialize(EnemySettingsList settings, Transform characterTransform)
    {
        Character = characterTransform;
        enemySettings = settings;
        attackComponent = GetComponent<IEnemyAttack>();
        agent = GetComponent<NavMeshAgent>();
        character = GetComponent<EnemyCharacter>();

        idle = new IdleState1(this);
        alert = new AlertState1(this);
        attack = new AttackState1(this);
        recover = new RecoverState1(this);
        stunned = new StunState1(this);
        block = new BlockState1(this);
        exposed = new ExposedState1(this);
        dead = new DeadState1(this);

        SetState(idle);
    }

    public void CurrentStateUpdate()
    {
        currentState?.Update();
        StatusTimers();
        HandleFacing();

        Debug.DrawLine(transform.position, transform.position + transform.forward * 2f, (character.CurrentMode == MovementMode.KCC) ? Color.red : Color.green);
    }

    void LateUpdate()
    {

        if (agent != null && agent.enabled && character.Motor.enabled)
            Debug.LogWarning($"{name}: Both NavMesh and KCC active at once!");

        var pos = transform.position;
        if (Vector3.Distance(pos, _lastPos) > 0.05f)
            Debug.Log($"[{Time.frameCount}] {name} moved to {pos}");
        _lastPos = pos;
    }
    private Vector3 _lastPos;

    public void SetState(IEnemyState newState)
    {
        if (isTransitioning || newState == currentState) return;

        isTransitioning = true;
        currentState?.OnExit();
        currentState = newState;
        currentState?.OnEnter();
        isTransitioning = false;
    }

    // ------------------- HELPERS -------------------
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

    // ------------------- VISIBILITY & RANGE CHECKS -------------------
    public bool CheckTargetOnView(Transform target = null)
    {
        if (target == null) target = Target;
        if (target == null) return false;
        return Vector3.Distance(Character.position, target.position)
               <= enemySettings.AISettings.detectionDistance;
    }

    public bool CheckTargetOnAttackRange(Transform target = null)
    {
        if (target == null) target = Target;
        if (target == null) return false;
        return Vector3.Distance(Character.position, target.position)
               <= enemySettings.AISettings.attackRange;
    }

    // ------------------- MOVEMENT -------------------
    public void EnterCombatMode()
    {
        SyncAgentToTransform();
        Debug.Log($"{name} agent synced. nextPos={agent.nextPosition} current={transform.position}");

        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.enabled = false;
        }
        Debug.Log($"Motor pos before enable: {character.Motor.TransientPosition}");
        if (character != null)
        {
            var motor = character.Motor;
            if (motor != null)
            {
                motor.SetPosition(transform.position);   
                motor.SetRotation(transform.rotation);
            }

            character.SetMovementMode(MovementMode.KCC);
        }

        SetBehaviourState(EnemyBehaviourState.Combat);
    }

    public void ExitCombatMode()
    {
        if (character != null)
            character.SetMovementMode(MovementMode.NavMesh);

        if (agent != null)
        {
            if (!agent.isOnNavMesh)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
                    transform.position = hit.position;
            }

            agent.enabled = true;
            agent.Warp(transform.position);  
            agent.updatePosition = true;
            agent.updateRotation = true;
            agent.isStopped = false;
        }

        SetBehaviourState(EnemyBehaviourState.Default);
    }


    public void StopMovement()
    {
        if (agent != null) agent.isStopped = true;
    }

    public void MoveTowardsTarget()
    {
        if (agent == null || Target == null) return;
        if (!agent.enabled) return; 

        agent.isStopped = false;
        agent.speed = MoveSpeed();
        agent.SetDestination(Target.position);
    }

    public void Knockback(Vector3 hitDirection)
    {
        StopMovement();
        hitDirection.y = 0f;

        if (TryGetComponent(out EnemyCharacter character))
        {
            character.AddExternalForce(hitDirection.normalized * knockbackForce);
        }
    }

    // ------------------- STATUS -------------------
    public void ApplyStatus(StatusEffect type, float duration)
    {
        activeEffects[type] = duration;
        if (type == StatusEffect.Blind)
        {
            isBlind = true;
            StopMovement();
        }
    }

    public void RemoveStatus(StatusEffect type)
    {
        if (activeEffects.ContainsKey(type))
            activeEffects.Remove(type);

        if (type == StatusEffect.Blind)
            isBlind = false;
    }

    public bool HasStatus(StatusEffect type) => activeEffects.ContainsKey(type);

    public void StatusTimers()
    {
        var keys = new List<StatusEffect>(activeEffects.Keys);
        List<StatusEffect> expired = new List<StatusEffect>();

        foreach (var key in keys)
        {
            activeEffects[key] -= Time.deltaTime;
            if (activeEffects[key] <= 0)
                expired.Add(key);
        }

        foreach (var e in expired)
            RemoveStatus(e);
    }

    public float MoveSpeed()
    {
        float speed = moveSpeed;
        if (HasStatus(StatusEffect.Bleeding))
            speed *= bleedSpeedMultiplier;
        return speed;
    }

    public float EffectiveDamage(float baseDamage)
    {
        float damage = baseDamage;
        if (HasStatus(StatusEffect.Bleeding))
            damage *= bleedDamageMultiplier;
        if (HasStatus(StatusEffect.Drunk))
            damage *= drunkDamageMultiplier;
        return damage;
    }

    public float DamageTakeMult()
    {
        return HasStatus(StatusEffect.Drunk) ? drunkWeakness : 1f;
    }

    public void HandleFacing()
    {
        if (Target == null || isBlind) return;
        float dist = Vector3.Distance(transform.position, Target.position);
        if (dist <= detectionRange)
        {
            Vector3 dir = (Target.position - transform.position);
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion lookRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * turnSpeed);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (enemySettings == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemySettings.AISettings.detectionDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemySettings.AISettings.attackRange);
    }
    private void SyncAgentToTransform()
    {
        if (agent == null || !agent.enabled) return;
        if (!agent.isOnNavMesh) return;

        agent.nextPosition = transform.position;
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.Warp(transform.position);
    }
}