using UnityEngine;
using UnityEngine.Serialization;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using TMPro;

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
    [SerializeField] private Canvas uiCanvas; // (Legacy) Canvas world-space opcional (ya no necesario si usas overlay)

    [Header("Prompt Overlay (TextMeshPro)")]
    [Tooltip("Usar prompt overlay global en vez de un canvas world-space local.")]
    [SerializeField] private bool useOverlayPrompt = true;
    [Tooltip("Texto base que se mostrará junto a la tecla.")]
    [SerializeField] private string promptVerb = "Agarrar";
    [Tooltip("Formato para mostrar. {0}=tecla, {1}=verbo")] 
    [SerializeField] private string promptFormat = "[{0}] {1}";
    [Tooltip("Clave por defecto si no se puede resolver del Input System.")]
    [SerializeField] private string fallbackKey = "E";

    // Cache de clave de interacción
    private static string _cachedInteractKey;
    private static double _lastInteractKeyTime;
    private const double KeyCacheRefreshSeconds = 2.0; // refrescar cada cierto tiempo por si cambia el dispositivo

    private bool _wasLooking; // para detectar transición mostrar/ocultar
    [Header("Debug Prompt")]
    [SerializeField] private bool debugPrompt = false;
    private static bool _reportedMissingPromptInstance = false;

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
            uiCanvas.gameObject.SetActive(false); // oculto por defecto
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
            // Aceptar colliders hijos: buscar componente en padres
            var grabbed = hit.collider.GetComponentInParent<GrabbableObject>();
            hitThis = (grabbed == this);
        }

        // Highlight logic
        if (showGlowWhenNear && _renderer != null)
        {
            _renderer.material = hitThis ? _highlightMaterial : _originalMaterial;
        }
        
        // World-space local canvas (legacy path)
        if (!useOverlayPrompt && uiCanvas != null)
        {
            uiCanvas.gameObject.SetActive(showCanvasWhenNear && hitThis);
        }

        // Overlay prompt path
        if (useOverlayPrompt)
        {
            if (hitThis)
            {
                string key = ResolveInteractKey();
                string text = string.Format(promptFormat, key, promptVerb);
                if (InteractionPromptExists())
                {
                    InteractionPrompt.Show(text);
                }
                else if (debugPrompt && !_reportedMissingPromptInstance)
                {
                    Debug.LogWarning("[GrabbableObject] No InteractionPrompt instance found in scene. Create one (Canvas + InteractionPrompt script).", this);
                    _reportedMissingPromptInstance = true;
                }
                _wasLooking = true;
            }
            else if (_wasLooking)
            {
                if (InteractionPromptExists())
                {
                    InteractionPrompt.Hide(this);
                }
                _wasLooking = false;
            }
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

    private string ResolveInteractKey()
    {
#if ENABLE_INPUT_SYSTEM
        // Cache to avoid string allocations cada frame
        if (string.IsNullOrEmpty(_cachedInteractKey) || (Time.realtimeSinceStartupAsDouble - _lastInteractKeyTime) > KeyCacheRefreshSeconds)
        {
            try
            {
                var actions = _inputActions ?? (_inputActions = new PlayerInputActions());
                if (!_inputActionsEnabled)
                {
                    actions.Enable();
                    _inputActionsEnabled = true;
                }
                var interact = actions.Player.Interact;
                // Intentar obtener binding legible (prioridad teclado)
                string display = interact.GetBindingDisplayString(bindingMask: InputBinding.MaskByGroup("Keyboard&Mouse"));
                if (string.IsNullOrEmpty(display))
                {
                    display = interact.GetBindingDisplayString();
                }
                _cachedInteractKey = string.IsNullOrEmpty(display) ? fallbackKey : display;
                _lastInteractKeyTime = Time.realtimeSinceStartupAsDouble;
            }
            catch
            {
                _cachedInteractKey = fallbackKey;
            }
        }
        return _cachedInteractKey;
#else
        return fallbackKey;
#endif
    }

#if ENABLE_INPUT_SYSTEM
    private static PlayerInputActions _inputActions;
    private static bool _inputActionsEnabled;
#endif

    private bool InteractionPromptExists()
    {
        // Usa búsqueda rápida (solo si necesario) – la clase singleton controla duplicados
        // Para evitar dependencias directas reflejamos a través de tipo
        return UnityEngine.Object.FindFirstObjectByType<InteractionPrompt>() != null;
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