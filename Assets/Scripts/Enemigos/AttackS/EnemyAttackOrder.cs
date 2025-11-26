using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAttackOrder : MonoBehaviour
{
    public static EnemyAttackOrder Instance { get; private set; }

    public float attackZoneRadius = 3.5f;
    public float frontZoneRadius = 7f;
    public float maxAggroRadius = 12f;

    public int maxAttackers = 2;
    public int maxFront = 3;

    public Transform player;

    private readonly List<EnemyStateHandler> allEnemies = new();
    private readonly List<EnemyStateHandler> lockedAttackers = new(); 
    private readonly List<EnemyStateHandler> front = new();
    private readonly List<EnemyStateHandler> rear = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (player == null)
        {
            var p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;
        UpdateSortedGroups();
        PromoteIfPossible();
    }

    public void RegisterEnemy(EnemyStateHandler e)
    {
        if (e == null) return;
        if (!allEnemies.Contains(e))
        {
            allEnemies.Add(e);
            Debug.Log($"EAO: Registered enemy {e.name}");
        }
    }

    public void UnregisterEnemy(EnemyStateHandler e)
    {
        allEnemies.Remove(e);
        lockedAttackers.Remove(e);
        front.Remove(e);
        rear.Remove(e);
    }

    private void UpdateSortedGroups()
    {
        front.Clear();
        rear.Clear();

        var valid = allEnemies.Where(e => e != null && e.Target != null).ToList();

        var sorted = valid
            .Select(e => new
            {
                enemy = e,
                dist = Vector3.Distance(e.character.transform.position, player.position)
            })
            .Where(x => x.dist <= maxAggroRadius).OrderBy(x => x.dist).ToList();

        foreach (var entry in sorted)
        {
            var e = entry.enemy;
            float d = entry.dist;

            if (lockedAttackers.Contains(e))
                continue;

            if (Time.time < e.attackTagCooldownEndTime)
            {
                rear.Add(e);
                continue;
            }

            if (front.Count < maxFront && d <= frontZoneRadius)
                front.Add(e);
            else
                rear.Add(e);
        }

        Debug.Log($"[EAO] Attackers: {string.Join(", ", lockedAttackers.Select(x => x.name))}");
    }

    private void PromoteIfPossible()
    {
        if (lockedAttackers.Count >= maxAttackers)
            return;

        if (front.Count == 0)
            return;

        var next = front[0];

        if (Time.time < next.attackTagCooldownEndTime)
        {
            front.Remove(next);
            rear.Add(next);
            return;
        }

        lockedAttackers.Add(next);
        front.Remove(next);

        Debug.Log($"EAO: PROMOTING {next.name} to attacker");
        next.SetState(next.GetAttackState());
    }

    public void NotifyAttackFinished(EnemyStateHandler e)
    {
        if (!lockedAttackers.Contains(e)) return;

        lockedAttackers.Remove(e);

        if (!rear.Contains(e))
            rear.Add(e);

        Debug.Log($"EAO: {e.name} FINISHED. attackers left: {lockedAttackers.Count}");
    }

    public bool IsAttacker(EnemyStateHandler e) => lockedAttackers.Contains(e);
    public bool IsFront(EnemyStateHandler e) => front.Contains(e);
    public bool IsRear(EnemyStateHandler e) => rear.Contains(e);
}