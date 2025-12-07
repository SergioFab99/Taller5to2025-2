using System.Security.Cryptography;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Playables;

public class PickUpWeapon : MonoBehaviour
{
    [SerializeField]
    [AssetsOnly]
    public CombatBase attacker;
    public WeaponType Wtype;
    public GameObject Prefab;
    public WeaponSettings settings;
    public LayerMask hitMask;

    public GameObject point1, point2;

    public GameObject model, BrokenModel,internalobject;

    

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
                if (coll.gameObject.TryGetComponent<TagContainer>(out TagContainer TagC) && TagC.HasTag("Damagable") && !TagC.HasTag("Player"))
                {

                    Debug.Log("hitted");
                    var distance = (coll.ClosestPoint(transform.position) - transform.position).normalized;
                    float damage = settings != null ? settings.damague : 25f;
                    HitInfo hitInfo = new HitInfo(coll, coll.ClosestPoint(transform.position), distance, damage, Wtype, attacker);
                    PerformOnHit(hitInfo);
                    BreakObj();     
                     
                }

            }
        }
    }

    public void PerformOnHit(HitInfo hitInfo)
    {
        {
            var recv = hitInfo.col.GetComponent<CombatHitReceiver>()
                    ?? hitInfo.col.GetComponentInChildren<CombatHitReceiver>()
                    ?? hitInfo.col.GetComponentInParent<CombatHitReceiver>();

            if (recv != null)
            {
                recv.OnHit(hitInfo);
                hited = true;
                Debug.Log("Se ha hecho daño al objeto");
                Debug.Log(hitInfo.damague);
                Debug.Log(hitInfo.attacker);
                Debug.Log(hitInfo.type);
                return;
            } 
            else
            {
                Debug.Log("No se ha podido hacer daño al objeto");
                hited = false;
            }           
        }
    }

    public void BreakObj()
    {
        if (!hited) return;
        if (model == null || BrokenModel == null) return;
        model.SetActive(false);
        BrokenModel.SetActive(true);
        var tag = this.GetComponent<TagContainer>();
        Destroy(tag);
        if(Wtype==WeaponType.Crate)
                    {
                        Wood();
                    }     
        Destroy(this.gameObject, 3f);

    }

    public void Wood()
    {
        if (!hited) return;
        if (model == null || BrokenModel == null) return;
        var o = Instantiate(internalobject, transform.position, transform.rotation);
    }

    private void OnDrawGizmos()
    {
        var size = new Vector3(0.2f, 0.2f, 1f);
        Gizmos.DrawWireCube(transform.position, size);
    }
}
