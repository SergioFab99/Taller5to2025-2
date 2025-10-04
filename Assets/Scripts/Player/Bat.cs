using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class Bat : MonoBehaviour
{
    [Header("Bat Hold Point")]
    public Transform HoldPoint; // Should be at the grip location/orientation
    private GrabbableObject grabbable;
    public int damage = 2;

    [Header("Simple Swing Settings")]
    [Tooltip("Max rotation angle in degrees for the forward strike.")]
    public float swingAngle = 95f;
    [Tooltip("Time for forward (attack) phase.")]
    public float forwardDuration = 0.11f;
    [Tooltip("Time for return phase.")]
    public float returnDuration = 0.14f;
    [Tooltip("Easing curve for forward phase.")]
    public AnimationCurve forwardCurve = AnimationCurve.EaseInOut(0,0,1,1);
    [Tooltip("Easing curve for return phase.")]
    public AnimationCurve returnCurve = AnimationCurve.EaseInOut(0,0,1,1);
    [Tooltip("If true uses a horizontal (side) swing, else a downward (overhand) swing.")]
    public bool horizontalSwing = false;

    private Coroutine swingRoutine;
    [Header("Hit Filtering")] 
    [Tooltip("If true, the target must have a TagContainer with a tag named 'Damagable'. If false, any HealthController/EnemyLife is valid.")]
    [SerializeField] private bool requireDamagableTag = true;
    [Tooltip("Name of the tag in TagContainer to accept as damageable.")]
    [SerializeField] private string damagableTagName = "Damagable";
    [Tooltip("Optional layer mask filter for raycast & collision validation (ignored if set to Everything)." )]
    [SerializeField] private LayerMask hitLayers = ~0;
    [Tooltip("Extra sphere radius (0 = ray) used when calling Hit() to make contact more forgiving.")]
    [SerializeField] private float hitAssistRadius = 0f;

    // Track already hit targets (EnemyLife OR HealthController) during a swing
    private readonly HashSet<UnityEngine.Object> _hitThisSwing = new HashSet<UnityEngine.Object>();
    private bool _isSwinging = false;

    private void Awake()
    {
        grabbable = GetComponent<GrabbableObject>();
        if (grabbable != null)
        {
            grabbable.OnBrokenEvent += HandleBroken;
        }
    }

    private void OnDestroy()
    {
        if (grabbable != null)
        {
            grabbable.OnBrokenEvent -= HandleBroken;
        }
    }

    private void HandleBroken()
    {
        // Optionally spawn particles or play sound here before destroy
        Destroy(gameObject);
    }

    // Called by PlayerCombat when attack is performed
    public void Hit(Transform origin, Vector3 direction, float range)
    {
        if (grabbable != null && grabbable.IsBroken)
        {
            Debug.Log("Bat is broken! Cannot hit.");
            return;
        }
        RaycastHit hit;
        bool gotHit = false;
        if (hitAssistRadius > 0.001f)
        {
            gotHit = Physics.SphereCast(origin.position, hitAssistRadius, direction, out hit, range, hitLayers, QueryTriggerInteraction.Ignore);
        }
        else
        {
            gotHit = Physics.Raycast(origin.position, direction, out hit, range, hitLayers, QueryTriggerInteraction.Ignore);
        }
        if (gotHit)
        {
            TryApplyDamage(hit.collider);
        }
        // SwingBat coroutine should be triggered externally when the correct hand swings
    }

    // Legacy simple swing (kept for fallback / debugging)
    // Public entry point for a swing. Provide the player's facing (root or camera) transform.
    public void PlaySwing(Transform facing)
    {
        if (swingRoutine != null) StopCoroutine(swingRoutine);
        swingRoutine = StartCoroutine(SwingRoutine(facing));
    }

    private IEnumerator SwingRoutine(Transform facing)
    {
        if (HoldPoint == null)
        {
            Debug.LogWarning("Bat swing called but HoldPoint is null.");
            yield break;
        }
        if (facing == null) facing = transform; // fallback

        // Determine axis based on desired style
        Vector3 axis = horizontalSwing ? facing.up : facing.right;
        Vector3 pivot = HoldPoint.position;

        // Cache original local transform to guarantee perfect restore
        Vector3 originalLocalPos = transform.localPosition;
        Quaternion originalLocalRot = transform.localRotation;

        // Prepare physics freeze if needed
        Rigidbody rb = GetComponent<Rigidbody>();
        bool hadRb = rb != null;
        bool prevKin = false;
        if (hadRb)
        {
            prevKin = rb.isKinematic;
            rb.isKinematic = true;
        }

        // Start swing tracking
        _isSwinging = true;
    _hitThisSwing.Clear();

        // We'll rotate by computing delta quaternions around pivot, keeping manual control of position
        Vector3 pivotToBat = transform.position - pivot;
        Quaternion batRot = transform.rotation;

        float elapsed = 0f;
        float lastAngle = 0f;
        while (elapsed < forwardDuration)
        {
            float t = Mathf.Clamp01(elapsed / forwardDuration);
            float eval = forwardCurve != null ? forwardCurve.Evaluate(t) : t;
            float targetAngle = eval * swingAngle;
            float delta = targetAngle - lastAngle;
            Quaternion dq = Quaternion.AngleAxis(delta, axis);
            pivotToBat = dq * pivotToBat;
            batRot = dq * batRot;
            transform.position = pivot + pivotToBat;
            transform.rotation = batRot;
            lastAngle = targetAngle;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Return
        elapsed = 0f;
        while (elapsed < returnDuration)
        {
            float t = Mathf.Clamp01(elapsed / returnDuration);
            float eval = returnCurve != null ? returnCurve.Evaluate(t) : t;
            float targetAngle = Mathf.Lerp(swingAngle, 0f, eval);
            float delta = targetAngle - lastAngle;
            Quaternion dq = Quaternion.AngleAxis(delta, axis);
            pivotToBat = dq * pivotToBat;
            batRot = dq * batRot;
            transform.position = pivot + pivotToBat;
            transform.rotation = batRot;
            lastAngle = targetAngle;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Restore exactly
        transform.localPosition = originalLocalPos;
        transform.localRotation = originalLocalRot;
        if (hadRb) rb.isKinematic = prevKin;
        _isSwinging = false;
        swingRoutine = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (grabbable != null && grabbable.IsBroken) return;
        if (((1 << collision.collider.gameObject.layer) & hitLayers) != 0)
        {
            TryApplyDamage(collision.collider);
        }
    }

    // Centralized damage application with per-swing duplication guard.
    private void TryApplyDamage(Collider col)
    {
        if (col == null) return;
        // Climb up hierarchy to find damage-relevant components
        TagContainer tagContainer = col.GetComponent<TagContainer>();
        if (tagContainer == null) tagContainer = col.GetComponentInParent<TagContainer>();

        EnemyLife enemyLife = col.GetComponent<EnemyLife>();
        if (enemyLife == null) enemyLife = col.GetComponentInParent<EnemyLife>();

        HealthController health = null;
        if (enemyLife != null)
        {
            health = enemyLife.healthController; // enemyLife ensures healthController is initialized
        }
        else
        {
            health = col.GetComponent<HealthController>();
            if (health == null) health = col.GetComponentInParent<HealthController>();
        }

        // Filter by tag if required
        if (requireDamagableTag)
        {
            if (tagContainer == null || !tagContainer.HasTag(damagableTagName)) return;
        }
        else
        {
            // If not requiring tag, still need some health target
            if (enemyLife == null && health == null) return;
        }

        // Nothing to damage
        if (enemyLife == null && health == null) return;

        // Determine identity object for per-swing dedupe
    UnityEngine.Object identity = (UnityEngine.Object)enemyLife ?? (UnityEngine.Object)health;
        if (_isSwinging && _hitThisSwing.Contains(identity)) return;
        if (_isSwinging) _hitThisSwing.Add(identity);

        // Apply damage
        if (enemyLife != null)
        {
            enemyLife.TakeDamage(damage); // passes explicit damage
        }
        else if (health != null)
        {
            health.TakeDamague(damage);
        }

        if (grabbable != null) grabbable.Use();
        Debug.Log($"Bat hit target: {identity.name} via collider {col.name}");
    }
}
