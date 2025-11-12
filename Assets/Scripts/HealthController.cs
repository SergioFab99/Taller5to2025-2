using UnityEngine;
using System;

public class HealthController : MonoBehaviour
{
    [Header("Health Values")] public float maxHealth = 100f;
    public float health = 100f;

    public event LifeChangued OnLifeChangue;
    public delegate void LifeChangued(float changeAmount);
    public event Live OnDead;
    public delegate void Live();
    public event Action<float, float> OnHealthUpdated;

    private void Awake()
    {
        health = Mathf.Clamp(health, 0f, maxHealth);
        RaiseFullUpdate();
    }

    public void AddHealth(float amount)
    {
        if (amount <= 0f) return;
        float before = health;
        health = Mathf.Min(health + amount, maxHealth);
        float delta = health - before;
        if (Mathf.Abs(delta) > 0.0001f)
        {
            OnLifeChangue?.Invoke(delta);
            RaiseFullUpdate();
        }
    }

    public void TakeDamague(float amount)
    {
        if (amount <= 0f) return;
        float before = health;
        health = Mathf.Max(health - amount, 0f);
    float delta = health - before;
        OnLifeChangue?.Invoke(delta);
        RaiseFullUpdate();
        if (health <= 0f)
        {
            OnDead?.Invoke();
            if(gameObject.tag == "Enemy")
            {
                SetUpTutorial.enemyDefeatCount++;
                UnlockDoors.instance.EnemiesDefated();
                ManagerEnemiesInTutorial.instance.CallSpawn();
                Debug.Log($"enemigos muertos {SetUpTutorial.enemyDefeatCount}");
            }
            
        }
    }

    private void RaiseFullUpdate()
    {
        OnHealthUpdated?.Invoke(health, maxHealth);
    }
}
