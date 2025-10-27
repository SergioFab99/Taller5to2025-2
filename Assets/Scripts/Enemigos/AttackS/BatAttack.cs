using UnityEngine;

public class BatAttack : MonoBehaviour, IEnemyAttack
{
    [Header("Attack Parameters")]
    public float attackRange = 3f;
    public float damage = 20f;
    public int maxCombo = 3;
    public float windupTime = 0.5f;
    public float swingActiveTime = 0.5f;
    public float recoveryTime = 1.5f;
    public float knockbackForce = 12f;
    public float swingArc = 120f;
    public LayerMask hittableLayers;

    [Header("Reflect")]
    public float reflectRadius = 4.5f;
    public float reflectForce = 15f;
    public float reflectCooldown = 0.5f;
    private float reflectTimer;

    private EnemyStateHandler handler;
    [SerializeField] private Transform characterTransform;

    private bool isAttacking;
    private bool finished;
    private bool interrupted;
    public bool Missed { get; private set; }
    public bool ForceBlocked { get; private set; }

    private int comboStep;

    private enum AttackPhase { None, Windup, Active, Recovery }
    [SerializeField] private AttackPhase currentPhase = AttackPhase.None;

    public float AttackRange => attackRange;
    public bool IsAttacking => isAttacking;
    public bool IsFinished => finished;
    public bool WasInterrupted => interrupted;

    private void Awake()
    {
        handler = GetComponent<EnemyStateHandler>();
        if (handler == null)
            handler = GetComponentInParent<EnemyStateHandler>();

        if (handler != null)
            characterTransform = handler.GetComponentInChildren<EnemyCharacter>().transform;
        else
            Debug.LogError($"{name}: No EnemyStateHandler found in parent!");
    }

    private void Update()
    {
        reflectTimer -= Time.deltaTime;
        if (reflectTimer <= 0f)
            DetectAndReflectIncoming();
    }

    public void Execute()
    {
        if (handler.Target == null)
        {
            return;
        }

        if (isAttacking)
        {
            return;
        }

        if (!finished && comboStep > 0)
        {
            return;
        }

        float dist = Vector3.Distance(characterTransform.position, handler.Target.position);
        Debug.Log($"[{name}] target distance: {dist:F2} / range: {attackRange}");

        if (dist > attackRange)
        {
            return;
        }

        comboStep = 0;
        finished = false;
        interrupted = false;

        StartWindup();
    }

    private void StartWindup()
    {
        if (comboStep >= maxCombo)
        {
            finished = true;
            return;
        }

        comboStep++;
        isAttacking = true;
        currentPhase = AttackPhase.Windup;

        handler?.StopMovement();

        Debug.Log($"[{handler.name}] Windup for bat swing #{comboStep}");
        Invoke(nameof(PerformSwing), windupTime);
    }

    private void PerformSwing()
    {
        if (interrupted)
        {
            EndAttack();
            return;
        }

        currentPhase = AttackPhase.Active;
        Debug.Log($"[{handler.name}] performs bat swing #{comboStep}");

        bool hitLanded = false;

        Vector3 forward = characterTransform.forward;
        Vector3 center = characterTransform.position + forward * (attackRange * 0.5f);

        Debug.DrawRay(characterTransform.position, forward * 2f, Color.green);
        Collider[] hits = Physics.OverlapSphere(center, attackRange * 0.75f, hittableLayers);
        foreach (Collider hit in hits)
        {
            Vector3 toTarget = (hit.transform.position - characterTransform.position).normalized;
            float angle = Vector3.Angle(forward, toTarget);
            if (angle <= swingArc * 0.5f)
            {
                if (hit.CompareTag("Player"))
                {
                    if (hit.TryGetComponent(out HealthController hp))
                        hp.TakeDamague(damage);

                    if (hit.attachedRigidbody != null)
                        hit.attachedRigidbody.AddForce(toTarget * knockbackForce, ForceMode.Impulse);

                    hitLanded = true;
                    Missed = false;
                    break;
                }
                else if (hit.CompareTag("Grabbable"))
                {
                    ReflectObject(hit);
                }
            }
        }

        if (!hitLanded)
        {
            Debug.Log($"[{handler.name}] swing missed");
            Missed = true;
            ForceCancel(false, false); 
        }

        Invoke(nameof(EndSwing), swingActiveTime);
    }

    private void EndSwing()
    {
        currentPhase = AttackPhase.Recovery;
        isAttacking = false;

        Invoke(nameof(NextComboStep), recoveryTime);
    }

    private void NextComboStep()
    {
        if (interrupted)
        {
            finished = true;
            return;
        }

        if (comboStep < maxCombo)
            StartWindup();
        else
        {
            finished = true;
            currentPhase = AttackPhase.None;
            Debug.Log($"[{handler.name}] finished combo.");
        }
    }

    private void EndAttack()
    {
        CancelInvoke();
        isAttacking = false;
        finished = true;
        currentPhase = AttackPhase.None;
    }

    public void ForceCancel(bool interrupt, bool block)
    {
        CancelInvoke();
        isAttacking = false;
        finished = true;
        interrupted = interrupt;
        ForceBlocked = block;
        currentPhase = AttackPhase.None;
    }

    public void ResetAttackCycle()
    {
        CancelInvoke();
        isAttacking = false;
        finished = false;
        interrupted = false;
        ForceBlocked = false;
        Missed = false;
        comboStep = 0;
        currentPhase = AttackPhase.None;
    }

    public bool TryInterrupt()
    {
        if (isAttacking && currentPhase == AttackPhase.Windup)
        {
            Debug.Log($"[{handler.name}] Bat attack interrupted during windup!");
            ForceCancel(true, false);
            return true;
        }
        return false;
    }

    private void DetectAndReflectIncoming()
    {
        Collider[] nearby = Physics.OverlapSphere(characterTransform.position, reflectRadius);
        foreach (Collider c in nearby)
        {
            if (!c.CompareTag("Grabbable")) continue;
            Rigidbody rb = c.attachedRigidbody;
            if (rb == null) continue;

            Vector3 toEnemy = (characterTransform.position - c.transform.position).normalized;
            float approachDot = Vector3.Dot(rb.linearVelocity.normalized, toEnemy);
            if (approachDot > 0.5f)
            {
                ReflectObject(c);
                reflectTimer = reflectCooldown;
                break;
            }
        }
    }

    private void ReflectObject(Collider obj)
    {
        Rigidbody rb = obj.attachedRigidbody;
        if (rb != null)
        {
            Vector3 dir = (handler.Target != null)
                ? (handler.Target.position - obj.transform.position).normalized
                : (characterTransform.forward + Vector3.up * 0.3f).normalized;

            rb.linearVelocity = dir * reflectForce;
            Debug.Log($"[{handler.name}] reflected {obj.name}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(characterTransform.position, reflectRadius);
        Gizmos.color = Color.red;
        Vector3 center = characterTransform.position + characterTransform.forward * (attackRange * 0.5f);
        Gizmos.DrawWireSphere(center, attackRange);
    }
}