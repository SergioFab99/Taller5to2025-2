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
    private Transform Character;

    [Header("Behaviour / Settings")]
    public EnemyBehaviourState EnemyBehaviourState = EnemyBehaviourState.Default;
    [NonSerialized] public EnemySettingsList enemySettings;
    [NonSerialized] public MeleeAttack attackComponent;

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

    [NonSerialized] public NavMeshAgent agent;
    [NonSerialized] public EnemyCharacter character;

    private Vector3 _lastCornerPos;
    private float _cornerTimer;
    

    public void Initialize(EnemySettingsList settings, Transform characterTransform, EnemyCharacter charac, NavMeshAgent agent1, MeleeAttack attacc)
    {
        Character = characterTransform;
        enemySettings = settings;
        attackComponent = attacc;
        agent = agent1;
        character = charac;

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
                    agent.SetDestination(Target.position);
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

        isTransitioning = true;
        currentState?.OnExit();
        currentState = newState;
        StateMovement(newState);
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
        SetBehaviourState(EnemyBehaviourState.Combat);
    }

    public void ExitCombatMode()
    {
        SetBehaviourState(EnemyBehaviourState.Default);
    }


    public void StopMovement()
    {
        if (agent.enabled && agent.isOnNavMesh)
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

        agent.isStopped = false;
        agent.speed = MoveSpeed();

        if (!agent.hasPath || Vector3.Distance(agent.destination, Target.position) > 0.25f)
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

    public void StateMovement(IEnemyState newState)
    {
        bool useKCC =
            newState == attack || newState == recover ||
            newState == stunned || newState == block || newState == exposed;

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
                agent.isStopped = false;
                agent.Warp(character.transform.position); 
            }
            character.SetMovementMode(MovementMode.NavMesh);
        }
    }

    private IEnumerator Helper()
    {
        //syncs character to next frame
        yield return null; 
        agent.nextPosition = character.transform.position;
    }

    public void HandleFacing()
    {
        if (Target == null || isBlind) return;

        if (character.CurrentMode != MovementMode.NavMesh) return;
        if (agent != null && agent.enabled && agent.updateRotation) return; 

        float dist = Vector3.Distance(character.transform.position, Target.position);
        if (dist <= detectionRange)
        {
            Vector3 dir = Target.position - character.transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion lookRot = Quaternion.LookRotation(dir);
                character.transform.rotation = Quaternion.Lerp(
                    character.transform.rotation, lookRot, Time.deltaTime * turnSpeed);
            }
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
        float speed = enemySettings.AlertEnemySettings.moveSettings.Speed;
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