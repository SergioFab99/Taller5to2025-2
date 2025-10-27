using UnityEngine;
using TMPro;
using System.Collections;

public class ScorePopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI popupText;
    [SerializeField] private CanvasGroup canvasGroup;
    
    [Header("Animation Settings")]
    [SerializeField] private float lifetime = 1.5f;
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
    private RectTransform rectTransform;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }
    
    public void Initialize(int score, string reason, Color color)
    {
        string sign = score >= 0 ? "+" : "";
        popupText.text = $"{sign}{score}\n<size=60%>{reason}</size>";
        popupText.color = color;
        
        StartCoroutine(AnimatePopup());
    }
    
    private IEnumerator AnimatePopup()
    {
        float elapsed = 0f;
        Vector3 startPos = rectTransform.anchoredPosition;
        
        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lifetime;
            
            rectTransform.anchoredPosition = startPos + Vector3.up * (moveSpeed * elapsed);
            
            canvasGroup.alpha = fadeCurve.Evaluate(t);
            
            yield return null;
        }
        
        Destroy(gameObject);
    }
}