using UnityEngine;

public class RangedAttack : MonoBehaviour
{
    //WIP aun le falta un monton
    public float attackRange = 15f;
    public float bulletDamage = 15f;
    public float fireCooldown = 1f;

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

        Vector3 dir = (enemy.target.position - transform.position).normalized;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);

        if (Vector3.Distance(transform.position, enemy.target.position) <= attackRange && cooldownTimer <= 0f)
        {
            IsAttacking = true;
            cooldownTimer = fireCooldown;

            Debug.Log("fire");
        }
        else
        {
            IsAttacking = false;
        }
    }

}
