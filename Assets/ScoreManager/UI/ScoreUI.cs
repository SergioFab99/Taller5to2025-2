using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ScoreUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private Image rankBackground;
    [SerializeField] private CanvasGroup canvasGroup;
    
    [Header("Rank Colors")]
    [SerializeField] private Color sRankColor = new Color(1f, 0.84f, 0f);
    [SerializeField] private Color aRankColor = new Color(0.8f, 0.8f, 0.8f);
    [SerializeField] private Color bRankColor = new Color(0.8f, 0.5f, 0.2f);
    [SerializeField] private Color cRankColor = new Color(0.4f, 0.6f, 0.8f);
    [SerializeField] private Color dRankColor = new Color(0.5f, 0.5f, 0.5f);
    
    [Header("Animation Settings")]
    [SerializeField] private float scoreUpdateSpeed = 50f;
    [SerializeField] private float rankPopDuration = 0.3f;
    [SerializeField] private float rankPopScale = 1.3f;
    
    private int displayedScore = 0;
    private int targetScore = 0;
    private ScoreRank currentRank = ScoreRank.D;
    
    private void Start()
    {
        if (ScoreManager.Instance == null)
        {
            Debug.LogError("ScoreManager not found! Make sure it exists in the scene.");
            return;
        }
        
        // Suscribirse a eventos
        ScoreManager.Instance.OnScoreChanged += OnScoreChanged;
        ScoreManager.Instance.OnRankChanged += OnRankChanged;
        
        // Inicializar UI
        UpdateScoreDisplay(0);
        UpdateRankDisplay(ScoreRank.D);
    }
    
    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= OnScoreChanged;
            ScoreManager.Instance.OnRankChanged -= OnRankChanged;
        }
    }
    
    private void Update()
    {
        // Animar el contador de score suavemente
        if (displayedScore != targetScore)
        {
            float step = scoreUpdateSpeed * Time.deltaTime;
            displayedScore = (int)Mathf.MoveTowards(displayedScore, targetScore, step);
            scoreText.text = displayedScore.ToString("N0");
        }
    }
    
    private void OnScoreChanged(int newScore)
    {
        targetScore = newScore;
    }
    
    private void OnRankChanged(ScoreRank newRank)
    {
        if (newRank != currentRank)
        {
            currentRank = newRank;
            UpdateRankDisplay(newRank);
            StartCoroutine(RankUpAnimation());
        }
    }
    
    private void UpdateScoreDisplay(int score)
    {
        displayedScore = score;
        targetScore = score;
        scoreText.text = score.ToString("N0");
    }
    
    private void UpdateRankDisplay(ScoreRank rank)
    {
        rankText.text = rank.ToString();
        
        Color rankColor = rank switch
        {
            ScoreRank.S => sRankColor,
            ScoreRank.A => aRankColor,
            ScoreRank.B => bRankColor,
            ScoreRank.C => cRankColor,
            ScoreRank.D => dRankColor,
            _ => Color.white
        };
        
        rankText.color = rankColor;
        if (rankBackground != null)
        {
            rankBackground.color = new Color(rankColor.r, rankColor.g, rankColor.b, 0.3f);
        }
    }
    
    private IEnumerator RankUpAnimation()
    {
        if (rankText == null) yield break;
        
        Vector3 originalScale = rankText.transform.localScale;
        Vector3 targetScale = originalScale * rankPopScale;
        
        // Expandir
        float elapsed = 0f;
        while (elapsed < rankPopDuration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (rankPopDuration / 2);
            rankText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        
        // Contraer
        elapsed = 0f;
        while (elapsed < rankPopDuration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (rankPopDuration / 2);
            rankText.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
        
        rankText.transform.localScale = originalScale;
    }
    
    // Método público para mostrar/ocultar el UI
    public void SetVisible(bool visible, float duration = 0.3f)
    {
        if (canvasGroup != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeCanvasGroup(visible ? 1f : 0f, duration));
        }
    }
    
    private IEnumerator FadeCanvasGroup(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }
        
        canvasGroup.alpha = targetAlpha;
    }
}