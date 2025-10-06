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

    // Animación por código removida
    // Si necesitas animación, usa Animator o Animation en el prefab
    // El código de swing ha sido deshabilitado
    //
    // private Coroutine swingRoutine;
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

    // Animación por código removida
    // Convierte el collider a trigger cuando el bat está agarrado
    public void SetColliderTrigger(bool isTrigger)
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = isTrigger;
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
