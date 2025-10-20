using UnityEngine;

public class BluntProyectile : MonoBehaviour
{
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(this.gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log($"Blunt proyectile hit player ");
            Destroy(this.gameObject);
        }
    }
}
