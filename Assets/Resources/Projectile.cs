using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;

    private float timer = 0f;
    private Rigidbody rb;
    private PoolingTestManager poolManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("el prefab necesita rb");
        }
    }

    public void Launch(Vector3 direction, PoolingTestManager manager)
    {
        gameObject.SetActive(true);
        timer = 0f;
        poolManager = manager;
        rb.linearVelocity = direction.normalized * speed;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            ReturnToPool();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Obstacle"))
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (poolManager != null)
        {
            poolManager.ReturnToPool(this);
        }
        else
        {
            Debug.LogWarning("Projectile sin referencia al pool. Destruyendo...");
            Destroy(gameObject);
        }
    }
}