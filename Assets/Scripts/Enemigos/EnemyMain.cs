using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyMain : MonoBehaviour
{
    public float detectionRange = 10f;
    public float attackRange = 5f;
    public float moveSpeed = 3f;
    public float turnSpeed = 8f;
    public Transform target;

    public IEnemyState currentState;
    bool isTransitioning;

    public float stunDuration = 2f;
    public float knockbackForce = 5f;

    private IdleState idle;
    private AlertState alert;
    private AttackState attack;
    private DeadState dead;
    private StunState stunned;
    private BlockState block;
    private ExposedState exposed;
    private RecoverState recover;

    public Rigidbody rb;
    public Renderer rend;
    protected NavMeshAgent agent;

    private Dictionary<StatusEffect, float> activeEffects = new Dictionary<StatusEffect, float>();
    public float bleedSpeedMultiplier = 0.7f;
    public float bleedDamageMultiplier = 0.8f;
    public float drunkDamageMultiplier = 1.5f;
    public float drunkWeakness = 1.3f;
    public bool isBlind = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.stoppingDistance = attackRange - 3f;

        idle = new IdleState(this);
        alert = new AlertState(this);
        attack = new AttackState(this);
        dead = new DeadState(this);
        stunned = new StunState(this);
        block = new BlockState(this);
        exposed = new ExposedState(this);
        recover = new RecoverState(this);

        SetState(idle);

    }

    void Update()
    {
        agent.speed = MoveSpeed();
        currentState?.Update();
        StatusTimers();
        HandleFacing();
    }

    public void SetState(IEnemyState newState)
    {
        if (isTransitioning || newState == currentState) return;

        isTransitioning = true;

        currentState?.OnExit();
        currentState = newState;
        currentState?.OnEnter();
        UpdateColor();

        isTransitioning = false;
    }

    public IEnemyState GetCurrentState() => currentState;
    public IEnemyState GetIdleState() => idle;
    public IEnemyState GetAlertState() => alert;
    public IEnemyState GetAttackState() => attack;
    public IEnemyState GetDeadState() => dead;
    public IEnemyState GetStunState() => stunned;
    public IEnemyState GetBlockState() => block;
    public IEnemyState GetExposedState() => exposed;
    public IEnemyState GetRecoverState() => recover;

    public bool Watching()
    {
        if (target == null)
        {
            return false;
        }
        return Vector3.Distance(transform.position, target.position) <= detectionRange;
    }

    public bool Attacking()
    {
        if (target == null)
        {
            return false;
        }
        return Vector3.Distance(transform.position, target.position) <= attackRange;
    }

    public void StopMovement()
    {
        if (agent != null)
        {
            agent.isStopped = true;
        }
        rb.linearVelocity = Vector3.zero;
    }

    public void MoveTowardsTarget()
    {
        if (agent == null || target == null) return;
        agent.isStopped = false;
        agent.speed = MoveSpeed();
        agent.SetDestination(target.position);
    }

    public abstract void Movement();

    public void Knockback(Vector3 hitDirection)
    {
        StopMovement();
        hitDirection.y = 0f;
        rb.AddForce(hitDirection.normalized * knockbackForce, ForceMode.Impulse);
    }

    private void UpdateColor()
    {
        if (rend == null) return;

        if (currentState == idle) rend.material.color = Color.gray;
        else if (currentState == alert) rend.material.color = Color.yellow;
        else if (currentState == stunned) rend.material.color = Color.cyan;
        else if (currentState == block) rend.material.color = Color.blue;
        else if (currentState == exposed) rend.material.color = Color.magenta;
        else if (currentState == recover) rend.material.color = new Color(1f, 0.5f, 0f); // orange
        else if (currentState == dead) rend.material.color = Color.black;
        else if (currentState == attack)
        {
            if (GetComponent<IEnemyAttack>()?.IsAttacking == true)
            {
                rend.material.color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time * 5, 1));
            }

            else
            {
                rend.material.color = Color.red;
            }

        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

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
        {
            activeEffects.Remove(type);
        }


        if (type == StatusEffect.Blind)
        {
            isBlind = false;
        }
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
            {
                expired.Add(key);
            }
        }

        foreach (var e in expired)
        {
            RemoveStatus(e);
        }
    }

    public float MoveSpeed()
    {
        float speed = moveSpeed;

        if (HasStatus(StatusEffect.Bleeding))
        {
            speed *= bleedSpeedMultiplier;
        }    
        return speed;
    }

    public float EffectiveDamage(float baseDamage)
    {
        float damage = baseDamage;

        if (HasStatus(StatusEffect.Bleeding))
        {
            damage *= bleedDamageMultiplier;
        }


        if (HasStatus(StatusEffect.Drunk))
        {
            damage *= drunkDamageMultiplier;
        }


        return damage;
    }

    public float DamageTakeMult()
    {
        return HasStatus(StatusEffect.Drunk) ? drunkWeakness : 1f;
    }

    private void HandleFacing()
    {
        if (target == null || isBlind) return;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= detectionRange)
        {
            Vector3 dir = (target.position - transform.position);
            dir.y = 0f; 
            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion lookRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * turnSpeed);
            }
        }
    }
}
