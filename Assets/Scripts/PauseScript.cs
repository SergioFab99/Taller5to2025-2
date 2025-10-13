using UnityEngine;
using System.Collections.Generic;

public class PauseScript : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private GameObject pauseCanvas;

    [Header("Input")]
    [SerializeField]
    private KeyCode toggleKey = KeyCode.Escape;

    [Tooltip("Scripts that should remain enabled while the game is paused.")]
    [SerializeField]
    private MonoBehaviour[] behaviourWhitelist;

    [SerializeField]
    private bool pauseOnStart;

    private readonly Dictionary<MonoBehaviour, bool> cachedStates = new Dictionary<MonoBehaviour, bool>();
    private readonly List<MonoBehaviour> behavioursToDisable = new List<MonoBehaviour>();

    private bool isPaused;
    private bool initialCursorVisible;
    private CursorLockMode initialCursorLockMode;
    private float initialTimeScale;

    public bool IsPaused => isPaused;

    private void Awake()
    {
        initialCursorVisible = Cursor.visible;
        initialCursorLockMode = Cursor.lockState;
        initialTimeScale = Time.timeScale;

        if (pauseCanvas == null)
        {
            Debug.LogWarning($"{nameof(PauseScript)} on {gameObject.name} is missing a pause canvas reference.");
        }

        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(false);
        }
    }

    private void Start()
    {
        if (pauseOnStart)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        if (isPaused)
        {
            return;
        }

        CacheBehavioursToDisable();

        foreach (var behaviour in behavioursToDisable)
        {
            if (behaviour == null)
            {
                continue;
            }

            cachedStates[behaviour] = behaviour.enabled;

            if (behaviour.enabled)
            {
                behaviour.enabled = false;
            }
        }

        Time.timeScale = 0f;

        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isPaused = true;
    }

    public void Resume()
    {
        if (!isPaused && cachedStates.Count == 0)
        {
            
            Time.timeScale = initialTimeScale;

            if (pauseCanvas != null)
            {
                pauseCanvas.SetActive(false);
            }

            Cursor.lockState = initialCursorLockMode;
            Cursor.visible = initialCursorVisible;
            return;
        }

        foreach (var kvp in cachedStates)
        {
            if (kvp.Key == null)
            {
                continue;
            }

            kvp.Key.enabled = kvp.Value;
        }

        cachedStates.Clear();
        behavioursToDisable.Clear();

        Time.timeScale = initialTimeScale;

        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(false);
        }

        Cursor.lockState = initialCursorLockMode;
        Cursor.visible = initialCursorVisible;

        isPaused = false;
    }

    private void CacheBehavioursToDisable()
    {
        behavioursToDisable.Clear();

    var allBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        foreach (var behaviour in allBehaviours)
        {
            if (behaviour == null)
            {
                continue;
            }

            if (behaviour == this || behaviour is PauseScript)
            {
                continue;
            }

            if (pauseCanvas != null && behaviour.transform.IsChildOf(pauseCanvas.transform))
            {
                continue;
            }

            if (behaviourWhitelist != null && System.Array.IndexOf(behaviourWhitelist, behaviour) >= 0)
            {
                continue;
            }

            if (!behaviour.enabled || !behaviour.gameObject.activeInHierarchy)
            {
                continue;
            }

            behavioursToDisable.Add(behaviour);
        }
    }
}
