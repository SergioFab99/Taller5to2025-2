using UnityEngine;

public class EBullet : MonoBehaviour
{
    public float speed = 30f;
    public float lifeTime = 3f;
    public float damage = 25f;

    private EnemyMain owner;

    void Start()
    {
        Destroy(gameObject, lifeTime); 
    }

    public void Init(EnemyMain shooter, float dmg)
    {
        owner = shooter;
        damage = dmg;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log($"Player hit for {damage} damage by bullet");
        }

        Destroy(gameObject);
    }
}
