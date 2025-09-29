using UnityEngine;
using UnityEngine.UI;
using System;

// Put this on a Screen Space - Overlay (or Screen Space - Camera) canvas object.
// Shows a single enemy health bar when the player is LOOKING at an enemy (center screen ray hits a HealthController).
// Does NOT spawn per-enemy world bars; instead it targets the one in crosshair.
public class EnemyTargetHUD : MonoBehaviour
{
    [Header("UI References")] 
    [Tooltip("Slider that represents the enemy health.")]
    [SerializeField] private Slider slider;
    [Tooltip("CanvasGroup for fading the whole HUD.")]
    [SerializeField] private CanvasGroup canvasGroup;
    [Tooltip("Optional text label for enemy name (can be left null).")]
    [SerializeField] private Text nameLabel; // If you later switch to TextMeshPro just adapt type.

    [Header("Detection")] 
    [Tooltip("Camera used for the center-screen ray. If null, will use Camera.main each frame.")]
    [SerializeField] private Camera targetCamera;
    [Tooltip("Max distance to detect an enemy when looking at it.")]
    [SerializeField] private float maxDetectDistance = 60f;
    [Tooltip("Layers considered enemies. Leave empty to hit everything then filter by components.")]
    [SerializeField] private LayerMask enemyLayers = ~0; // default all
    [Tooltip("If true, only objects with this tag AND a HealthController qualify.")]
    [SerializeField] private bool useTagFilter = false;
    [SerializeField] private string enemyTag = "Enemy";
    [Tooltip("Optional extra sphere radius for the ray (spherecast) to make acquisition less pixel-perfect.")]
    [SerializeField] private float aimAssistRadius = 0.05f;

    [Header("Behaviour")] 
    [Tooltip("Hide bar when the enemy is at full health.")]
    [SerializeField] private bool hideWhenFull = true;
    [Tooltip("Seconds the bar remains visible after you stop looking at the enemy.")]
    [SerializeField] private float lingerTime = 0.35f;
    [Tooltip("Fade speed for showing / hiding.")]
    [SerializeField] private float fadeSpeed = 8f;
    [Tooltip("Update rate for detection. Lower for performance, higher for responsiveness.")]
    [SerializeField] private float detectionInterval = 0.05f;

    [Header("Debug")]
    [SerializeField] private bool debugDrawRay = false;

    private HealthController _current;
    private float _lastSeenTime;
    private float _nextDetectTime;
    private bool _wantsVisible;

    private void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (targetCamera == null) targetCamera = Camera.main;
        if (canvasGroup != null) canvasGroup.alpha = 0f;
        if (slider != null)
        {
            slider.minValue = 0f;
        }
    }

    private void OnDisable()
    {
        UnsubscribeCurrent();
        if (canvasGroup != null) canvasGroup.alpha = 0f;
        _current = null;
    }

    private void Update()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        float time = Time.time;

        if (time >= _nextDetectTime)
        {
            _nextDetectTime = time + detectionInterval;
            DetectTarget();
        }

        // Visibility decision
        _wantsVisible = false;
        if (_current != null)
        {
            bool full = Mathf.Approximately(_current.health, _current.maxHealth);
            if (!(hideWhenFull && full))
            {
                // Either still looking or within linger time.
                if (IsLookingAtCurrent() || time - _lastSeenTime <= lingerTime)
                    _wantsVisible = true;
            }
        }

        if (canvasGroup != null)
        {
            float targetAlpha = _wantsVisible ? 1f : 0f;
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
        }

        if (debugDrawRay && targetCamera != null)
        {
            Debug.DrawRay(targetCamera.transform.position, targetCamera.transform.forward * maxDetectDistance, Color.red);
        }
    }

    private void DetectTarget()
    {
        if (targetCamera == null) return;
        Ray ray = new Ray(targetCamera.transform.position, targetCamera.transform.forward);
        RaycastHit hit;
        bool gotHit = false;

        if (aimAssistRadius > 0.001f)
        {
            gotHit = Physics.SphereCast(ray, aimAssistRadius, out hit, maxDetectDistance, enemyLayers, QueryTriggerInteraction.Ignore);
        }
        else
        {
            gotHit = Physics.Raycast(ray, out hit, maxDetectDistance, enemyLayers, QueryTriggerInteraction.Ignore);
        }

        if (gotHit)
        {
            var hc = hit.collider.GetComponentInParent<HealthController>();
            if (hc != null)
            {
                if (!useTagFilter || hit.collider.CompareTag(enemyTag) || hc.CompareTag(enemyTag) || hc.gameObject.CompareTag(enemyTag))
                {
                    if (hc != _current)
                    {
                        SwitchTarget(hc);
                    }
                    _lastSeenTime = Time.time;
                    return;
                }
            }
        }
        // If we reach here, no (valid) target this tick. Don't clear immediately—linger handles fade.
    }

    private bool IsLookingAtCurrent()
    {
        if (_current == null || targetCamera == null) return false;
        // Quick direction check (helps edge cases where spherecast still touches enemy while reticle moved far)
        Vector3 to = _current.transform.position - targetCamera.transform.position;
        float dot = Vector3.Dot(targetCamera.transform.forward.normalized, to.normalized);
        return dot > 0.8f; // ~36° cone
    }

    private void SwitchTarget(HealthController newTarget)
    {
        UnsubscribeCurrent();
        _current = newTarget;
        if (_current != null)
        {
            _current.OnHealthUpdated += OnTargetHealthUpdated;
            OnTargetHealthUpdated(_current.health, _current.maxHealth);
            _lastSeenTime = Time.time;
        }
    }

    private void UnsubscribeCurrent()
    {
        if (_current != null)
        {
            _current.OnHealthUpdated -= OnTargetHealthUpdated;
        }
    }

    private void OnTargetHealthUpdated(float current, float max)
    {
        if (slider != null)
        {
            slider.maxValue = max;
            slider.value = current;
        }
        if (nameLabel != null && _current != null)
        {
            nameLabel.text = _current.gameObject.name; // Placeholder; customize later
        }
    }
}
