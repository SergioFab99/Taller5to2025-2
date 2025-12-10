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

        Debug.Log($"[{name}] BAT WINDUP START");

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

        Debug.Log($"[{name}] BAT SWING {currentHit + 1}");

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
        Debug.Log($"[{name}] BAT RECOVERY");

        Invoke(nameof(FinishAttack), recoveryTime);
    }

    private void FinishAttack()
    {
        IsAttacking = false;
        IsFinished = true;
        phase = Phase.None;

        Debug.Log($"[{name}] BAT FINISHED | Missed={Missed} Interrupted={WasInterrupted} Blocked={ForceBlocked}");
    }

    private void PerformArcHit()
    {
        if (ai == null || ai.character == null)
        {
            Missed = true;
            return;
        }

        Vector3 origin = ai.character.transform.position;
        Vector3 forward = ai.character.transform.forward;
        bool hitSomething = false;

        Collider[] cols = Physics.OverlapSphere(origin, ArcRadius, hitMask);

        foreach (var col in cols)
        {
            Vector3 dir = col.transform.position - origin;
            dir.y = 0;

            float angle = Vector3.Angle(forward, dir);

            if (angle <= ArcAngle / 2f && col.TryGetComponent(out HealthController hp))
            {
                hp.TakeDamague(ai.EffectiveDamage(Damage));
                hitSomething = true;
                hitConnectedThisSwing = true;
            }
        }

        if (!hitSomething)
            Missed = true;
    }
    public void ManualUpdate()
    {
        CheckProjectileSwat();
    }

    private void CheckProjectileSwat()
    {
        if (Time.time < nextSwatTime) return;
        if (ai == null || ai.character == null) return;

        Collider[] incoming =
            Physics.OverlapSphere(ai.character.transform.position, swatRadius, projectileMask);

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
                Debug.Log($"[{name}] BAT DEFLECT PROJECTILE");
                return;
            }
        }
    }

    public bool TryInterrupt()
    {
        if (phase == Phase.Windup)
        {
            Debug.Log($"[{name}] INTERRUPTED");
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
}
