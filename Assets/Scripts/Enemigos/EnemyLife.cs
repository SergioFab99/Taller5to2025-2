using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    [Header("Vida (Legacy)")]
    public int vidasIniciales = 2;
    [HideInInspector] public int vidasActuales;
    [HideInInspector] public bool estaMuerto = false;

    [Header("Components")]
    public Animator enemyAnimator;
    [Tooltip("Optional explicit HealthController. If null one will be searched or auto-created.")]
    public HealthController healthController;

    [Header("Damage Settings")] public int damagePerHit = 1;

    private void Awake()
    {
        if (healthController == null)
        {
            healthController = GetComponent<HealthController>();
            if (healthController == null)
            {
                healthController = gameObject.AddComponent<HealthController>();
                healthController.maxHealth = Mathf.Max(1, vidasIniciales);
                healthController.health = healthController.maxHealth;
            }
        }
        // Sync legacy fields
        vidasIniciales = Mathf.Max(1, (int)healthController.maxHealth);
        vidasActuales = (int)healthController.health;
        healthController.OnDead += OnDeadHandler;
        healthController.OnHealthUpdated += OnHealthUpdated;
    }

    private void OnDestroy()
    {
        if (healthController != null)
        {
            healthController.OnDead -= OnDeadHandler;
            healthController.OnHealthUpdated -= OnHealthUpdated;
        }
    }

    private void OnHealthUpdated(float current, float max)
    {
        vidasActuales = (int)current;
    }

    public void TakeDamage(int cantidad = -1)
    {
        if (estaMuerto) return;
        if (cantidad <= 0) cantidad = damagePerHit;
    if (enemyAnimator != null) enemyAnimator.SetTrigger("Hit");
        healthController.TakeDamage(cantidad);
    }

    private void OnDeadHandler()
    {
        Die();
    }

    private void Die()
    {
        if (estaMuerto) return;
        estaMuerto = true;
        // Optionally play death animation trigger here
        Destroy(gameObject, 0.05f);
    }

    // (Legacy hooks left for future bullet triggers etc.)
    private void OnCollisionEnter(Collision other) { }
    private void OnTriggerEnter(Collider other) { }
}
