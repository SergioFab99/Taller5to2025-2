using UnityEngine;

public class Bottle : MonoBehaviour
{
    public float explosionDamage = 35f;
    public float fireDuration = 5f;
    public GameObject fireAreaPrefab; 

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            Kaboom();
        }
    }

    public void Kaboom()
    {
        Debug.Log("Molotov explodes!");
        if (fireAreaPrefab != null)
        {
            Instantiate(fireAreaPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
