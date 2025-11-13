using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyAttackOrder : MonoBehaviour
{
    public static EnemyAttackOrder Instance { get; private set; }

    public float attackZoneRadius = 3.5f;
    public float standbyZoneRadius = 6f;
    public int maxAttackers = 3;
    public int maxStandby = 6;

    private List<EnemyStateHandler> allEnemies = new();
    private List<EnemyStateHandler> activeAttackers = new();
    private List<EnemyStateHandler> standbys = new();

    public Transform player;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (player == null) return;

        UpdateZoneAssignments();
    }

    public void RegisterEnemy(EnemyStateHandler e)
    {
        if (!allEnemies.Contains(e))
            allEnemies.Add(e);
    }

    public void UnregisterEnemy(EnemyStateHandler e)
    {
        allEnemies.Remove(e);
        activeAttackers.Remove(e);
        standbys.Remove(e);
    }

    private void UpdateZoneAssignments()
    {
        foreach (var e in allEnemies.Where(x => x != null && x.Target != null))
        {
            float dist = Vector3.Distance(e.transform.position, player.position);

            if (dist <= attackZoneRadius)
            {
                if (!activeAttackers.Contains(e) && activeAttackers.Count < maxAttackers)
                {
                    PromoteToAttack(e);
                }
            }

            else if (dist <= standbyZoneRadius)
            {
                if (!standbys.Contains(e) && !activeAttackers.Contains(e))
                    standbys.Add(e);
            }

            else
            {
                standbys.Remove(e);
                activeAttackers.Remove(e);
            }
        }

        while (activeAttackers.Count > maxAttackers)
        {
            var removed = activeAttackers[0];
            activeAttackers.RemoveAt(0);
            if (!standbys.Contains(removed))
                standbys.Add(removed);
        }
    }

    private void PromoteToAttack(EnemyStateHandler e)
    {
        if (activeAttackers.Contains(e)) return;
        activeAttackers.Add(e);
        standbys.Remove(e);

        if (e.GetCurrentState() != e.GetAttackState())
        {
            Debug.Log($"Promoting {e.name} to ATTACK zone");
            e.SetState(e.GetAttackState());
        }
    }

    public bool IsAttacking(EnemyStateHandler e) => activeAttackers.Contains(e);
    public bool IsStandby(EnemyStateHandler e) => standbys.Contains(e);

    public void NotifyAttackFinished(EnemyStateHandler e)
    {
        activeAttackers.Remove(e);
        if (!standbys.Contains(e))
            standbys.Add(e);
    }
}