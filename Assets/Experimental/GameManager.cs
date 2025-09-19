using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Life")]
    public int startingLives = 10;
    public Slider lifeSlider; // Arrastra tu Slider aquí

    public int CurrentLives { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        // Opcional: DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        CurrentLives = Mathf.Max(0, startingLives);
        if (lifeSlider)
        {
            lifeSlider.minValue = 0;
            lifeSlider.maxValue = startingLives;
            lifeSlider.value = CurrentLives;
        }
    }

    public void DamagePlayer(int amount)
    {
        if (amount <= 0) return;
        CurrentLives = Mathf.Max(0, CurrentLives - amount);
        if (lifeSlider) lifeSlider.value = CurrentLives;
        if (CurrentLives <= 0)
        {
            OnPlayerDeath();
        }
    }

    public void HealPlayer(int amount)
    {
        if (amount <= 0) return;
        CurrentLives = Mathf.Min(startingLives, CurrentLives + amount);
        if (lifeSlider) lifeSlider.value = CurrentLives;
    }

    void OnPlayerDeath()
    {
        // Aquí puedes deshabilitar el control del jugador, mostrar UI de derrota, etc.
        Debug.Log("Player muerto");
    }
}
