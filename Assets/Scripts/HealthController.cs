using UnityEngine;
using System;

public class HealthController : MonoBehaviour
{
    [Header("Health Values")]
    public float maxHealth = 100f;
    public float health = 100f;

    public event LifeChangued OnLifeChangue;
    public delegate void LifeChangued(float changeAmount);
    public event Live OnDead;
    public delegate void Live();
    public event Action<float, float> OnHealthUpdated;

    [Header("Hit Audio")]
    [SerializeField] AudioSource hitAudioSource;
    [SerializeField] AudioClip hitClip;

    void Awake()
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
        float delta = health - before;   // será < 0 si perdió vida

        // Cada vez que se reduce la vida, reproducir sonido
        if (delta < 0f)
        {
            PlayHitSound();
        }

        OnLifeChangue?.Invoke(delta);
        RaiseFullUpdate();

        if (health <= 0f)
        {
            OnDead?.Invoke();

            if (gameObject.tag == "Enemy")
            {
                if (DisplayInteractHUD.thisIsTutorial)
                {
                    SetUpTutorial.enemyDefeatCount++;
                    UnlockDoors.instance.EnemiesDefated();
                    Debug.Log($"enemigos muertos {SetUpTutorial.enemyDefeatCount}");
                }

                if (DisplayInteractHUD.thisIsLevel1)
                {
                    SpawnEnemiesLvl1.enemiesCapacity--;
                }
            }
        }
    }

    void PlayHitSound()
    {
        if (hitAudioSource == null || hitClip == null)
            return;

        Debug.Log("Está sonando");
        hitAudioSource.PlayOneShot(hitClip);
    }

    void RaiseFullUpdate()
    {
        OnHealthUpdated?.Invoke(health, maxHealth);
    }
}
