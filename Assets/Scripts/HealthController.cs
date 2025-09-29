using Unity.Mathematics.Geometry;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public float health;

    public float maxHealth;

    public event LifeChangued OnLifeChangue;
    public delegate void LifeChangued(float changue);
    public event Live OnDead;
    public delegate void Live();
    public void AddHealth(float lifeAdded)
    {

        var healing = health + lifeAdded;
        health =Mathf.Max(healing, maxHealth);
        OnLifeChangue?.Invoke(lifeAdded);
    }

    public void TakeDamague(float damague)
    {
        var damaged = health - damague;
        OnLifeChangue?.Invoke(damaged);
        health = damaged;
        if(health == 0)
        {
            OnDead?.Invoke();
        }
    }
}
