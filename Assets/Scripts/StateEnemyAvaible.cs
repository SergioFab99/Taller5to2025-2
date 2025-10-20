using UnityEngine;
using UnityEngine.EventSystems;

public class StateEnemyAvaible : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Salud")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Daño por Zona Corporal")]
    public int dañoCuerpoSuperior = 30;
    public int dañoCuerpoMedio = 20;
    public int dañoCuerpoInferior = 10;

    [Header("Estado")]
    public bool estaMuerto = false;

    [Header("Depuración")]
    public bool mostrarMensajes = true;

    [Header("Colisión")]
    public GameObject objetoColisionador;
    [SerializeField] private Vector3 normalPlanoArrastre = Vector3.up;

    private Plane planoArrastre;
    private Vector3 offsetArrastre;
    private bool arrastreActivo;
    private Collider ultimoColliderValido;

    void Start()
    {
        currentHealth = maxHealth;
        estaMuerto = false;

        if (mostrarMensajes)
            Debug.Log($"{name}: Enemigo listo. Vida: {currentHealth}");
    }

    void OnTriggerEnter(Collider other)
    {
        if (estaMuerto) return;

        if (!other.CompareTag("PlayerFist")) return;

        if (!EsColisionadorValido(other))
        {
            if (mostrarMensajes)
            {
                Debug.Log($"{name}: Colisión ignorada. {other.name} no coincide con objetoColisionador.");
            }
            return;
        }

        ultimoColliderValido = other;

        if (mostrarMensajes)
        {
            Debug.Log($"{name}: Colisión válida detectada con {other.name}.");
        }
    }

    public void RecibirGolpeEnZona(string zona)
    {
        ProcesarGolpe(zona, null);
    }

    public void RecibirGolpeEnZona(string zona, Collider colliderGolpeador)
    {
        ProcesarGolpe(zona, colliderGolpeador);
    }

    public void TakeDamage(int daño, string zona = "")
    {
        int saludAnterior = currentHealth;
        currentHealth -= daño;

        if (mostrarMensajes)
        {
            Debug.Log($"{name} golpeado en {zona}! Daño: {daño}. Vida: {saludAnterior} → {currentHealth}");
        }

        if (currentHealth <= 0 && !estaMuerto)
        {
            Morir();
        }
    }

    private void Morir()
    {
        estaMuerto = true;
        if (mostrarMensajes)
        {
            Debug.Log($"{name} ha sido derrotado.");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        arrastreActivo = false;

        if (objetoColisionador == null) return;

        Camera camara = ObtenerCamara(eventData);
        if (camara == null) return;

        Vector3 normal = normalPlanoArrastre.sqrMagnitude > 0f ? normalPlanoArrastre.normalized : Vector3.up;
        planoArrastre = new Plane(normal, objetoColisionador.transform.position);

        Ray rayo = camara.ScreenPointToRay(eventData.position);
        if (!planoArrastre.Raycast(rayo, out float distancia)) return;

        Vector3 punto = rayo.GetPoint(distancia);
        offsetArrastre = objetoColisionador.transform.position - punto;
        arrastreActivo = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (objetoColisionador == null || !arrastreActivo) return;

        Camera camara = ObtenerCamara(eventData);
        if (camara == null) return;

        Ray rayo = camara.ScreenPointToRay(eventData.position);
        if (!planoArrastre.Raycast(rayo, out float distancia)) return;

        Vector3 punto = rayo.GetPoint(distancia);
        objetoColisionador.transform.position = punto + offsetArrastre;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        arrastreActivo = false;
    }

    private void ProcesarGolpe(string zona, Collider colliderGolpeador)
    {
        if (estaMuerto) return;

        Collider referencia = colliderGolpeador ?? ultimoColliderValido;

        if (objetoColisionador != null)
        {
            if (referencia == null)
            {
                if (mostrarMensajes)
                {
                    Debug.LogWarning($"{name}: Se recibió un golpe sin referencia de collider. Arrastra objetoColisionador o pasa la colisión explícitamente.");
                }
                return;
            }

            if (!EsColisionadorValido(referencia))
            {
                if (mostrarMensajes)
                {
                    Debug.Log($"{name}: Golpe ignorado. {referencia.name} no coincide con objetoColisionador.");
                }
                return;
            }
        }

        int daño = 0;
        string nombreZona = string.Empty;

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
                if (mostrarMensajes)
                {
                    Debug.LogWarning($"Zona no reconocida: {zona}");
                }
                break;
        }

        if (mostrarMensajes)
        {
            Debug.Log($"{name}: Colisión detectada en {nombreZona}. Daño previsto: {daño}");
        }

        TakeDamage(daño, nombreZona);
    }

    private bool EsColisionadorValido(Collider other)
    {
        if (other == null) return false;

        if (objetoColisionador == null) return true;

        if (other.gameObject == objetoColisionador) return true;

        return other.transform.IsChildOf(objetoColisionador.transform);
    }

    private Camera ObtenerCamara(PointerEventData eventData)
    {
        return eventData != null && eventData.pressEventCamera != null ? eventData.pressEventCamera : Camera.main;
    }
}