using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] public TMP_Text scoreText;
    [SerializeField] public TMP_Text scoreTextSecondCanvas;
    [SerializeField] public Canvas scoreCanvas;
    [SerializeField] private int pointsPerEnemy = 500;
    [SerializeField] private string enemyTag = "enemy";
    [SerializeField] private int enemiesToDefeat = 11;
    [SerializeField] public GameObject rankPanel;
    [SerializeField] public TMP_Text rankPanelTitle;
    [SerializeField] public TMP_Text rankPanelDetails;

    private const float EnemyScanIntervalSeconds = 1f;

    private int totalScore;
    private int defeatedEnemies;
    private bool hasWarnedForMissingScoreText;
    private bool hasWarnedForMissingEnemyTag;
    private bool hasShownRankPanel;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateScoreDisplay();
        HideRankPanel();
    }

    private void OnEnable()
    {
        ScanForEnemiesAndAttachReporter();
        if (EnemyScanIntervalSeconds > 0f)
            InvokeRepeating(nameof(ScanForEnemiesAndAttachReporter), EnemyScanIntervalSeconds, EnemyScanIntervalSeconds);
    }

    private void OnDisable()
    {
        if (EnemyScanIntervalSeconds > 0f)
            CancelInvoke(nameof(ScanForEnemiesAndAttachReporter));
    }

    public void AddScore()
    {
        totalScore += pointsPerEnemy;
        Debug.Log($"ScoreManager: Enemy defeated, total score = {totalScore}");
        UpdateScoreDisplay();
    }

    public int GetTotalScore()
    {
        return totalScore;
    }

    public string GetRank()
    {
        var rank = CalculateRank(totalScore);
        Debug.Log($"ScoreManager: Final rank = {rank} with total score = {totalScore}");
        return rank;
    }

    public int GetIntuitionByRank()
    {
        string rank = GetRank();
        switch(rank)
        {
            case "S": return 50;
            case "A": return 25;
            case "B": return 20;
            case "C": return 15;
            default:  return 10;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TryPrepareEnemy(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryPrepareEnemy(collision.gameObject);
    }

    private void TryPrepareEnemy(GameObject candidate)
    {
        if (!IsEnemy(candidate)) return;
        EnsureReporter(candidate);
    }

    public void UpdateScoreDisplay()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {totalScore}";
        if (scoreTextSecondCanvas != null)
            scoreTextSecondCanvas.text = $"Score: {totalScore}";
        if (!hasWarnedForMissingScoreText && scoreText == null && scoreTextSecondCanvas == null)
        {
            Debug.LogWarning("ScoreManager: No TMP_Text assigned to display the score.");
            hasWarnedForMissingScoreText = true;
        }
    }

    private void ScanForEnemiesAndAttachReporter()
    {
        try
        {
            var enemies = GameObject.FindGameObjectsWithTag(enemyTag);
            foreach (var enemy in enemies)
                EnsureReporter(enemy);
        }
        catch (UnityException)
        {
            if (!hasWarnedForMissingEnemyTag)
            {
                Debug.LogWarning($"ScoreManager: Tag '{enemyTag}' is not defined in the project.");
                hasWarnedForMissingEnemyTag = true;
            }
        }
    }

    private void EnsureReporter(GameObject enemy)
    {
        if (enemy == null) return;
        if (enemy.TryGetComponent(out EnemyScoreReporter reporter))
        {
            reporter.Bind(this);
            return;
        }
        reporter = enemy.AddComponent<EnemyScoreReporter>();
        reporter.Bind(this);
    }

    private bool IsEnemy(GameObject candidate)
    {
        if (candidate == null) return false;
        if (candidate.CompareTag(enemyTag)) return true;
        return string.Equals(candidate.tag, enemyTag, StringComparison.OrdinalIgnoreCase);
    }

    internal void HandleEnemyDestroyed(GameObject enemy)
    {
        if (!IsEnemy(enemy)) return;
        AddScore();
        defeatedEnemies++;
        if (defeatedEnemies >= enemiesToDefeat)
            ShowRankPanel();
    }

    private static string CalculateRank(int score)
    {
        if (score >= 5000) return "S";
        if (score >= 3500) return "A";
        if (score >= 2500) return "B";
        if (score >= 1000) return "C";
        return "D";
    }

    public void ShowRankPanel()
    {
        if (hasShownRankPanel) return;
        hasShownRankPanel = true;
        if (EnemyScanIntervalSeconds > 0f)
            CancelInvoke(nameof(ScanForEnemiesAndAttachReporter));
        if (rankPanel != null)
            rankPanel.SetActive(true);
        var rank = GetRank();
        if (rankPanelTitle != null)
            rankPanelTitle.text = $"Rank: {rank}";
        if (rankPanelDetails != null)
            rankPanelDetails.text = "S = 5000+\nA = 3500+\nB = 2500+\nC = 1000+\nD = 0+\nTotal Score = " + totalScore;
    }

    public void HideRankPanel()
    {
        if (rankPanel != null)
            rankPanel.SetActive(false);
    }
}

public sealed class EnemyScoreReporter : MonoBehaviour
{
    private ScoreManager scoreManager;
    private bool hasReported;

    internal void Bind(ScoreManager manager)
    {
        scoreManager = manager;
        hasReported = false;
    }

    private void OnDestroy()
    {
        if (hasReported) return;
        if (scoreManager == null || !scoreManager.isActiveAndEnabled) return;
        scoreManager.HandleEnemyDestroyed(gameObject);
        hasReported = true;
    }
}
