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
        hp.OnLifeChangue += HandleHitEvent;
    }

    private void Awake()
    {
        if (character == null)
        {
            character = GetComponentInChildren<EnemyCharacter>();
        }
        
    }
    private void Start()
    {
        if (Target == null)
        {
            Target = GameObject.FindWithTag("Player").transform;
        }
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
        var recv = GetComponentInChildren<CombatHitReceiver>();
        if (recv != null)
        {
           recv.isBlocking = (newState == block);
        }

        isTransitioning = false;
        QueuedBlock = false;
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

     character.SetMovementMode(MovementMode.NavMesh);
        agent.isStopped = false;
        agent.speed = MoveSpeed();

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
        bool useKCC =
            newState == attack || newState == recover ||
            newState == stunned || newState == block || newState == exposed || newState == idle;
        Debug.Log($"useKCC: {useKCC} - newState: {newState}");
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
                agent.Warp(character.transform.position);; 
            }
            character.SetMovementMode(MovementMode.NavMesh);
        }
    }

    private IEnumerator Helper()
    {
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
                attackComponent.ForceCancel(false, false);
                StopMovement();
                if (currentState == attack || currentState == alert)
                    SetState(idle);
                break;
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

        if (isBlind)
        {
            StopMovement();
        }

        if (HasStatus(StatusEffect.Drunk))
        {
            drunkSwayTimer -= Time.deltaTime;
            if (drunkSwayTimer <= 0f)
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
        float speed = baseSpeed;

        if (HasStatus(StatusEffect.Bleeding))
        {
            speed *= bleedSpeedMultiplier;
            Debug.Log($"{name}: MoveSpeed reduced by bleed ({baseSpeed:F2} > {speed:F2})");
        }

        return speed;
    }

    public float EffectiveDamage(float baseDamage)
    {
        float damage = baseDamage;

        if (HasStatus(StatusEffect.Bleeding))
        {
            float reduced = damage * bleedDamageMultiplier;
            damage = reduced;
        }

        if (HasStatus(StatusEffect.Drunk))
        {
            float boosted = damage * drunkDamageMultiplier;
            damage = boosted;
        }

        return damage;
    }

    public float DamageTakeMult()
    {
        float mult = HasStatus(StatusEffect.Drunk) ? drunkWeakness : 1f;
        return mult;
    }

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
            Debug.Log("Taking bleed damage");
            suppressOnHit = true;    
            hp.TakeDamague(5);
            suppressOnHit = false;       
            yield return new WaitForSeconds(1.0f);
        }
    }
    private IEnumerator DrunkWobble() //wip
    {
        while (HasStatus(StatusEffect.Drunk))
        {
            Vector3 sway = new Vector3(0, UnityEngine.Random.Range(-15f, 15f), 0);
            character.transform.Rotate(sway * Time.deltaTime);
            yield return new WaitForSeconds(0.2f);
        }
    }

    //----------------------DAMAGE------------------------
    private void HandleHitEvent(float delta)
    {
        if (delta >= 0f) return;
        if (suppressOnHit) return;

        Vector3 hitDir = Vector3.zero;
        if (Target != null)
            hitDir = (transform.position - Target.position).normalized;

        OnHit(hitDir);
    }
    public void OnHit(Vector3 hitDir)
    {
        Debug.Log($"{name}: OnHit triggered in state {currentState?.GetType().Name}. Direction: {hitDir}");

        if (currentState == attack)
        {
            if (attackComponent != null && attackComponent.TryInterrupt())
            {
                Debug.Log($"{name}: Attack interrupted = switching to stun state");
                SetState(stunned);
                Knockback(hitDir);
                return;
            }

            Debug.Log($"{name}: Attack not interruptible = queuing block.");
            QueuedBlock = true;
            attackComponent.ForceCancel(false, true);
            return;
        }

        else if (currentState == block)
        {
            Debug.Log($"{name}: Extending block duration.");
            block.ExtendBlock();
            Knockback(hitDir);
            return;
        }

        else if (currentState == exposed || currentState == recover)
        {
            Debug.Log($"{name}: Hit while exposed/recovering = stunned.");
            SetState(stunned);
            Knockback(hitDir);
            return;
        }
        if (currentState == stunned)
        {
            Debug.Log($"{name}: already stunned.");
            stunned.ExtendStun(0.8f);
            Knockback(hitDir);
            return;
        }

        Debug.Log($"{name}: Regular hit knockback applied.");
        Knockback(hitDir);
    }

    private void OnDisable()
    {
        if (TryGetComponent(out HealthController hp))
            hp.OnLifeChangue -= HandleHitEvent;
    }
}