using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    [Header("Vida")]
    public int vidasIniciales = 2;
    public int vidasActuales;
    public bool estaMuerto = false;



    void Awake()
    {

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

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {

    }

    void OnTriggerEnter(Collider other)
    {

    }
}
