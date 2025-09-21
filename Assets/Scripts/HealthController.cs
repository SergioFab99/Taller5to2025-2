using Unity.Mathematics.Geometry;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public float health;

    public float maxHealth;

    public event LifeChangued OnLifeChangue;
    public delegate void LifeChangued(float changue);

    public void AddHealth(float lifeAdded)
    {

        var healing = health + lifeAdded;
        health =Mathf.Max(healing, maxHealth);
        OnLifeChangue?.Invoke(lifeAdded);
    }

    public void TakeDamague(float damague)
    {
        var damaged = health + damague;
        health = Mathf.Min(0, damaged);
        OnLifeChangue?.Invoke(damaged);
    }
}
