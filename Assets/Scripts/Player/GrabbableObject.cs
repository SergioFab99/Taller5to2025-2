using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableObject : MonoBehaviour
{
    [Header("Durability")]
    [SerializeField] private int maxUses = 5;
    [SerializeField]private int usesLeft;
    public bool IsBroken { get; private set; } = false;
    public event System.Action OnBrokenEvent; // Fired once when object breaks
    [Header("Configuración de Agarre")]
    [FormerlySerializedAs("tag")]
    [SerializeField] private string requiredTag = "Grabbable"; // Tag que debe tener el objeto (asegúrate de que esté asignado)
    [SerializeField] private float interactionDistance = 2f; // Distancia para mostrar indicador visual
    [SerializeField] private Color highlightColor = Color.yellow; // Color para resaltar cuando es agarrable
    [SerializeField] private bool showGlowWhenNear = true; // ¿Mostrar efecto visual cuando está cerca?
    [SerializeField] private bool showCanvasWhenNear = true; // ¿Activar/desactivar un Canvas de UI cuando esté enfocado?
    [SerializeField] private Canvas uiCanvas; // Referencia al Canvas de UI (opcional)

    private Renderer _renderer;
    private Rigidbody _rigidbody;
    private Material _originalMaterial;
    private Material _highlightMaterial;

    private void Awake()
    {
        // Initialize durability
        usesLeft = maxUses;
        IsBroken = false;
        // Asegurar que tenga Rigidbody
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError($"[{name}] GrabbableObject requires a Rigidbody component!", this);
            enabled = false;
            return;
        }

        // Asignar tag automáticamente si no está configurado
        if (string.IsNullOrEmpty(requiredTag))
            requiredTag = "Grabbable";

        gameObject.tag = requiredTag; // Esto asegura que siempre tenga el tag correcto

        // Inicializar material para resaltar
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            _originalMaterial = _renderer.material;
            _highlightMaterial = new Material(_originalMaterial);
            _highlightMaterial.color = highlightColor;
        }

        // Asegurar que el Canvas de UI empiece desactivado
        if (uiCanvas != null)
        {
            uiCanvas.gameObject.SetActive(false);
        }

    // Initialize durability
    usesLeft = maxUses;
    IsBroken = false;
    }

    private void Update()
    {
        // Check if we need to evaluate raycast (for highlight and/or Canvas)
        bool needsCheck = ((showGlowWhenNear && _renderer != null) || (showCanvasWhenNear && uiCanvas != null));
        if (!needsCheck) return;

        // Ray from main camera forward up to interactionDistance
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);
        bool hitThis = false;
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            hitThis = (hit.collider.gameObject == gameObject);
        }

        // Highlight logic
        if (showGlowWhenNear && _renderer != null)
        {
            _renderer.material = hitThis ? _highlightMaterial : _originalMaterial;
        }

        // UI Canvas logic: activate only if looking at this object
        if (uiCanvas != null)
        {
            uiCanvas.gameObject.SetActive(showCanvasWhenNear && hitThis);
        }
    }

    // Opcional: Método público para permitir que otros sistemas interactúen
    public bool IsGrabbed() => _rigidbody.isKinematic;

    // Use the object (durability)
    // Use the object (durability)
    public bool Use()
    {
        if (IsBroken) return false;
        usesLeft--;
        if (usesLeft <= 0)
        {
            IsBroken = true;
            OnBroken();
        }
        return true;
    }

    public int GetUsesLeft() => usesLeft;

    protected virtual void OnBroken()
    {
        Debug.Log($"{name} is broken!");
        // Add visual effects, disable object, etc.
        OnBrokenEvent?.Invoke();
    }

    // Opcional: Evento para notificar que fue soltado
    public void OnReleased()
    {
    // You can play sounds, particles, etc.
    Debug.Log($"{name} was released!");
    }

    // Opcional: Evento para notificar que fue agarrado
    public void OnGrabbed()
    {
    Debug.Log($"{name} was grabbed!");
    }
}