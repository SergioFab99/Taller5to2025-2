using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackOrder : MonoBehaviour
{
    public static EnemyAttackOrder Instance { get; private set; }

    [Header("Formation")]
    public float attackZoneRadius = 3.5f;    
    public float innerRingRadius = 6f;
    public float middleRingRadius = 9f;
    public float outerRingRadius = 12f;

    [Header("Aggro")]
    public float maxAggroRadius = 12f;
    public float frontArcDegrees = 130f;

    [Header("Thresholds")]
    public int twoRingThreshold = 5;
    public int threeRingThreshold = 10;

    [Header("Attack")]
    public int maxAttackers = 2;
    public float attackerDelayMin = 0.25f;
    public float attackerDelayMax = 0.6f;

    [Header("Standby")]
    public float personalSpinMin = 0.1f;
    public float personalSpinMax = 0.3f;
    public float globalSpinSpeed = 0.8f;    
    public float lateralShuffleLimit = 0.35f; 

    public Transform player;

    private readonly List<EnemyStateHandler> allEnemies = new();
    private readonly List<EnemyStateHandler> lockedAttackers = new();
    public IEnumerable<EnemyStateHandler> LockedAttackers => lockedAttackers;
    private readonly Dictionary<EnemyStateHandler, float> pendingPromotions = new();

    public struct FormationData
    {
        public int ringIndex;
        public float dist;
        public bool inFrontArc;
        public Vector3 targetPos;
    }
    public readonly Dictionary<EnemyStateHandler, FormationData> formation = new();

    private readonly Dictionary<EnemyStateHandler, float> baseAngle = new();

    private readonly Queue<EnemyStateHandler> recentAttackers = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        if (!player)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p) player = p.transform;
        }
    }

    private void Update()
    {
        Cleanup();

        if (!player) return;

        BuildFormation();

        PromoteIfPossible();
        ProcessPendingPromotions();
    }

    public void RegisterEnemy(EnemyStateHandler e)
    {
        if (!e) return;
        if (!allEnemies.Contains(e))
        {
            allEnemies.Add(e);
            baseAngle[e] = Random.Range(0f, 360f);
        }
    }

    public void UnregisterEnemy(EnemyStateHandler e)
    {
        allEnemies.Remove(e);
        lockedAttackers.Remove(e);
        pendingPromotions.Remove(e);
        formation.Remove(e);
        baseAngle.Remove(e);

        TryPromoteReplacement();
    }
    private void BuildFormation()
    {
        formation.Clear();

        if (allEnemies.Count == 0) return;

        List<(EnemyStateHandler e, float dist)> actives = new();

        foreach (var e in allEnemies)
        {
            if (!e || e.Target == null) continue;

            float dist = Vector3.Distance(e.character.transform.position, player.position);
            if (dist > maxAggroRadius) continue;

            actives.Add((e, dist));
        }

        if (actives.Count == 0) return;

        actives.Sort((a, b) => a.dist.CompareTo(b.dist));

        int ringCount = 1;
        if (actives.Count > twoRingThreshold) ringCount = 2;
        if (actives.Count > threeRingThreshold) ringCount = 3;

        float[] radii = { innerRingRadius, middleRingRadius, outerRingRadius };
        int perRing = Mathf.CeilToInt(actives.Count / (float)ringCount);

        float globalSpin = globalSpinSpeed * Time.time;

        for (int i = 0; i < actives.Count; i++)
        {
            var enemy = actives[i].e;
            float distToPlayer = actives[i].dist;

            if (!enemy) continue;

            int ringIndex = Mathf.Clamp(i / perRing, 0, ringCount - 1);

            float radius = radii[ringIndex];
            if (ringIndex == 0)
                radius = Mathf.Min(radius, attackZoneRadius * 0.9f);

            if (!baseAngle.TryGetValue(enemy, out float baseDeg))
                baseDeg = baseAngle[enemy] = Random.Range(0f, 360f);

            float personalSpeed = Mathf.Lerp(personalSpinMin, personalSpinMax,
                Mathf.Abs(enemy.GetHashCode() % 1000) / 1000f);

            float angle = (baseDeg + globalSpin + personalSpeed * Time.time) * Mathf.Deg2Rad;

            Vector3 dir = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
            Vector3 pos = player.position + (dir * radius);

            float shuffle = Mathf.Sin(Time.time * 0.7f + enemy.GetHashCode()) * lateralShuffleLimit;
            Vector3 tangent = new Vector3(-dir.z, 0, dir.x);
            pos += tangent * shuffle;

            float signed = Vector3.SignedAngle(player.forward, dir, Vector3.up);
            bool inFront = Mathf.Abs(signed) <= (frontArcDegrees * 0.5f);

            formation[enemy] = new FormationData
            {
                ringIndex = ringIndex,
                dist = distToPlayer,
                inFrontArc = inFront,
                targetPos = pos
            };
        }
    }
    public bool IsAttacker(EnemyStateHandler e) => lockedAttackers.Contains(e);

    public void RequestAttack(EnemyStateHandler e)
    {
        if (!e) return;
        if (!allEnemies.Contains(e)) return;
        if (lockedAttackers.Contains(e)) return;
        if (pendingPromotions.ContainsKey(e)) return;

        pendingPromotions[e] = Time.time + Random.Range(attackerDelayMin, attackerDelayMax);
    }

    public void NotifyAttackFinished(EnemyStateHandler e)
    {
        if (!e) return;

        lockedAttackers.Remove(e);
        pendingPromotions.Remove(e);

        if (!recentAttackers.Contains(e))
        {
            recentAttackers.Enqueue(e);

            while (recentAttackers.Count > maxAttackers)
                recentAttackers.Dequeue();
        }

        TryPromoteReplacement();
    }

    public bool TryGetFormationDestination(EnemyStateHandler e, out Vector3 dest)
    {
        if (formation.TryGetValue(e, out var f))
        {
            Vector3 point = f.targetPos;

            if (NavMesh.SamplePosition(point, out NavMeshHit hit, 1.5f, NavMesh.AllAreas))
            {
                dest = hit.position;
                return true;
            }

            dest = point;
            return true;
        }

        dest = default;
        return false;
    }

    private void PromoteIfPossible()
    {
        if (lockedAttackers.Count + pendingPromotions.Count >= maxAttackers)
            return;

        if (formation.Count == 0)
            return;

        var candidates = formation.Where(kv => kv.Value.ringIndex == 0 && kv.Value.inFrontArc && kv.Key != null && !lockedAttackers.Contains(kv.Key) && kv.Value.dist <= attackZoneRadius * 1.35f).Select(kv => kv.Key).ToList();

        if (candidates.Count == 0)
            return;

        var fresh = candidates.Where(c => !recentAttackers.Contains(c)).OrderBy(c => Vector3.Distance(c.transform.position, player.position)).ToList();

        EnemyStateHandler chosen = fresh.Count > 0 ? fresh[0] : candidates.OrderBy(c => Vector3.Distance(c.transform.position, player.position)).First();

        pendingPromotions[chosen] = Time.time + Random.Range(attackerDelayMin, attackerDelayMax);
    }


    private void ProcessPendingPromotions()
    {
        float now = Time.time;

        var ready = pendingPromotions.Where(kv => kv.Value <= now).Select(kv => kv.Key).ToList();

        foreach (var e in ready)
        {
            pendingPromotions.Remove(e);
            if (!e) continue;
            if (lockedAttackers.Count >= maxAttackers) break;

            bool recentlyAttacked = recentAttackers.Contains(e);

            bool someoneElseAvailable =
                formation.Keys.Any(x => x != e && !recentAttackers.Contains(x) && !lockedAttackers.Contains(x) && formation[x].ringIndex == 0 && formation[x].inFrontArc);

            if (recentlyAttacked && someoneElseAvailable)
                continue;

            lockedAttackers.Add(e);
        }
    }

    private void Cleanup()
    {
        allEnemies.RemoveAll(e => e == null);
        lockedAttackers.RemoveAll(e => e == null);

        var deadKeys = pendingPromotions.Where(kv => kv.Key == null).Select(kv => kv.Key).ToList();

        foreach (var k in deadKeys)
        {
            pendingPromotions.Remove(k);
        }

        var fDead = formation.Where(kv => kv.Key == null).Select(kv => kv.Key).ToList();

        foreach (var k in fDead)
        {
            formation.Remove(k);
        }

    }

    private void TryPromoteReplacement()
    {
        if (lockedAttackers.Count >= maxAttackers)
            return;

        if (formation.Count == 0)
            return;

        var candidates = formation.Where(kv =>kv.Value.ringIndex == 0 && kv.Value.inFrontArc && kv.Key != null && !lockedAttackers.Contains(kv.Key)).Select(kv => kv.Key).ToList();

        if (candidates.Count == 0)
            return;

        var fresh = candidates.Where(c => !recentAttackers.Contains(c)).OrderBy(c => Vector3.Distance(c.transform.position, player.position)).ToList();

        EnemyStateHandler chosen = fresh.Count > 0 ? fresh[0] : candidates.OrderBy(c => Vector3.Distance(c.transform.position, player.position)).First();

        lockedAttackers.Add(chosen);
    }
}