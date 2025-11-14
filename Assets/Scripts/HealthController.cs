using UnityEngine;
using System;

public class HealthController : MonoBehaviour
{
    [Header("Health Values")] 
    public float maxHealth = 100f;
    public float health = 100f;

    [Header("Blood Canvas")]
    public GameObject bloodCanvas; 
    public float bloodVisibleTime = 0.5f;   

    private float bloodTimer = 0f;

    public event Action<float, float> OnHealthUpdated;
    public event Action<float> OnPlayerDamaged;
    public event Action OnDead;

    private void Awake()
    {
        health = Mathf.Clamp(health, 0f, maxHealth);
        RaiseFullUpdate();

        if (bloodCanvas != null)
            bloodCanvas.SetActive(false);
    }

    public void AddHealth(float amount)
    {
        if (amount <= 0f) return;
        float before = health;
        health = Mathf.Min(health + amount, maxHealth);
        if (Mathf.Abs(health - before) > 0.0001f)
        {
            RaiseFullUpdate();
        }
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;
        float before = health;
        health = Mathf.Max(health - amount, 0f);
        if (health < before)
        {
            RaiseFullUpdate();
            OnPlayerDamaged?.Invoke(health / maxHealth);

            if (bloodCanvas != null)
            {
                bloodCanvas.SetActive(true);
                bloodTimer = bloodVisibleTime;
            }

            if (health <= 0f)
            {
                OnDead?.Invoke();
            }
        }
    }

    private void Update()
    {
        if (bloodCanvas != null && bloodCanvas.activeSelf)
        {
            bloodTimer -= Time.deltaTime;
            if (bloodTimer <= 0f)
                bloodCanvas.SetActive(false);
        }
    }

    private void RaiseFullUpdate()
    {
        OnHealthUpdated?.Invoke(health, maxHealth);
    }
}
