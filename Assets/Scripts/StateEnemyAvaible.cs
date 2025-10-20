using UnityEngine;

/// <summary>
/// Script principal del enemigo que gestiona su estado, salud y reacción a golpes.
/// Incluye detección de zonas corporales (superior, media, inferior) para aplicar daño variable.
/// Funciona con Kinematic Character Controller y colisiones por trigger (sin Rigidbody).
/// </summary>
public class StateEnemyAvaible : MonoBehaviour
{
    // --- SALUD DEL ENEMIGO ---
    [Header("Salud")]
    public int maxHealth = 100;          // Vida máxima del enemigo
    public int currentHealth;            // Vida actual (pública para depuración)

    // --- DAÑO POR ZONA ---
    [Header("Daño por Zona Corporal")]
    public int dañoCuerpoSuperior = 30;  // Cabeza / hombros → daño alto
    public int dañoCuerpoMedio = 20;     // Pecho / abdomen → daño medio
    public int dañoCuerpoInferior = 10;  // Piernas → daño bajo

    // --- ESTADO ---
    [Header("Estado")]
    public bool estaMuerto = false;      // Indica si el enemigo ya murió

    // --- DEPURACIÓN ---
    [Header("Depuración")]
    public bool mostrarMensajes = true;  // Activa/desactiva logs en consola

    // --- INICIALIZACIÓN ---
    void Start()
    {
        // Inicializamos la vida actual al máximo
        currentHealth = maxHealth;
        estaMuerto = false;

        if (mostrarMensajes)
            Debug.Log($"{name}: Enemigo listo. Vida: {currentHealth}");
    }

    /// <summary>
    /// Se llama cuando un collider entra en contacto con un trigger del enemigo.
    /// Este método detecta qué parte del cuerpo fue golpeada (por el nombre del GameObject que colisiona).
    /// Solo responde a objetos con la etiqueta "PlayerFist".
    /// </summary>
    /// <param name="other">El collider que entró en el trigger (ej. puño del jugador).</param>
    void OnTriggerEnter(Collider other)
    {
        // Si el enemigo ya está muerto, ignoramos cualquier golpe
        if (estaMuerto) return;

        // Solo procesamos colisiones con los puños del jugador
        if (other.CompareTag("PlayerFist"))
        {
            // Obtenemos el nombre del GameObject que tiene el collider del puño
            // Pero más importante: necesitamos saber **qué zona del enemigo fue golpeada**.
            // Para eso, el trigger debe estar en un hijo del enemigo (UpperBody, etc.).
            // Sin embargo, OnTriggerEnter se ejecuta en el objeto que tiene el script.
            // Por lo tanto, **este script debe estar en cada zona**, o usar otra estrategia.

            // ⚠️ ¡IMPORTANTE! Esta implementación asume que este script está en el ENEMIGO PRINCIPAL,
            // pero los triggers están en sus hijos. En ese caso, **OnTriggerEnter NO se llamará aquí**.
            // Por eso, la solución correcta es tener un script ligero en CADA ZONA que llame a este método.

            // Pero si insistes en tener TODO en StateEnemyAvaible, debemos usar otro enfoque:
            // → El puño debe tener un script que, al colisionar, le diga al enemigo qué zona fue golpeada.

            // Dado que tu enfoque es con triggers en el enemigo, la mejor práctica es:
            // → Tener un componente en cada zona (UpperBody, etc.) que llame a TakeDamageFromZone.
            // Por eso, exponemos un método público para que otras zonas lo llamen.
        }
    }

    /// <summary>
    /// Método público que permite a las zonas del cuerpo (UpperBody, MiddleBody, etc.)
    /// notificar al enemigo que fue golpeado en una zona específica.
    /// </summary>
    /// <param name="zona">La zona del cuerpo golpeada ("Superior", "Media", "Inferior").</param>
    public void RecibirGolpeEnZona(string zona)
    {
        if (estaMuerto) return;

        int daño = 0;
        string nombreZona = "";

        // Determinamos el daño según la zona
        switch (zona)
        {
            case "Superior":
                daño = dañoCuerpoSuperior;
                nombreZona = "Cuerpo Superior";
                break;
            case "Media":
                daño = dañoCuerpoMedio;
                nombreZona = "Cuerpo Medio";
                break;
            case "Inferior":
                daño = dañoCuerpoInferior;
                nombreZona = "Cuerpo Inferior";
                break;
            default:
                daño = dañoCuerpoInferior;
                nombreZona = "Zona Desconocida";
                if (mostrarMensajes) Debug.LogWarning($"Zona no reconocida: {zona}");
                break;
        }

        if (mostrarMensajes)
        {
            Debug.Log($"{name}: Colisión detectada en {nombreZona}. Daño previsto: {daño}");
        }

        // Aplicamos el daño
        TakeDamage(daño, nombreZona);
    }

    /// <summary>
    /// Aplica daño al enemigo y verifica si muere.
    /// </summary>
    /// <param name="daño">Cantidad de daño a restar.</param>
    /// <param name="zona">Nombre de la zona golpeada (solo para mensajes).</param>
    public void TakeDamage(int daño, string zona = "")
    {
        int saludAnterior = currentHealth;
        currentHealth -= daño;

        if (mostrarMensajes)
        {
            Debug.Log($"{name} golpeado en {zona}! Daño: {daño}. Vida: {saludAnterior} → {currentHealth}");
        }

        // Verificamos si el enemigo murió
        if (currentHealth <= 0 && !estaMuerto)
        {
            Morir();
        }
    }

    /// <summary>
    /// Lógica de muerte del enemigo.
    /// </summary>
    private void Morir()
    {
        estaMuerto = true;
        if (mostrarMensajes)
        {
            Debug.Log($"{name} ha sido derrotado.");
        }

        // Aquí puedes añadir:
        // - Animación de muerte
        // - Partículas
        // - Notificación al sistema de puntuación
        // Ejemplo: Destroy(gameObject, 2f);
    }
}