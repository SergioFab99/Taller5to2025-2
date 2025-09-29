using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    [Header("Vida")]
    public int vidasIniciales = 2;
    public int vidasActuales;
    public bool estaMuerto = false;

    public Animator animation;



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
        animation.SetTrigger("Hit");
    }

    void Die()
    {
        if (estaMuerto) return;
        estaMuerto = true;

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        /*if (other.collider.CompareTag("bullet"))
        {
            TakeDamage();
            Destroy(other.collider.gameObject);
        }*/
    }

    void OnTriggerEnter(Collider other)
    {

    }
}
