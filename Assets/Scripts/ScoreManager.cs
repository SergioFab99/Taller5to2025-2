using System;
using TMPro;
using UnityEngine;

public class ScoreManager: MonoBehaviour
{
    [SerializeField]
    private int pointsPerEnemy = 500;

    [SerializeField]
    private string enemyTag = "enemy";

    [SerializeField]
    private Canvas scoreCanvas;

    [SerializeField]
    private TMP_Text scoreText;

    [SerializeField]
    private int enemiesToDefeat = 11;

    [SerializeField]
    private GameObject rankPanel;

    [SerializeField]
    private TMP_Text rankPanelTitle;

    [SerializeField]
    private TMP_Text rankPanelDetails;

    private const float EnemyScanIntervalSeconds = 1f;

    private int totalScore;
    private int defeatedEnemies;
    private bool hasWarnedForMissingScoreText;
    private bool hasWarnedForMissingEnemyTag;
    private bool hasShownRankPanel;

    private void Start()
    {
        UpdateScoreDisplay();
        HideRankPanel();
    }

    private void OnEnable()
    {
        ScanForEnemiesAndAttachReporter();

        if (EnemyScanIntervalSeconds > 0f)
        {
            InvokeRepeating(nameof(ScanForEnemiesAndAttachReporter), EnemyScanIntervalSeconds, EnemyScanIntervalSeconds);
        }
    }

    private void OnDisable()
    {
        if (EnemyScanIntervalSeconds > 0f)
        {
            CancelInvoke(nameof(ScanForEnemiesAndAttachReporter));
        }
    }

    public void AddScore()
    {
        totalScore += pointsPerEnemy;
        Debug.Log($"ScoreManager: Enemy defeated, total score = {totalScore}");
        UpdateScoreDisplay();
    }

    public string GetRank()
    {
        var rank = CalculateRank(totalScore);
        Debug.Log($"ScoreManager: Final rank = {rank} with total score = {totalScore}");
        return rank;
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
        if (!IsEnemy(candidate))
        {
            return;
        }

        EnsureReporter(candidate);
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText == null && scoreCanvas != null)
        {
            scoreText = scoreCanvas.GetComponentInChildren<TMP_Text>();
        }

        if (scoreText != null)
        {
            scoreText.text = $"Score: {totalScore}";
            return;
        }

        if (!hasWarnedForMissingScoreText)
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
            {
                EnsureReporter(enemy);
            }
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
        if (enemy == null)
        {
            return;
        }

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
        if (candidate == null)
        {
            return false;
        }

        if (candidate.CompareTag(enemyTag))
        {
            return true;
        }

        return string.Equals(candidate.tag, enemyTag, StringComparison.OrdinalIgnoreCase);
    }

    internal void HandleEnemyDestroyed(GameObject enemy)
    {
        if (!IsEnemy(enemy))
        {
            return;
        }

        AddScore();
        defeatedEnemies++;

        if (defeatedEnemies >= enemiesToDefeat)
        {
            ShowRankPanel();
        }
    }

    private static string CalculateRank(int score)
    {
        if (score >= 5000)
        {
            return "S";
        }

        if (score >= 3500)
        {
            return "A";
        }

        if (score >= 2500)
        {
            return "B";
        }

        if (score >= 1000)
        {
            return "C";
        }

        return "D";
    }

    private void ShowRankPanel()
    {
        if (hasShownRankPanel)
        {
            return;
        }

        hasShownRankPanel = true;

        if (EnemyScanIntervalSeconds > 0f)
        {
            CancelInvoke(nameof(ScanForEnemiesAndAttachReporter));
        }

        if (rankPanel != null)
        {
            rankPanel.SetActive(true);
        }

        var rank = GetRank();

        if (rankPanelTitle != null)
        {
            rankPanelTitle.text = $"Rank: {rank}";
        }

        if (rankPanelDetails != null)
        {
            rankPanelDetails.text = "S = 5000+\nA = 3500+\nB = 2500+\nC = 1000+\nD = 0+\nTotal Score = " + totalScore;
        }
    }

    private void HideRankPanel()
    {
        if (rankPanel != null)
        {
            rankPanel.SetActive(false);
        }
    }
}

public sealed class EnemyScoreReporter: MonoBehaviour
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
        if (hasReported)
        {
            return;
        }

        if (scoreManager == null || !scoreManager.isActiveAndEnabled)
        {
            return;
        }

        scoreManager.HandleEnemyDestroyed(gameObject);
        hasReported = true;
    }
}
