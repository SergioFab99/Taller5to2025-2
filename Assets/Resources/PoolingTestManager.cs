using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class PoolingTestManager : MonoBehaviour
{
    [Header("Prefab y Spawn")]
    public GameObject projectilePrefab;
    public Transform spawnPoint;

    [Header("Configuración del Pool")]
    [Tooltip("Número inicial de proyectiles pre-instanziados")]
    public int defaultCapacity = 20;

    [Tooltip("Máximo número de proyectiles permitidos (0 = ilimitado)")]
    public int maxSize = 100;

    private ObjectPool<Projectile> projectilePool;

    private void Start()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("PoolingTestManager: Falta asignar el Projectile Prefab.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("PoolingTestManager: Falta asignar el Spawn Point.");
            return;
        }

        
        projectilePool = new ObjectPool<Projectile>(
            createFunc: () => Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity, transform).GetComponent<Projectile>(),
            actionOnGet: (proj) => {
                proj.gameObject.SetActive(true);
                
                proj.transform.position = spawnPoint.position;
                proj.transform.rotation = spawnPoint.rotation;
            },
            actionOnRelease: (proj) => {
                proj.gameObject.SetActive(false);
                
                Rigidbody rb = proj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            },
            actionOnDestroy: (proj) => Destroy(proj.gameObject),
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    public void FireWithoutPooling()
    {
        GameObject bullet = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        Projectile proj = bullet.GetComponent<Projectile>();
        if (proj != null)
        {
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = spawnPoint.forward * proj.speed;
            Destroy(bullet, proj.lifeTime);
        }
    }


    public void FireWithPooling()
    {
        Projectile proj = projectilePool.Get();
        if (proj != null)
        {
            proj.Launch(spawnPoint.forward, this);
        }
    }


    public void ReturnToPool(Projectile proj)
    {
        if (proj != null && projectilePool != null)
        {
            projectilePool.Release(proj);
        }
    }

   
    private void OnDestroy()
    {
        projectilePool?.Dispose();
    }
}