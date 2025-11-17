using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Playables;

public class PickUpWeapon : MonoBehaviour
{
    [SerializeField]
    [AssetsOnly]
    public CombatBase attacker;
    WeaponType Wtype = WeaponType.Bat;
    public GameObject Prefab;
    public WeaponSettings settings;
    public LayerMask hitMask;

    public GameObject point1, point2;

    public bool hited = true;

    public void Update()
    {
        if(!hited)
        {
            Throwed();
        }
    }
    public void Throwed()
    {
        var col = Physics.OverlapCapsule(point1.transform.position,point2.transform.position, 0.3f, hitMask.value, QueryTriggerInteraction.Ignore);
        if (col != null && col.Length > 0)
        {
            foreach (Collider coll in col)
            {
                if (coll.gameObject.TryGetComponent<TagContainer>(out TagContainer TagC) && TagC.HasTag("damagable"))
                {

                    Debug.Log("hitted");
                    var distance = (col[0].ClosestPoint(transform.position) - transform.position).normalized;
                    HitInfo hitInfo = new HitInfo(coll, coll.ClosestPoint(transform.position), distance, (settings as FistsWeaponSettings).damague, Wtype, attacker);

                    PerformOnHit(hitInfo);


                    
                }
            }
        }
    }

    public void PerformOnHit(HitInfo hitInfo)
    {
        if (hitInfo.col.TryGetComponent<TagContainer>(out TagContainer tags))
        {
            if (tags.HasTag("Damagable") && !tags.HasTag("Player"))
            {
                Debug.Log($"Bat hit {hitInfo.col.name}");

                if (tags.TryGetComponent(out HealthController health))
                {
                    health.TakeDamague((settings as BatWeaponSettings).damague);

                }
                else
                {
                    health = tags.GetComponentInParent<HealthController>();
                    if (health != null )
                    {
                        health.TakeDamague((settings as BatWeaponSettings).damague);
                    }
                }

            }
            hited = true;
        }
        else if (hitInfo.col.GetComponentInParent<TagContainer>())
        {
            var tags1 = hitInfo.col.GetComponentInParent<TagContainer>();

            if (tags1.HasTag("Damagable") && !tags1.HasTag("Player"))
            {
                Debug.Log($"Bat hit {hitInfo.col.name}");

                if (tags1.TryGetComponent(out HealthController health))
                {
                    health.TakeDamague((settings as BatWeaponSettings).damague);
                }

            }
            hited = true;
        }
    }

    private void OnDrawGizmos()
    {
        var size = new Vector3(0.2f, 0.2f, 1f);
        Gizmos.DrawWireCube(transform.position, size);
    }
}
