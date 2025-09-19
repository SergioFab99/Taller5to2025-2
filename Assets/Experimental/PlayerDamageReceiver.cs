using UnityEngine;


public class PlayerDamageReceiver : MonoBehaviour
{
    public string enemyTag = "Enemy";

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag(enemyTag)) GameManager.Instance?.DamagePlayer(1);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(enemyTag)) GameManager.Instance?.DamagePlayer(1);
    }
}
