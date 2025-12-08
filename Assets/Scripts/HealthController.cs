using UnityEngine;
using System;
using System.Collections;

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
    public event Action<float> OnPlayerDamaged;

    [Header("Blood Canvas")]
    [SerializeField] GameObject bloodCanvas;
    [SerializeField] float bloodVisibleTime = 0.5f;
    [SerializeField, Range(0f, 1f)] float minimumBloodAlpha = 0.2f;

    float bloodTimer;
    CanvasGroup bloodCanvasGroup;

    [Header("Hit Audio")]
    [SerializeField] AudioSource hitAudioSource;
    [SerializeField] AudioClip hitClip;

    void Awake()
    {
        health = Mathf.Clamp(health, 0f, maxHealth);
        RaiseFullUpdate();
        InitializeBloodCanvas();
    }

    void Start()
    {
        if (bloodCanvas != null)
        {
            StartCoroutine(ForceDeactivateOnFirstFrame());
        }
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

        
        if (delta < 0f)
        {
            PlayHitSound();
            ShowBloodHitEffect();
            OnPlayerDamaged?.Invoke(GetNormalizedHealth());
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

    void Update()
    {
        if (bloodCanvas == null || !bloodCanvas.activeSelf)
            return;

        bloodTimer -= Time.deltaTime;

        if (bloodTimer <= 0f)
        {
            HideBloodCanvas();
        }
    }

    void RaiseFullUpdate()
    {
        OnHealthUpdated?.Invoke(health, maxHealth);
    }

    void InitializeBloodCanvas()
    {
        if (bloodCanvas == null)
            return;

        EnsureBloodCanvasGroup();
        HideBloodCanvas();
    }

    IEnumerator ForceDeactivateOnFirstFrame()
    {
        yield return null;
        HideBloodCanvas();
    }

    void ShowBloodHitEffect()
    {
        if (bloodCanvas == null)
            return;

        EnsureBloodCanvasGroup();

        bloodCanvas.SetActive(true);
        bloodTimer = bloodVisibleTime;

        if (bloodCanvasGroup != null)
        {
            float normalizedHealth = GetNormalizedHealth();
            float opacity = Mathf.Lerp(minimumBloodAlpha, 1f, 1f - normalizedHealth);
            bloodCanvasGroup.alpha = opacity;
        }
    }

    void HideBloodCanvas()
    {
        if (bloodCanvas == null)
            return;

        if (bloodCanvasGroup != null)
        {
            bloodCanvasGroup.alpha = 0f;
        }

        bloodCanvas.SetActive(false);
        bloodTimer = 0f;
    }

    void EnsureBloodCanvasGroup()
    {
        if (bloodCanvas == null || bloodCanvasGroup != null)
            return;

        bloodCanvasGroup = bloodCanvas.GetComponent<CanvasGroup>();

        if (bloodCanvasGroup == null)
        {
            bloodCanvasGroup = bloodCanvas.AddComponent<CanvasGroup>();
        }
    }

    float GetNormalizedHealth()
    {
        if (maxHealth <= 0f)
            return 0f;

        return Mathf.Clamp01(health / maxHealth);
    }
}
