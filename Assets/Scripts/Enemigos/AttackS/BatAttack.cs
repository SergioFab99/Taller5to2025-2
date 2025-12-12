using UnityEngine;

public class BatAttack : MonoBehaviour, IEnemyAttack
{
    [Header("Bat Settings")]
    [SerializeField] private float attackRange = 3.2f;
    public float AttackRange => attackRange;

    public float Damage = 22f;
    public float ArcAngle = 120f;
    public float ArcRadius = 2.5f;
    public LayerMask hitMask;
    public LayerMask projectileMask;

    [Header("Deflect")]
    public float swatArcAngle = 130f;
    public float swatRadius = 3f;
    public float swatCooldown = 0.3f;
    private float nextSwatTime = 0f;

    [Header("Timings")]
    public float windupTime = 0.8f;
    public float swingTime = 0.30f;
    public float recoveryTime = 1.4f;
    public int totalHits = 3;

    public bool IsAttacking { get; private set; }
    public bool IsFinished { get; private set; }
    public bool WasInterrupted { get; private set; }
    public bool ForceBlocked { get; private set; }
    public bool Missed { get; private set; }

    private EnemyStateHandler ai;
    private int currentHit = 0;
    private bool hitConnectedThisSwing = false;

    private enum Phase { None, Windup, Active, Recovery }
    private Phase phase = Phase.None;

    private void Awake()
    {
        ai = GetComponentInParent<EnemyStateHandler>();
    }

    public void BeginAttack(EnemyStateHandler handler)
    {
        ai = handler;
        ResetAttackCycle();
    }
    public void Execute()
    {
        if (IsAttacking || IsFinished) return;
        if (ai == null || ai.Target == null) return;

        IsAttacking = true;
        IsFinished = false;
        WasInterrupted = false;
        ForceBlocked = false;
        Missed = false;

        currentHit = 0;
        phase = Phase.Windup;

        ai.StopMovement();
        ai.character.UpdateInputs(new EnemyInput { Direction = ai.character.transform.forward, Move = ai.character.transform.forward * 0.35f }, ai.GetBehaviourState());

        Debug.Log($"{name} BAT WINDUP START");

        Invoke(nameof(StartSwing), windupTime);
    }

    private void StartSwing()
    {
        if (!IsAttacking || IsFinished) return;

        if (currentHit >= totalHits)
        {
            BeginRecovery();
            return;
        }

        Debug.Log($"{name} BAT HIT {currentHit + 1}");

        phase = Phase.Active;
        hitConnectedThisSwing = false;

        PerformArcHit(); 

        Invoke(nameof(EndSwing), swingTime);
    }

    private void EndSwing()
    {
        if (WasInterrupted || ForceBlocked)
        {
            FinishAttack();
            return;
        }

        if (!hitConnectedThisSwing)
            Missed = true;

        currentHit++;

        if (currentHit < totalHits)
        {
            Invoke(nameof(StartSwing), swingTime);
        }
        else
        {
            BeginRecovery();
        }
    }

    private void BeginRecovery()
    {
        phase = Phase.Recovery;
        Debug.Log($"{name} BAT RECOVERY");

        Invoke(nameof(FinishAttack), recoveryTime);
    }

    private void FinishAttack()
    {
        IsAttacking = false;
        IsFinished = true;
        phase = Phase.None;

        Debug.Log($"{name} BAT FINISHED | Missed={Missed} Interrupted={WasInterrupted} Blocked={ForceBlocked}");
    }

    private void PerformArcHit()
    {
        if (ai == null || ai.character == null) { Missed = true; return; }

        Vector3 origin = ai.character.transform.position + Vector3.up * 1.0f;

        Vector3 forward = ai.character.transform.forward;
        Collider[] hits = Physics.OverlapSphere(ai.character.transform.position, attackRange);
        bool hitLanded = false;
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                var recv = hit.GetComponent<CombatHitReceiver>()
                   ?? hit.GetComponentInChildren<CombatHitReceiver>()
                   ?? hit.GetComponentInParent<CombatHitReceiver>();

                if (recv != null)
                {
                    Vector3 hitPoint = hit.ClosestPoint(ai.character.transform.position);
                    var hitInfo = new HitInfo(hit, hitPoint, (hitPoint - ai.character.transform.position), 10f, WeaponType.Fist, null);
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
    }

    public void ManualUpdate()
    {
        CheckProjectileSwat();
    }

    private void CheckProjectileSwat()
    {
        if (Time.time < nextSwatTime) return;
        if (ai == null || ai.character == null) return;

        Collider[] incoming = Physics.OverlapSphere(ai.character.transform.position, swatRadius, projectileMask);

        foreach (var proj in incoming)
        {
            Vector3 dir = proj.transform.position - ai.character.transform.position;
            dir.y = 0;

            float angle = Vector3.Angle(ai.character.transform.forward, dir);

            if (angle <= swatArcAngle / 2f)
            {
                if (proj.TryGetComponent(out Rigidbody rb))
                {
                    rb.linearVelocity =
                        ai.character.transform.forward * rb.linearVelocity.magnitude * 1.25f;
                }

                nextSwatTime = Time.time + swatCooldown;
                Debug.Log($"{name} BAT DEFLECT");
                return;
            }
        }
    }

    public bool TryInterrupt()
    {
        if (phase == Phase.Windup)
        {
            Debug.Log($"{name} INTERRUPTED");
            WasInterrupted = true;
            IsFinished = true;
            IsAttacking = false;
            CancelInvoke();
            phase = Phase.None;
            return true;
        }

        return false;
    }

    public void ForceCancel(bool interrupted, bool blocked)
    {
        CancelInvoke();
        StopAllCoroutines();

        WasInterrupted = interrupted;
        ForceBlocked = blocked;

        if (!interrupted && !blocked)
            Missed = true;

        IsAttacking = false;
        IsFinished = true;
        phase = Phase.None;
    }

    public void ResetAttackCycle()
    {
        CancelInvoke();
        StopAllCoroutines();

        IsAttacking = false;
        IsFinished = false;
        Missed = false;
        WasInterrupted = false;
        ForceBlocked = false;

        currentHit = 0;
        phase = Phase.None;
        hitConnectedThisSwing = false;
    }

    private void OnDrawGizmos()
    {
        if (ai == null || ai.character == null)
            return;

        Vector3 origin = ai.character.transform.position + Vector3.up * 1.1f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, 0.1f); 

        Vector3 forward = ai.character.transform.forward;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, origin + forward * ArcRadius);

        float halfAngle = ArcAngle * 0.5f;
        Quaternion leftRot = Quaternion.Euler(0, -halfAngle, 0);
        Quaternion rightRot = Quaternion.Euler(0, halfAngle, 0);

        Vector3 leftDir = leftRot * forward;
        Vector3 rightDir = rightRot * forward;

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(origin, origin + leftDir * ArcRadius);
        Gizmos.DrawLine(origin, origin + rightDir * ArcRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin, ArcRadius);

        if (ai.character != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(ai.character.transform.position, Vector3.one * 0.1f);
        }

        if (ai.transform != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(ai.transform.position, Vector3.one * 0.1f);
        }
    }
}
