using UnityEngine;

public class Flame : MonoBehaviour
{
    public float dps = 2f;
    public float duration = 5f;

    private void Start()
    {
        Destroy(gameObject, duration);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player takes fire damage");
            // Apply DPS tick to player health here
        }
    }
}
