using UnityEngine;
using UnityEngine.AI;

public class EnemyMain : MonoBehaviour
{
    public float detectionRange = 10f;
    public float attackRange = 5f;
    public float moveSpeed = 3f;
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
    private NavMeshAgent agent;
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
        agent.speed = moveSpeed;
        currentState?.Update();
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
        if (agent != null) agent.isStopped = true;
        rb.linearVelocity = Vector3.zero;
    }

    public void MoveTowardsTarget()
    {
        if (agent == null || target == null) return;
        agent.isStopped = false;
        agent.SetDestination(target.position);
    }

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
}
