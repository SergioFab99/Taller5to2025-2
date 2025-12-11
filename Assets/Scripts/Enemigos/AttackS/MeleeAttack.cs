using UnityEngine;

public class MeleeAttack : MonoBehaviour, IEnemyAttack
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2.0f;
    public float AttackRange => attackRange;

    [SerializeField] private float damage = 10f;
    [SerializeField] private float hitRadius = 1.2f;
    [SerializeField] private LayerMask hitMask;

    [Header("Timings")]
    [SerializeField] private float windupTime = 0.4f;
    [SerializeField] private float punchActiveTime = 0.2f;
    [SerializeField] private float comboGap = 0.3f;
    [SerializeField] private int maxCombo = 2; 
    public bool IsAttacking { get; private set; }
    public bool IsFinished { get; private set; }
    public bool WasInterrupted { get; private set; }
    public bool ForceBlocked { get; private set; }
    public bool Missed { get; private set; }

    private EnemyStateHandler ai;
    private Transform characterTransform;

    private int currentPunch = 0;

    private enum AttackPhase { None, Windup, Active, Recovery }
    [SerializeField] private AttackPhase currentPhase = AttackPhase.None;

    private void Awake()
    {
        ai = GetComponentInParent<EnemyStateHandler>();
        if (ai != null && ai.character != null)
            characterTransform = ai.character.transform;
    }

    public void BeginAttack(EnemyStateHandler handler)
    {
        ai = handler;
        if (ai != null && ai.character != null)
            characterTransform = ai.character.transform;

        ResetAttackCycle();
    }

    public void Execute()
    {
        if (ai == null || ai.Target == null)
            return;

        if (IsAttacking || (!IsFinished && currentPunch > 0))
            return;

        if (currentPhase != AttackPhase.None)
            return;

        float dist = Vector3.Distance(characterTransform.position, ai.Target.position);
        if (dist > attackRange)
            return;

        currentPunch = 0;
        IsFinished = false;
        WasInterrupted = false;
        ForceBlocked = false;
        Missed = false;

        Windup();
    }

    private void Windup()
    {
        if (currentPunch >= maxCombo)
        {
            IsAttacking = false;
            IsFinished = true;
            currentPhase = AttackPhase.None;
            return;
        }

        currentPhase = AttackPhase.Windup;
        IsAttacking = true;
        currentPunch++;

        ai.StopMovement();
        ai.character.UpdateInputs(new EnemyInput { Direction = ai.character.transform.forward, Move = ai.character.transform.forward * 0.35f }, ai.GetBehaviourState());
        Debug.Log($"[{name}] Windup for punch {currentPunch}");

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

                var recv = hit.GetComponent<CombatHitReceiver>()
                   ?? hit.GetComponentInChildren<CombatHitReceiver>()
                   ?? hit.GetComponentInParent<CombatHitReceiver>();

                if (recv != null)
                {
                    Vector3 hitPoint = hit.ClosestPoint(characterTransform.position);
                    var hitInfo = new HitInfo(hit, hitPoint, (hitPoint - characterTransform.position), 10f, WeaponType.Fist, null);
                    recv.OnHit(hitInfo);
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

        if (ai != null && ai.QueuedBlock)
        {
            ForceCancel(false, true);
            return;
        }

        if (currentPunch < maxCombo && !WasInterrupted && !ForceBlocked)
        {
            Invoke(nameof(Windup), comboGap);
        }
        else
        {
            IsAttacking = false;
            IsFinished = true;
            currentPhase = AttackPhase.None;
        }
    }

    private void FinishAsMiss()
    {
        CancelInvoke();
        IsAttacking = false;
        IsFinished = true;
        Missed = true;
        currentPhase = AttackPhase.None;
    }

    public void ManualUpdate()
    {
        // wip
    }

    public bool TryInterrupt()
    {
        if (IsAttacking && currentPhase == AttackPhase.Windup)
        {
            Debug.Log($"{name} attack INTERRUPTED during windup");
            ForceCancel(true, false);
            return true;
        }
        return false;
    }

    public void ForceCancel(bool interrupted, bool blocked)
    {
        CancelInvoke();
        StopAllCoroutines();

        IsAttacking = false;
        IsFinished = true;
        WasInterrupted = interrupted;
        ForceBlocked = blocked;

        if (!interrupted && !blocked)
            Missed = true;

        currentPhase = AttackPhase.None;
    }

    public void ResetAttackCycle()
    {
        CancelInvoke();
        StopAllCoroutines();

        currentPunch = 0;
        IsAttacking = false;
        IsFinished = false;
        WasInterrupted = false;
        ForceBlocked = false;
        Missed = false;

        currentPhase = AttackPhase.None;
    }

    private void OnDrawGizmosSelected()
    {
        if (characterTransform == null)
            return;

        Vector3 center = characterTransform.position + characterTransform.forward * 1.4f + Vector3.up * 1.0f;

        float radius = 1.0f;
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(center, radius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(characterTransform.position + Vector3.up * 1.0f, center);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(characterTransform.position, AttackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(characterTransform.position + Vector3.up * 1f, 0.05f);
    }
}