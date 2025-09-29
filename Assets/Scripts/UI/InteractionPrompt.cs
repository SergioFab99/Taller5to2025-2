using UnityEngine;
using TMPro;

// Simple singleton overlay prompt manager for interaction messages.
// Create a Canvas (Screen Space - Overlay) with a TextMeshProUGUI child and assign it.
public class InteractionPrompt : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptLabel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeSpeed = 10f;
    [SerializeField] private bool startHidden = true;

    private static InteractionPrompt _instance;
    private static string _pendingText;
    private static bool _showRequested;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (startHidden && canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    private void Update()
    {
        if (canvasGroup == null || promptLabel == null) return;
        if (_showRequested)
        {
            if (promptLabel.text != _pendingText)
            {
                promptLabel.text = _pendingText;
            }
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, fadeSpeed * Time.deltaTime);
        }
        else
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, fadeSpeed * Time.deltaTime);
        }
    }

    public static void Show(string text)
    {
        if (_instance == null) return;
        _pendingText = text;
        _showRequested = true;
    }

    public static void Hide(object requester)
    {
        if (_instance == null) return;
        _showRequested = false;
    }
}
