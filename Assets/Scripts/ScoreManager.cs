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

    private const float EnemyScanIntervalSeconds = 1f;

    private int totalScore;
    private bool hasWarnedForMissingScoreText;
    private bool hasWarnedForMissingEnemyTag;

    private void Start()
    {
        UpdateScoreDisplay();
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
