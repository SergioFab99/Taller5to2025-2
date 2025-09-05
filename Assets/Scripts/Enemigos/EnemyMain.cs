using UnityEngine;

public class EnemyMain : MonoBehaviour
{
    public float detectionRange = 10f;
    public float attackRange = 5f;
    public float moveSpeed = 3f;
    public Transform target;

    private IEnemyState currentState;

    private IdleState idle;
    private AlertState alert;
    private AttackState attack;
    private DeadState dead;

    void Start()
    {
        idle = new IdleState(this);
        alert = new AlertState(this);
        attack = new AttackState(this);
        dead = new DeadState(this);

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
    public IEnemyState GetEngagingState() => attack;
    public IEnemyState GetDeadState() => dead;

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
}
