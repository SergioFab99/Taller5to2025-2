using UnityEngine;

public class TestingStatuses : MonoBehaviour
{
    public float effectDuration = 5f;

    private void OnCollisionEnter(Collision collision)
    {
        EnemyMain enemy = collision.collider.GetComponent<EnemyMain>();
        if (enemy != null)
        {
            StatusEffect effect = (StatusEffect)Random.Range(0, 3);
            enemy.ApplyStatus(effect, effectDuration);

            Debug.Log($"{enemy.name} inflicted with {effect} for {effectDuration} seconds");
        }
    }
}
