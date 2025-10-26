using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    // Estado actual
    private int currentScore = 0;
    private ScoreRank currentRank = ScoreRank.D;
    
    // ===== VALORES DE PUNTUACIÓN (SEGÚN GDD) =====
    [Header("Combat Score Values")]
    [SerializeField] private int neutralConnectionScore = 15;
    [SerializeField] private int directConnectionScore = 30;
    [SerializeField] private int partialConnectionScore = 10;
    [SerializeField] private int totalConnectionScore = 100;
    [SerializeField] private int counterattackScore = 150;
    [SerializeField] private int dodgedAttackScore = 50;
    [SerializeField] private int reducedDamageScore = 25;
    [SerializeField] private int healthLostPenalty = -70;
    [SerializeField] private int healthHealedScore = 35;
    
    [Header("Enemy Interaction Score Values")]
    [SerializeField] private int enemyDefeatedScore = 200;
    [SerializeField] private int enemyDefeatedDrunkScore = 300;
    [SerializeField] private int enemyBlindedScore = 45;
    [SerializeField] private int enemyStunnedScore = 75;
    [SerializeField] private int enemyKnockedOutScore = 100;
    [SerializeField] private int enemyPushedScore = 30;
    
    [Header("Object Interaction Score Values")]
    [SerializeField] private int weaponPickedUpScore = 20;
    [SerializeField] private int bottleBrokenScore = 25;
    [SerializeField] private int tableFlippedScore = 25;
    [SerializeField] private int chairBrokenScore = 30;
    
    [Header("Rank Thresholds")]
    [SerializeField] private int sRankThreshold = 4000;
    [SerializeField] private int aRankThreshold = 3500;
    [SerializeField] private int bRankThreshold = 2500;
    [SerializeField] private int cRankThreshold = 1000;
    
    [Header("Intuition Rewards")]
    [SerializeField] private int sRankIntuition = 50;
    [SerializeField] private int aRankIntuition = 25;
    [SerializeField] private int bRankIntuition = 20;
    [SerializeField] private int cRankIntuition = 15;
    [SerializeField] private int dRankIntuition = 10;
    
    // Eventos para notificar cambios
    public event Action<int> OnScoreChanged;
    public event Action<ScoreRank> OnRankChanged;
    public event Action<int, string> OnScoreAdded; // Para popups
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // ===== MÉTODO PRINCIPAL =====
    public void AddScore(int points, string reason = "")
    {
        currentScore += points;
        OnScoreChanged?.Invoke(currentScore);
        OnScoreAdded?.Invoke(points, reason);
        
        // Verificar cambio de rango
        ScoreRank newRank = CalculateRank();
        if (newRank != currentRank)
        {
            currentRank = newRank;
            OnRankChanged?.Invoke(currentRank);
        }
        
        #if UNITY_EDITOR
        Debug.Log($"<color=yellow>Score:</color> {points:+0;-#} - {reason} | <color=cyan>Total: {currentScore}</color> | <color=green>Rank: {currentRank}</color>");
        #endif
    }
    
    // ===== GETTERS =====
    public int GetCurrentScore() => currentScore;
    public ScoreRank GetCurrentRank() => currentRank;
    
    public ScoreRank CalculateRank()
    {
        if (currentScore >= sRankThreshold) return ScoreRank.S;
        if (currentScore >= aRankThreshold) return ScoreRank.A;
        if (currentScore >= bRankThreshold) return ScoreRank.B;
        if (currentScore >= cRankThreshold) return ScoreRank.C;
        return ScoreRank.D;
    }
    
    public int GetIntuitionReward()
    {
        return currentRank switch
        {
            ScoreRank.S => sRankIntuition,
            ScoreRank.A => aRankIntuition,
            ScoreRank.B => bRankIntuition,
            ScoreRank.C => cRankIntuition,
            ScoreRank.D => dRankIntuition,
            _ => 0
        };
    }
    
    public void ResetScore()
    {
        currentScore = 0;
        currentRank = ScoreRank.D;
        OnScoreChanged?.Invoke(currentScore);
        OnRankChanged?.Invoke(currentRank);
    }
    
    // ===== MÉTODOS DE PUNTUACIÓN - COMBATE =====
    public void ScoreNeutralConnection() => AddScore(neutralConnectionScore, "Neutral Connection");
    public void ScoreDirectConnection() => AddScore(directConnectionScore, "Direct Connection");
    public void ScorePartialConnection() => AddScore(partialConnectionScore, "Partial Connection");
    public void ScoreTotalConnection() => AddScore(totalConnectionScore, "Total Connection");
    public void ScoreCounterattack() => AddScore(counterattackScore, "Counterattack!");
    public void ScoreDodgedAttack() => AddScore(dodgedAttackScore, "Dodged!");
    public void ScoreReducedDamage() => AddScore(reducedDamageScore, "Damage Reduced");
    public void ScoreHealthLost() => AddScore(healthLostPenalty, "Health Lost");
    public void ScoreHealthHealed() => AddScore(healthHealedScore, "Health Healed");
    
    // ===== MÉTODOS DE PUNTUACIÓN - ENEMIGOS =====
    public void ScoreEnemyDefeated(bool isDrunk = false)
    {
        if (isDrunk)
            AddScore(enemyDefeatedDrunkScore, "Enemy Defeated (Drunk)");
        else
            AddScore(enemyDefeatedScore, "Enemy Defeated");
    }
    
    public void ScoreEnemyBlinded() => AddScore(enemyBlindedScore, "Enemy Blinded");
    public void ScoreEnemyStunned() => AddScore(enemyStunnedScore, "Enemy Stunned");
    public void ScoreEnemyKnockedOut() => AddScore(enemyKnockedOutScore, "Knocked Out!");
    public void ScoreEnemyPushed() => AddScore(enemyPushedScore, "Enemy Pushed");
    
    // ===== MÉTODOS DE PUNTUACIÓN - OBJETOS =====
    public void ScoreWeaponPickedUp() => AddScore(weaponPickedUpScore, "Weapon Picked Up");
    public void ScoreBottleBroken() => AddScore(bottleBrokenScore, "Bottle Broken");
    public void ScoreTableFlipped() => AddScore(tableFlippedScore, "Table Flipped");
    public void ScoreChairBroken() => AddScore(chairBrokenScore, "Chair Broken");
}

// ===== ENUMS =====
public enum ScoreRank
{
    D,
    C,
    B,
    A,
    S
}

public enum ConnectionType
{
    Neutral,   // Disparo normal
    Direct,    // Golpe directo sin defensa
    Partial,   // Bloqueado
    Total      // Golpe fatal (enemigo expuesto/noqueado)
}