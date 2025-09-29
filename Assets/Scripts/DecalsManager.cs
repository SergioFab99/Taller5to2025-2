using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
[AddComponentMenu("Rendering/Gestor de Decals")]
public class DecalsManager : MonoBehaviour
{
    private static readonly string[] FallbackColorProperties =
    {
        "_BaseColor",
        "_Color",
        "_TintColor"
    };

    [Header("Prefab y Pool de Decals")]
    [SerializeField] private GameObject decalPrefab;
    [SerializeField] private int initialPoolSize = 6;
    [SerializeField] private bool autoExpandPool = true;

    [Header("Ciclo de Vida")]
    [Tooltip("Segundos que un decal permanece totalmente visible antes de iniciar el desvanecimiento.")]
    [SerializeField] private float visibleDuration = 3f;
    [Tooltip("Segundos que tarda el decal en desvanecerse después del periodo visible.")]
    [SerializeField] private float fadeDuration = 1.5f;
    [Tooltip("Destruye el GameObject del decal en lugar de regresarlo al pool cuando finalice el ciclo.")]
    [SerializeField] private bool destroyOnComplete = false;

    [Header("Movimiento Sutil")]
    [Tooltip("Movimiento local máximo por eje mientras el decal está activo.")]
    [SerializeField] private Vector3 movementAmplitude = new Vector3(0.05f, 0.02f, 0.05f);
    [Tooltip("Multiplicador de velocidad de oscilación para el movimiento sutil.")]
    [SerializeField] private float movementFrequency = 1.25f;
    [Tooltip("Aplica el movimiento en espacio local cuando es verdadero; en caso contrario, en espacio mundial.")]
    [SerializeField] private bool useLocalMovement = true;
    [Tooltip("Aleatoriza la fase inicial del movimiento sutil para que los decals no se muevan sincronizados.")]
    [SerializeField] private bool randomizeMovementPhase = true;

    [Header("Desvanecimiento")]
    [Tooltip("Propiedad de color del shader usada para controlar la opacidad. Déjalo vacío para detectar automáticamente propiedades comunes.")]
    [SerializeField] private string colorPropertyName = "_BaseColor";
    [Tooltip("Controla la curva de desvanecimiento. X es el tiempo de desvanecimiento normalizado y Y la opacidad resultante (1 = completamente opaco).")]
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    [Header("Intervalo de Aparición")]
    [Tooltip("Alterna automáticamente la visibilidad de cada decal activándolo y desactivándolo en intervalos.")]
    [SerializeField] private bool habilitarIntervalo = false;
    [Tooltip("Segundos que el decal permanece activo durante el intervalo.")]
    [SerializeField] private float intervaloTiempoActivo = 2f;
    [Tooltip("Segundos que el decal permanece inactivo antes de volver a activarse.")]
    [SerializeField] private float intervaloTiempoInactivo = 2f;
    [Tooltip("Retraso inicial antes de iniciar el primer ciclo de intervalo.")]
    [SerializeField] private float intervaloRetrasoInicial = 0f;

    private readonly List<DecalInstance> activeDecals = new();
    private readonly Queue<DecalInstance> availableDecals = new();
    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();

        if (decalPrefab == null)
        {
            Debug.LogWarning($"{nameof(DecalsManager)} en '{name}' no tiene asignada una referencia de prefab de decal.", this);
            return;
        }

        if (destroyOnComplete)
        {
            return;
        }

        for (int i = 0; i < initialPoolSize; i++)
        {
            availableDecals.Enqueue(CreateInstance());
        }
    }

    private void Update()
    {
        if (activeDecals.Count == 0)
        {
            return;
        }

        float currentTime = Time.time;

        for (int i = activeDecals.Count - 1; i >= 0; i--)
        {
            DecalInstance instance = activeDecals[i];
            float elapsed = currentTime - instance.spawnTime;

            ApplyMovement(instance, currentTime);

            if (habilitarIntervalo)
            {
                HandleInterval(instance, currentTime);
                continue;
            }

            if (elapsed < visibleDuration)
            {
                continue;
            }

            float fadeElapsed = elapsed - visibleDuration;
            float fadeT = fadeDuration <= Mathf.Epsilon ? 1f : Mathf.Clamp01(fadeElapsed / fadeDuration);
            ApplyFade(instance, fadeCurve.Evaluate(fadeT));

            if (fadeElapsed >= fadeDuration)
            {
                FinalizeInstance(instance);
                activeDecals.RemoveAt(i);
            }
        }
    }

    public GameObject SpawnDecal(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (decalPrefab == null)
        {
            Debug.LogError($"No se puede instanciar un decal porque '{nameof(decalPrefab)}' no está asignado.", this);
            return null;
        }

        DecalInstance instance = ObtainInstance();
        if (instance == null)
        {
            Debug.LogWarning("El gestor de decals no pudo obtener una instancia.", this);
            return null;
        }

        Transform targetParent = parent != null ? parent : transform;
        instance.transform.SetParent(targetParent, true);
        instance.transform.SetPositionAndRotation(position, rotation);

        instance.spawnTime = Time.time;
        instance.originalWorldPosition = instance.transform.position;
        instance.originalLocalPosition = instance.transform.localPosition;
        instance.randomSeed = randomizeMovementPhase ? Random.insideUnitSphere * 32f : Vector3.zero;

        if (habilitarIntervalo)
        {
            if (intervaloRetrasoInicial > 0f)
            {
                instance.intervalActive = false;
                instance.nextIntervalChangeTime = Time.time + Mathf.Max(Mathf.Epsilon, intervaloRetrasoInicial);
            }
            else
            {
                instance.intervalActive = true;
                instance.nextIntervalChangeTime = Time.time + Mathf.Max(Mathf.Epsilon, intervaloTiempoActivo);
            }
        }
        else
        {
            instance.intervalActive = true;
            instance.nextIntervalChangeTime = 0f;
        }

        ResetFade(instance);
        bool activarAhora = !habilitarIntervalo || (habilitarIntervalo && (intervaloRetrasoInicial <= 0f));
        instance.gameObject.SetActive(activarAhora);
        if (!activarAhora)
        {
            instance.transform.localPosition = instance.originalLocalPosition;
            instance.transform.position = instance.originalWorldPosition;
        }

        activeDecals.Add(instance);
        return instance.gameObject;
    }

    public void ClearAllDecals(bool destroy = false)
    {
        for (int i = activeDecals.Count - 1; i >= 0; i--)
        {
            DecalInstance instance = activeDecals[i];
            if (destroy)
            {
                Destroy(instance.gameObject);
            }
            else
            {
                FinalizeInstance(instance, enqueue: !destroyOnComplete);
            }
        }

        activeDecals.Clear();

        if (destroy)
        {
            availableDecals.Clear();
        }
    }

    private void ApplyMovement(DecalInstance instance, float currentTime)
    {
        if (!instance.gameObject.activeSelf)
        {
            return;
        }

        if (movementAmplitude == Vector3.zero || movementFrequency <= Mathf.Epsilon)
        {
            return;
        }

        float time = currentTime + instance.randomSeed.x;
        float sinX = Mathf.Sin(time * movementFrequency + instance.randomSeed.x);
        float sinY = Mathf.Sin(time * movementFrequency + instance.randomSeed.y);
        float sinZ = Mathf.Sin(time * movementFrequency + instance.randomSeed.z);

        Vector3 offset = new Vector3(
            movementAmplitude.x * sinX,
            movementAmplitude.y * sinY,
            movementAmplitude.z * sinZ
        );

        if (useLocalMovement)
        {
            instance.transform.localPosition = instance.originalLocalPosition + offset;
        }
        else
        {
            instance.transform.position = instance.originalWorldPosition + offset;
        }
    }

    private void HandleInterval(DecalInstance instance, float currentTime)
    {
        if (intervaloTiempoActivo <= 0f && intervaloTiempoInactivo <= 0f)
        {
            return;
        }

        if (currentTime < instance.nextIntervalChangeTime)
        {
            return;
        }

        if (instance.intervalActive)
        {
            instance.intervalActive = false;
            instance.gameObject.SetActive(false);
            instance.transform.localPosition = instance.originalLocalPosition;
            instance.transform.position = instance.originalWorldPosition;
            instance.nextIntervalChangeTime = currentTime + Mathf.Max(Mathf.Epsilon, intervaloTiempoInactivo);
        }
        else
        {
            instance.intervalActive = true;
            instance.transform.localPosition = instance.originalLocalPosition;
            instance.transform.position = instance.originalWorldPosition;
            instance.gameObject.SetActive(true);
            ResetFade(instance);
            instance.spawnTime = currentTime;
            instance.nextIntervalChangeTime = currentTime + Mathf.Max(Mathf.Epsilon, intervaloTiempoActivo);
        }
    }

    private void ApplyFade(DecalInstance instance, float opacity)
    {
        int rendererCount = instance.renderers.Length;
        for (int i = 0; i < rendererCount; i++)
        {
            if (!instance.supportsFade[i])
            {
                continue;
            }

            Renderer renderer = instance.renderers[i];
            int propertyId = instance.colorPropertyIds[i];

            Color baseColor = instance.baseColors[i];
            baseColor.a *= Mathf.Clamp01(opacity);

            propertyBlock.Clear();
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(propertyId, baseColor);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }

    private void ResetFade(DecalInstance instance)
    {
        int rendererCount = instance.renderers.Length;
        for (int i = 0; i < rendererCount; i++)
        {
            Renderer renderer = instance.renderers[i];

            if (!instance.supportsFade[i])
            {
                renderer.SetPropertyBlock(null);
                continue;
            }

            int propertyId = instance.colorPropertyIds[i];
            propertyBlock.Clear();
            propertyBlock.SetColor(propertyId, instance.baseColors[i]);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }

    private DecalInstance ObtainInstance()
    {
        if (destroyOnComplete)
        {
            return CreateInstance();
        }

        if (availableDecals.Count > 0)
        {
            return availableDecals.Dequeue();
        }

        if (!autoExpandPool)
        {
            Debug.LogWarning("El pool de decals se agotó y la expansión automática está deshabilitada. Considera aumentar el tamaño del pool.", this);
            return null;
        }

        return CreateInstance();
    }

    private DecalInstance CreateInstance()
    {
        if (decalPrefab == null)
        {
            return null;
        }

        GameObject go = Instantiate(decalPrefab, transform);
        go.SetActive(false);

        DecalInstance instance = new(go);
        ConfigureRendererData(instance);
        return instance;
    }

    private void ConfigureRendererData(DecalInstance instance)
    {
        Renderer[] renderers = instance.renderers;
        int rendererCount = renderers.Length;

        for (int i = 0; i < rendererCount; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null)
            {
                instance.supportsFade[i] = false;
                continue;
            }

            int propertyId = ResolveColorPropertyId(renderer);
            instance.colorPropertyIds[i] = propertyId;

            if (propertyId == -1)
            {
                instance.supportsFade[i] = false;
                continue;
            }

            instance.supportsFade[i] = true;

            Material sharedMaterial = renderer.sharedMaterial;
            if (sharedMaterial != null && sharedMaterial.HasProperty(propertyId))
            {
                instance.baseColors[i] = sharedMaterial.GetColor(propertyId);
            }
            else
            {
                instance.baseColors[i] = Color.white;
            }
        }
    }

    private int ResolveColorPropertyId(Renderer renderer)
    {
        if (renderer == null)
        {
            return -1;
        }

        Material material = renderer.sharedMaterial;
        if (material == null)
        {
            return -1;
        }

        if (!string.IsNullOrWhiteSpace(colorPropertyName) && material.HasProperty(colorPropertyName))
        {
            return Shader.PropertyToID(colorPropertyName);
        }

        foreach (string fallback in FallbackColorProperties)
        {
            if (material.HasProperty(fallback))
            {
                return Shader.PropertyToID(fallback);
            }
        }

        return -1;
    }

    private void FinalizeInstance(DecalInstance instance, bool enqueue = true)
    {
        if (destroyOnComplete && enqueue)
        {
            enqueue = false;
        }

        if (enqueue)
        {
            instance.transform.SetParent(transform, false);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.gameObject.SetActive(false);
            instance.intervalActive = true;
            instance.nextIntervalChangeTime = 0f;
            availableDecals.Enqueue(instance);
        }
        else if (destroyOnComplete)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            instance.transform.SetParent(transform, false);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.gameObject.SetActive(false);
            instance.intervalActive = true;
            instance.nextIntervalChangeTime = 0f;
        }
    }

    private void OnValidate()
    {
        visibleDuration = Mathf.Max(0f, visibleDuration);
        fadeDuration = Mathf.Max(0f, fadeDuration);
        initialPoolSize = Mathf.Max(0, initialPoolSize);
        movementFrequency = Mathf.Max(0f, movementFrequency);
        movementAmplitude.x = Mathf.Max(0f, movementAmplitude.x);
        movementAmplitude.y = Mathf.Max(0f, movementAmplitude.y);
        movementAmplitude.z = Mathf.Max(0f, movementAmplitude.z);
        intervaloTiempoActivo = Mathf.Max(0f, intervaloTiempoActivo);
        intervaloTiempoInactivo = Mathf.Max(0f, intervaloTiempoInactivo);
        intervaloRetrasoInicial = Mathf.Max(0f, intervaloRetrasoInicial);
    }

    private sealed class DecalInstance
    {
        public readonly GameObject gameObject;
        public readonly Transform transform;
        public readonly Renderer[] renderers;
        public readonly int[] colorPropertyIds;
        public readonly Color[] baseColors;
        public readonly bool[] supportsFade;

        public float spawnTime;
        public Vector3 originalWorldPosition;
        public Vector3 originalLocalPosition;
        public Vector3 randomSeed;
        public bool intervalActive;
        public float nextIntervalChangeTime;

        public DecalInstance(GameObject go)
        {
            gameObject = go;
            transform = go.transform;
            renderers = go.GetComponentsInChildren<Renderer>(true);

            int rendererCount = renderers.Length;
            colorPropertyIds = new int[rendererCount];
            baseColors = new Color[rendererCount];
            supportsFade = new bool[rendererCount];
            intervalActive = true;
            nextIntervalChangeTime = 0f;
        }
    }
}
