using UnityEngine;

public class EnemyMain : MonoBehaviour
{
    public float detectionRange = 10f;
    public float attackRange = 5f;
    public float moveSpeed = 3f;
    public Transform target;

    private IEnemyState currentState;

    public float stunDuration = 2f;
    public float knockbackForce = 5f;

    private IdleState idle;
    private AlertState alert;
    private AttackState attack;
    private DeadState dead;
    private StunState stunned;

    public Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        idle = new IdleState(this);
        alert = new AlertState(this);
        attack = new AttackState(this);
        dead = new DeadState(this);
        stunned = new StunState(this);

        SetState(idle);

    }

    void Update()
    {
        currentState?.Update();
    }

    public void SetState(IEnemyState newState)
    {
        currentState?.OnExit();
        currentState = newState;
        currentState?.OnEnter();
    }

    public IEnemyState GetIdleState() => idle;
    public IEnemyState GetAlertState() => alert;
    public IEnemyState GetAttackState() => attack;
    public IEnemyState GetDeadState() => dead;
    public IEnemyState GetStunState() => stunned;

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
        rb.linearVelocity = Vector3.zero;
    }

    public void Knockback(Vector3 hitDirection)
    {
        hitDirection.y = 0f;
        rb.AddForce(hitDirection.normalized * knockbackForce, ForceMode.Impulse);
    }
}
