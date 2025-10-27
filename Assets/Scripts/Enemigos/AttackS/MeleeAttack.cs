using UnityEngine;

public class MeleeAttack : MonoBehaviour, IEnemyAttack
{
    public float attackRange = 2f;

    public float windupTime = 0.4f;
    public float punchActiveTime = 0.2f;
    public float comboGap = 0.3f;
    public int maxCombo = 2;

    private EnemyStateHandler handler;
    private EnemyMain ai;
    [SerializeField] private Transform characterTransform;
    private int currentPunch = 0;

    private bool isAttacking = false;
    private bool finished = false;
    private bool interrupted = false;
    public bool Missed { get; private set; }
    public bool ForceBlocked { get; private set; }

    public float AttackRange => attackRange;
    public bool IsAttacking => isAttacking;
    public bool IsFinished => finished;
    public bool WasInterrupted => interrupted;

    private enum AttackPhase { None, Windup, Active, Recovery }
    [SerializeField] private AttackPhase currentPhase = AttackPhase.None;

    void Awake()
    {
        handler = GetComponent<EnemyStateHandler>();
        ai = GetComponent<EnemyMain>();

        if (handler != null)
            characterTransform = handler.GetComponentInChildren<EnemyCharacter>().transform;
    }

    public void Execute()
    {
        Transform target = handler.Target;
        if (target == null) return;
        if (isAttacking || (!finished && currentPunch > 0))
            return;

        if (currentPhase != AttackPhase.None)
            return;

        float dist = Vector3.Distance(characterTransform.position, handler.Target.position);
        if (dist > attackRange) return;

        currentPunch = 0;
        finished = false;
        interrupted = false;

        Windup();
    }

    private void Windup()
    {
        if (currentPunch >= maxCombo)
        {
            finished = true;
            return;
        }

        currentPhase = AttackPhase.Windup;
        isAttacking = true;
        currentPunch++;

        if (handler != null) handler.StopMovement();
        if (ai != null) ai.StopMovement();
        Debug.Log($"windup for punch {currentPunch}");

        Invoke(nameof(PerformPunch), windupTime);
    }

    private void PerformPunch()
    {
        Debug.Log($"punch {currentPunch}");
        currentPhase = AttackPhase.Active;

        Collider[] hits = Physics.OverlapSphere(characterTransform.position, attackRange);
        bool hitLanded = false;
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log($"punch {currentPunch} hit");
                if (hit.TryGetComponent(out HealthController hp))
                {
                    hp.TakeDamague(10);
                }
                hitLanded = true;
                Missed = false;
                break;
            }
        }

        if (!hitLanded)
        {
            Debug.Log("or miss, i guess they never miss huh");
            Missed = true;
            ForceCancel(false, false);
        }

        Invoke(nameof(EndPunch), punchActiveTime);
    }

    private void EndPunch()
    {
        currentPhase = AttackPhase.Recovery;

        if(handler.QueuedBlock)
        {
            ForceCancel(false, true);
            
        }
        if (currentPunch < maxCombo && !interrupted)
        {
            isAttacking = true; 
            Invoke(nameof(Windup), comboGap);
        }
        else
        {
            isAttacking = false;
            finished = true;
        }
    }

    public void ForceCancel(bool interrupt, bool block)
    {
        CancelInvoke();
        StopAllCoroutines();
        isAttacking = false;
        finished = true;
        interrupted = interrupt;
        ForceBlocked = block; 
        currentPhase = AttackPhase.None;
    }

    public void ResetAttackCycle()
    {
        currentPunch = 0;
        finished = false;
        interrupted = false;
        ForceBlocked = false;
        isAttacking = false;
        currentPhase = AttackPhase.None;
    }

    public bool TryInterrupt()
    {
        if (isAttacking && currentPhase == AttackPhase.Windup)
        {
            Debug.Log($"{name} attack interrupted during windup");
            ForceCancel(true, false);
            return true;
        }
        return false;
    }
}