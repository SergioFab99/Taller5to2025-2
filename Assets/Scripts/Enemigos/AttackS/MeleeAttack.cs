using UnityEngine;

public class MeleeAttack : MonoBehaviour, IEnemyAttack
{
    public float attackRange = 2f;

    public float windupTime = 0.4f;
    public float punchActiveTime = 0.2f;
    public float comboGap = 0.3f;
    public int maxCombo = 2;

    private EnemyMain ai;
    private int currentPunch = 0;

    private bool isAttacking = false;
    private bool finished = false;
    private bool interrupted = false;

    public float AttackRange => attackRange;
    public bool IsAttacking => isAttacking;
    public bool IsFinished => finished;
    public bool WasInterrupted => interrupted;

    void Awake()
    {
        ai = GetComponent<EnemyMain>();
    }

    public void Execute()
    {
        if (ai.target == null) return;
        if (isAttacking || !finished && currentPunch > 0) return;

        float dist = Vector3.Distance(transform.position, ai.target.position);
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

        isAttacking = true;
        currentPunch++;

        ai.StopMovement();
        Debug.Log($"windup for punch {currentPunch}");

        Invoke(nameof(PerformPunch), windupTime);
    }

    private void PerformPunch()
    {
        Debug.Log($"punch {currentPunch}");

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);
        bool hitLanded = false;
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log($"punch {currentPunch} hit");
                hitLanded = true;
                break;
            }
        }

        if (!hitLanded)
        {
            Debug.Log("or miss, i guess they never miss huh");
            ForceCancel();
        }

        Invoke(nameof(EndPunch), punchActiveTime);
    }

    private void EndPunch()
    {
        isAttacking = false;

        if (currentPunch < maxCombo && !interrupted)
        {
            Invoke(nameof(Windup), comboGap);
        }
        else
        {
            finished = true;
        }
    }

    public void ForceCancel()
    {
        CancelInvoke();
        isAttacking = false;
        finished = true;
        interrupted = true;
    }

    public void ResetAttackCycle()
    {
        currentPunch = 0;
        finished = false;
        interrupted = false;
        isAttacking = false;
    }
}