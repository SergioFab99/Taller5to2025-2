using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerLife : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public int vidasIniciales = 3;
    public int vidasMaximas = 5;

    [Header("Estado Actual")]
    public int vidasActuales;
    public bool estaMuerto = false;

    private void Start()
    {
        vidasIniciales = Mathf.Max(1, vidasIniciales);
        vidasMaximas = Mathf.Max(vidasIniciales, vidasMaximas);
        vidasActuales = Mathf.Clamp(vidasIniciales, 0, vidasMaximas);
    }

    private void Update()
    {
    }

    public void TakeDamage(int cantidad = 1)
    {
    if (estaMuerto) return;

        cantidad = Mathf.Max(1, cantidad);
        vidasActuales -= cantidad;
        vidasActuales = Mathf.Max(0, vidasActuales);

        if (vidasActuales <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (estaMuerto) return;
        estaMuerto = true;
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Enemy"))
        {
            TakeDamage();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage();
        }
    }
    public int ObtenerVidas() => vidasActuales;
}
