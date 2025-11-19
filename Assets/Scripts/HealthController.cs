using UnityEngine;
using System;
using System.Collections;

public class HealthController : MonoBehaviour
{
    [Header("Health Values")]
    public float maxHealth = 100f;
    public float health = 100f;

    [Header("Blood Canvas")]
    public GameObject bloodCanvas;
    public float bloodVisibleTime = 0.5f;

    private float bloodTimer = 0f;
    private CanvasGroup canvasGroup;

    public event Action<float, float> OnHealthUpdated;
    public event Action<float> OnPlayerDamaged;
    public event Action OnDead;

    private void Awake()
    {
        health = Mathf.Clamp(health, 0f, maxHealth);
        RaiseFullUpdate();

        DesactivarPanel();
    }

    private void Start()
    {
        DesactivarPanel();
        StartCoroutine(ForceDeactivateOnFirstFrame());
    }

    private IEnumerator ForceDeactivateOnFirstFrame()
    {
        yield return null;
        DesactivarPanel();
    }

    private void DesactivarPanel()
    {
        if (bloodCanvas != null)
        {
            bloodCanvas.SetActive(false);

            canvasGroup = bloodCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            canvasGroup = bloodCanvas.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
        }
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

            if (bloodCanvas != null && canvasGroup != null)
            {
                bloodCanvas.SetActive(true);
                bloodTimer = bloodVisibleTime;
                float opacity = Mathf.Lerp(0.2f, 1f, 1f - (health / maxHealth));
                canvasGroup.alpha = opacity;
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
            {
                bloodCanvas.SetActive(false);
                if (canvasGroup != null) canvasGroup.alpha = 0f;
            }
        }
    }

    private void RaiseFullUpdate()
    {
        OnHealthUpdated?.Invoke(health, maxHealth);
    }
}
