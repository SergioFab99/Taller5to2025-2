using UnityEngine;

public class Thrower : MonoBehaviour, IEnemyAttack
{
    [Header("Throw Settings")]
    public float attackRange = 12f;
    public float windup = 1f;              // delay before throwing
    public float throwCooldown = 2f;       // time between throws
    public float throwForce = 15f;
    public float upwardBoost = 0f;         // 0 = straight line, >0 = small arc

    public GameObject bluntPrefab;
    public Transform throwPoint;

    private EnemyMain ai;
    private bool isAttacking;
    private bool finished;
    private bool interrupted;
    private bool coolingDown;

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
        if (ai.target == null || isAttacking || coolingDown)
            return;

        float dist = Vector3.Distance(transform.position, ai.target.position);
        if (dist > attackRange) return;

        isAttacking = true;
        finished = false;
        interrupted = false;

        ai.StopMovement();
        Debug.Log($"{ai.name} winding up");

        Invoke(nameof(ThrowObject), windup);
    }

    private void ThrowObject()
    {
        if (interrupted || ai.target == null)
        {
            EndAttack();
            return;
        }

        Debug.Log($"{ai.name} throws a big ass brick");

        Vector3 targetPos = ai.target.position + Vector3.up * 1.0f;
        Vector3 dir = targetPos - throwPoint.position;
        float dist = dir.magnitude;
        dir.Normalize();

        // --- NEW: line-of-sight test ---
        bool blocked = Physics.Raycast(
            throwPoint.position,
            dir,
            out RaycastHit hit,
            dist,
            ~LayerMask.GetMask("Enemy") // ignore enemy layer
        );

        // true if something is between throwPoint and target
        bool targetVisible = false;
        if (blocked)
        {
            // If the hit object *is* the player, we actually have line of sight
            if (hit.collider.CompareTag("Player"))
                targetVisible = true;
        }
        else
        {
            // nothing in the way
            targetVisible = true;
        }

        GameObject obj = Instantiate(bluntPrefab, throwPoint.position, Quaternion.LookRotation(dir));
        Rigidbody rb = obj.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 velocity;

            if (targetVisible)
            {
                velocity = dir * throwForce + Vector3.up * upwardBoost;
                Debug.Log("[Thrower] Direct throw");
            }
            else
            {
                float arcBoost = Mathf.Clamp(dist * 0.25f, 6f, 14f);
                velocity = dir * (throwForce * 0.7f) + Vector3.up * arcBoost;
                Debug.Log("[Thrower] Arc throw (obstacle detected)");
            }

            rb.linearVelocity = velocity;
        }

        StartCooldown();
    }

    private void StartCooldown()
    {
        coolingDown = true;
        isAttacking = false;
        finished = false;

        Debug.Log($"{ai.name} cooling down ({throwCooldown}s)");
        Invoke(nameof(ResetCooldown), throwCooldown);
    }

    private void ResetCooldown()
    {
        coolingDown = false;
        finished = false;
        Debug.Log($"{ai.name} ready to throw again");
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
        Debug.Log($"{ai.name} interrupted.");
    }

    public void ResetAttackCycle()
    {
        isAttacking = false;
        interrupted = false;
        finished = !coolingDown;

        Debug.Log($"{ai.name}: ResetAttackCycle() — coolingDown={coolingDown}, finished={finished}");
    }

    void OnDisable() => CancelInvoke();
    void OnDestroy() => CancelInvoke();
}