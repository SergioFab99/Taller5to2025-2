using UnityEngine;

public class ScorePopupManager : MonoBehaviour
{
    public static ScorePopupManager Instance { get; private set; }
    
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private Transform popupContainer;
    [SerializeField] private Vector2 spawnOffset = new Vector2(0, 50);
    
    [Header("Colors")]
    [SerializeField] private Color positiveColor = Color.green;
    [SerializeField] private Color negativeColor = Color.red;
    [SerializeField] private Color specialColor = Color.yellow;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreAdded += ShowPopup;
        }
    }
    
    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreAdded -= ShowPopup;
        }
    }
    
    private void ShowPopup(int score, string reason)
    {
        if (popupPrefab == null || popupContainer == null) return;
        
        GameObject popup = Instantiate(popupPrefab, popupContainer);
        RectTransform rect = popup.GetComponent<RectTransform>();
        rect.anchoredPosition = spawnOffset;
        
        // Determinar color
        Color color = positiveColor;
        if (score < 0)
            color = negativeColor;
        else if (score >= 100) // Conexiones totales, counterattacks, etc.
            color = specialColor;
        
        ScorePopup popupScript = popup.GetComponent<ScorePopup>();
        if (popupScript != null)
        {
            popupScript.Initialize(score, reason, color);
        }
    }
}