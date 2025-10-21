using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;





public class Punch : MonoBehaviour

{
    bool hitDone = false;
    Vector3 lastPos;
    public float radius = 0.2f;
    public Vector3 dir;
    public LayerMask hitMask;
    private bool isActive;

    public int dmg;

    public delegate void OnHitEvent(HitInfo hitInfo);
    public event OnHitEvent OnHit;

    private void Start()
    {
        lastPos = transform.position;
    }
    public void FixedUpdate()
    {
        if (hitDone)
        {
            return;
        }
        Vector3 currentPos = transform.position;
        dir = currentPos - lastPos;
        float dist = dir.magnitude;
        if (dist > 0.0001f)
        {
            RaycastHit hit;

            if (Physics.SphereCast(transform.position, radius, dir.normalized, out hit, dist, hitMask, QueryTriggerInteraction.Ignore))
            {
                if (isActive)
                {
                    HitInfo hitInfo = new HitInfo(hit.collider, hit.point, hit.normal, dmg);
                    
                    PerformOnHit(hitInfo);

                }
            }
        }
        lastPos = currentPos;
        
    }

    public void PerformOnHit(HitInfo hitInfo)
    {
        var enemyCharacter = hitInfo.col.GetComponentInParent<EnemyCharacter>().gameObject;
        if (enemyCharacter != null) 
        {
            
            if (enemyCharacter.TryGetComponent<TagContainer>(out TagContainer tagContainer) && tagContainer.HasTag("Damagable") && !tagContainer.HasTag("Player"))
            {
                Debug.Log("Hitted");
                var enemy = enemyCharacter.GetComponentInParent<Enemy>();
                if (enemy != null)
                {
                    hitInfo.enemyState = enemy.GetEnemyState();
                    enemyCharacter.TryGetComponent<HealthController>(out HealthController lifeController);
                    OnHit?.Invoke(hitInfo);
                    if (hitInfo.enemyState is BlockState)
                    {
                        lifeController.TakeDamague(dmg / 2);

                    }
                    else
                    {
                     
                        lifeController.TakeDamague(dmg);

                    }
                }
                hitDone = true;
            }
        
        }
        else
        {
           enemyCharacter = hitInfo.col.GetComponent<EnemyCharacter>().gameObject;
            if(enemyCharacter != null)
            {
                if (enemyCharacter.TryGetComponent<TagContainer>(out TagContainer tagContainer) && tagContainer.HasTag("Damagable") && !tagContainer.HasTag("Player"))
                {
                    Debug.Log("Hitted");
                    var enemy = enemyCharacter.GetComponentInParent<Enemy>();
                    if (enemy != null)
                    {
                        hitInfo.enemyState = enemy.GetEnemyState();
                        enemyCharacter.TryGetComponent<HealthController>(out HealthController lifeController);
                        OnHit?.Invoke(hitInfo);
                        if (hitInfo.enemyState is BlockState)
                        {
                            lifeController.TakeDamague(dmg / 2);

                        }
                        else
                        {

                            lifeController.TakeDamague(dmg);

                        }
                    }

                    if (hitInfo.col.gameObject.TryGetComponent<EnemyLife>(out EnemyLife enemyLife))
                    {
                        OnHit?.Invoke(hitInfo);
                        if (hitInfo.enemyState is BlockState)
                        {
                            enemyLife.TakeDamage(dmg / 2);

                        }
                        else
                        {
                            enemyLife.TakeDamage(dmg);

                        }

                    }
                    hitDone = true;
                }
            }
        }
    }

    public void ActivateOrDeactivePunch(bool trigger)
    {
        isActive = trigger;
        if (isActive)
        {
            hitDone = false;
            var col = Physics.OverlapSphere(transform.position, radius, hitMask.value, QueryTriggerInteraction.Ignore);
            if (col != null && col.Length > 0)
            {
                var distance = (col[0].ClosestPoint(transform.position) - transform.position).normalized;
                HitInfo hitInfo = new HitInfo(col[0], col[0].ClosestPoint(transform.position), distance, dmg);

                PerformOnHit(hitInfo);
            }


        }

    }

   
}
