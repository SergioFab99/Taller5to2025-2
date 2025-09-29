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
    // Track enemies already hit during the current swing so we don't apply damage multiple times.
    private readonly HashSet<EnemyLife> _enemiesHitThisSwing = new HashSet<EnemyLife>();
    private bool _isSwinging = false;

    private void Awake()
    {
        grabbable = GetComponent<GrabbableObject>();
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
        if (Physics.Raycast(origin.position, direction, out hit, range))
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
        _enemiesHitThisSwing.Clear();

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
        TryApplyDamage(collision.collider);
    }

    // Centralized damage application with per-swing duplication guard.
    private void TryApplyDamage(Collider col)
    {
        if (col == null) return;
        var tagContainer = col.GetComponent<TagContainer>();
        if (tagContainer == null || !tagContainer.HasTag("Damagable")) return;

        var enemy = col.GetComponent<EnemyLife>();
        if (enemy == null) return;

        // Prevent multiple hits on same enemy during a single swing
        if (_isSwinging && _enemiesHitThisSwing.Contains(enemy)) return;

        if (_isSwinging)
        {
            _enemiesHitThisSwing.Add(enemy);
        }

        enemy.TakeDamage(); // (Optional: pass damage value if EnemyLife supports it)
        if (grabbable != null) grabbable.Use();
        Debug.Log($"Bat hit damagable: {col.name}");
    }
}
