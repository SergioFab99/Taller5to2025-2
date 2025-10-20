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

    [Header("Throwable Reflection")]
    public float reflectRadius = 4.5f;  
    public float reflectForce = 15f;
    public float reflectCooldown = 0.5f;  
    private float reflectTimer;

    private EnemyMain ai;
    private bool isAttacking;
    private bool finished;
    private bool interrupted;
    private int comboStep;

    public float AttackRange => attackRange;
    public bool IsAttacking => isAttacking;
    public bool IsFinished => finished;
    public bool WasInterrupted => interrupted;

    void Awake()
    {
        ai = GetComponent<EnemyMain>();
    }

    void Update()
    {
        reflectTimer -= Time.deltaTime;
        if (reflectTimer <= 0f)
        {
            DetectAndReflectIncoming();
        }
    }

    public void Execute()
    {
        if (ai.target == null || isAttacking) return;

        float dist = Vector3.Distance(transform.position, ai.target.position);
        if (dist > attackRange) return;

        comboStep = 0;
        finished = false;
        interrupted = false;

        StartCombo();
    }

    private void StartCombo()
    {
        if (comboStep >= maxCombo)
        {
            finished = true;
            return;
        }

        comboStep++;
        isAttacking = true;
        ai.StopMovement();
        Debug.Log($"[{ai.name}] Windup for bat attack #{comboStep}");

        Invoke(nameof(PerformSwing), windupTime);
    }

    private void PerformSwing()
    {
        if (interrupted || ai.target == null)
        {
            EndAttack();
            return;
        }

        Debug.Log($"[{ai.name}] performs bat swing #{comboStep}");

        Vector3 forward = ai.transform.forward;
        Collider[] hits = Physics.OverlapSphere(transform.position + forward, attackRange, hittableLayers);

        foreach (Collider hit in hits)
        {
            Vector3 toTarget = (hit.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(forward, toTarget);
            if (angle <= swingArc * 0.5f)
            {
                if (hit.CompareTag("Player"))
                {
                    var health = hit.GetComponentInParent<HealthController>();
                    if (health != null)
                        health.TakeDamague(damage);

                    Rigidbody rb = hit.attachedRigidbody;
                    if (rb != null)
                        rb.AddForce(toTarget * knockbackForce, ForceMode.Impulse);
                }
                else if (hit.CompareTag("Grabbable"))
                {
                    ReflectObject(hit);
                }
            }
        }

        Invoke(nameof(NextComboStep), swingActiveTime + recoveryTime);
    }

    private void NextComboStep()
    {
        isAttacking = false;

        if (comboStep < maxCombo && !interrupted)
        {
            StartCombo();
        }
        else
        {
            finished = true;
            Debug.Log($"[{ai.name}] finished combo.");
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
        finished = true;
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
        CancelInvoke();
        isAttacking = false;
        finished = false;
        interrupted = false;
        comboStep = 0;
    }
    private void DetectAndReflectIncoming()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, reflectRadius);
        foreach (Collider c in nearby)
        {
            if (!c.CompareTag("Grabbable")) continue;

            Rigidbody rb = c.attachedRigidbody;
            if (rb == null) continue;

            Vector3 toEnemy = (transform.position - c.transform.position).normalized;
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
            Vector3 dir = (ai.target != null)
                ? (ai.target.position - obj.transform.position).normalized
                : (transform.forward + Vector3.up * 0.3f).normalized;

            rb.linearVelocity = dir * reflectForce;
            Debug.Log($"[{ai.name}] reflected {obj.name}!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + transform.forward, attackRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, reflectRadius);
    }
}