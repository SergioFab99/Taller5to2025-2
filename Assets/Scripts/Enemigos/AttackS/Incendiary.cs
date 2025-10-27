using UnityEngine;

public class Incendiary : MonoBehaviour, IEnemyAttack
{
    [Header("Molotov Settings")]
    public float attackRange = 12f;
    public float windup = 1f;             
    public float throwCooldown = 5f;         
    public float throwForce = 10f;
    public float upwardArc = 5f;

    public GameObject molotovPf;
    public Transform throwPoint;

    private EnemyMain ai;
    private bool isAttacking = false;
    private bool finished = false;
    private bool interrupted = false;
    private bool coolingDown = false;
    public bool Missed { get; private set; }
    public bool ForceBlocked { get; private set; }

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
        Debug.Log($"{ai.name} is winding up a molotov");

        Invoke(nameof(ThrowMolotov), windup);
    }

    private void ThrowMolotov()
    {
        if (interrupted || ai.target == null)
        {
            EndAttack();
            return;
        }

        Debug.Log($"{ai.name} throws a molotov!");

        Vector3 targetPos = ai.target.position;
        Vector3 dir = targetPos - throwPoint.position;

        GameObject molotovObj = Instantiate(molotovPf, throwPoint.position, Quaternion.identity);
        Rigidbody rb = molotovObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 throwVelocity = dir.normalized * throwForce + Vector3.up * upwardArc;
            rb.linearVelocity = throwVelocity;
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
        Debug.Log($"{ai.name} ready to throw again.");
    }

    private void EndAttack()
    {
        isAttacking = false;
        finished = true;
    }

    public void ForceCancel(bool interrupt, bool block)
    {
        CancelInvoke();
        isAttacking = false;
        finished = true;
        interrupted = interrupt;
    
        Debug.Log($"{ai.name} dropped molotov due to interruption!");
        if (molotovPf != null)
        {
            GameObject molotov = Instantiate(molotovPf, transform.position + Vector3.up, Quaternion.identity);
            Bottle proj = molotov.GetComponent<Bottle>();
            proj?.Kaboom();
        }
    }

    public void ResetAttackCycle()
    {
        CancelInvoke();
        isAttacking = false;
        interrupted = false;
        finished = !coolingDown;
        Debug.Log($"{ai.name}: ResetAttackCycle() complete. Cooldown: {coolingDown}, Finished: {finished}");
    }

    public bool TryInterrupt()
    {
        return true;
    }
}
