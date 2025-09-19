using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyLife : MonoBehaviour
{
    [Header("Vida")]
    public int vidasIniciales = 2;
    public int vidasActuales;
    public bool estaMuerto = false;

    [Header("Daño por contacto EditorOnly")]
    [Tooltip("Tag que causa el daño al entrar en contacto")]
    public string hazardTag = "EditorOnly";

    [Header("UI (opcional)")]
    [Tooltip("Slider de vida del enemigo (world-space o UI). Si no se asigna, se ignora.")]
    public Slider vidaSlider;
    [Tooltip("Si es un slider compartido entre varios enemigos, reiniciarlo a full cuando este enemigo muera")]
    public bool resetSharedSliderOnDeath = true;

    // Seguimiento de objetos con los que ya estamos en contacto para no aplicar daño repetido hasta que salgamos
    private readonly HashSet<GameObject> hazardContacts = new HashSet<GameObject>();


    void Awake()
    {
    }

    void OnEnable()
    {
        // Reset UI con valores actuales si se reactivara
        UpdateLifeUI();
    }

    void Start()
    {
        vidasIniciales = Mathf.Max(1, vidasIniciales);
        vidasActuales = vidasIniciales;

        if (vidaSlider == null)
        {
            vidaSlider = GetComponentInChildren<Slider>();
        }
        
        if (vidaSlider != null)
        {
            vidaSlider.wholeNumbers = true; // pasos enteros
            vidaSlider.minValue = 0f;
            vidaSlider.maxValue = vidasIniciales;
            vidaSlider.value = vidasActuales;
        }
    }

    void Update()
    {
        if (estaMuerto) return;

        // Solo aplicar daño si:
        // 1) Estamos en contacto con al menos un objeto con el tag indicado
        // 2) Se presiona click izquierdo del mouse en este frame
        if (hazardContacts.Count > 0 && Input.GetMouseButtonDown(0))
        {
            TakeDamage(1);
        }
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

        UpdateLifeUI();
    }

    void Die()
    {
        if (estaMuerto) return;
        estaMuerto = true;

        // Si varios enemigos comparten el mismo slider, lo dejamos listo para el siguiente
        if (resetSharedSliderOnDeath && vidaSlider != null)
        {
            // Mantener el maxValue ya configurado; sólo reseteamos el valor a full
            vidaSlider.value = vidaSlider.maxValue;
        }

        Destroy(gameObject);
    }

    // Eliminado Update de daño por segundo: el daño sucede una sola vez por entrada

    private void OnCollisionEnter(Collision other)
    {
        GameObject hazard = GetHazardRoot(other.collider);
        if (hazard != null)
        {
            // Registrar contacto; el daño se aplicará sólo cuando haya click
            hazardContacts.Add(hazard);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        GameObject hazard = GetHazardRoot(other.collider);
        if (hazard != null)
        {
            hazardContacts.Remove(hazard);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        // No hacemos daño continuo en Stay; sólo aseguramos que el contacto siga registrado
        GameObject hazard = GetHazardRoot(other.collider);
        if (hazard != null)
        {
            hazardContacts.Add(hazard);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject hazard = GetHazardRoot(other);
        if (hazard != null)
        {
            // Registrar contacto; el daño se aplicará sólo cuando haya click
            hazardContacts.Add(hazard);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject hazard = GetHazardRoot(other);
        if (hazard != null)
        {
            hazardContacts.Remove(hazard);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        GameObject hazard = GetHazardRoot(other);
        if (hazard != null)
        {
            hazardContacts.Add(hazard);
        }
    }

    private void OnDisable()
    {
        hazardContacts.Clear();
    }

    private void UpdateLifeUI()
    {
        if (vidaSlider != null)
        {
            vidaSlider.value = vidasActuales;
        }
    }

    // Busca el objeto raíz con el tag de hazard para contar una sola vez por objeto (no por cada collider)
    private GameObject GetHazardRoot(Collider col)
    {
        if (col == null) return null;
        Transform t = col.transform;
        while (t != null)
        {
            if (t.CompareTag(hazardTag))
                return t.gameObject;
            t = t.parent;
        }
        return null;
    }
}
