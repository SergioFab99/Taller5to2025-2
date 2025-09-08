using UnityEngine;

public class MeleeAttack : MonoBehaviour, IEnemyAttack
{
    public float attackRange = 2f;
    public float punchDamage = 20f;
    public float punchCooldown = 1.2f;

    private float cooldownTimer = 0f;
    private EnemyMain enemy;

    public bool IsAttacking { get; private set; }
    public float AttackRange => attackRange;

    void Awake()
    {
        enemy = GetComponent<EnemyMain>();
    }

    public void Execute()
    {
        if (enemy.target == null) return;

        cooldownTimer -= Time.deltaTime;

        if (Vector3.Distance(transform.position, enemy.target.position) <= attackRange && cooldownTimer <= 0f)
        {
            IsAttacking = true;
            cooldownTimer = punchCooldown;

            Debug.Log("punch");
        }
        else
        {
            IsAttacking = false;
        }
    }
}
