using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private HealthController playerHealth;
    [SerializeField] private Slider slider;
    [SerializeField] private Image lifeBar;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private bool hideWhenFull = false;

    private void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (lifeBar == null) lifeBar = GetComponent<Image>();
        if (playerHealth==null)
        {
            playerHealth = GameObject.FindWithTag("Player").GetComponent<HealthController>();
        }
    }

    void Update()
    {
        if(playerHealth==null)
        {
            playerHealth = GameObject.FindWithTag("Player").GetComponent<HealthController>();
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthUpdated += OnHealthUpdated;
            // force init
            OnHealthUpdated(playerHealth.health, playerHealth.maxHealth);
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthUpdated -= OnHealthUpdated;
        }
    }

    public void Bind(HealthController controller)
    {
        if (playerHealth != null)
            playerHealth.OnHealthUpdated -= OnHealthUpdated;
        playerHealth = controller;
        if (playerHealth != null)
        {
            playerHealth.OnHealthUpdated += OnHealthUpdated;
            OnHealthUpdated(playerHealth.health, playerHealth.maxHealth);
        }
    }

    private void OnHealthUpdated(float current, float max)
    {
        /*if (slider != null)
        {
            slider.minValue = 0f;
            slider.maxValue = max;
            slider.value = current;
        }
        if (hideWhenFull && canvasGroup != null)
        {
            canvasGroup.alpha = current >= max ? 0f : 1f;
        }*/
        if (lifeBar != null)
        {
            lifeBar.fillAmount = playerHealth.health / max;
        }
    }
}
