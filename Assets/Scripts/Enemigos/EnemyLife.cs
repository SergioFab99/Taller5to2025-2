using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    [Header("Vida")]
    public int vidasIniciales = 2;
    public int vidasActuales;
    public bool estaMuerto = false;

    [Header("Referencias")]
    public ExperimentalEnemy movimiento;

    void Awake()
    {
        if (movimiento == null)
            movimiento = GetComponent<ExperimentalEnemy>();
    }

    void Start()
    {
        vidasIniciales = Mathf.Max(1, vidasIniciales);
        vidasActuales = vidasIniciales;
    }

    public void TakeDamage(int cantidad = 1)
    {
        if (estaMuerto) return;
        cantidad = Mathf.Max(1, cantidad);
        vidasActuales -= cantidad;
        if (vidasActuales <= 0)
        {
            vidasActuales = 0;
            Die();
        }
    }

    void Die()
    {
        if (estaMuerto) return;
        estaMuerto = true;
        if (movimiento != null)
        {
            movimiento.enabled = false;
        }
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("bullet"))
        {
            TakeDamage();
            Destroy(other.collider.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bullet"))
        {
            TakeDamage();
            Destroy(other.gameObject);
        }
    }
}
