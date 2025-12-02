using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAttackOrder : MonoBehaviour
{
    public static EnemyAttackOrder Instance { get; private set; }

    public float attackZoneRadius = 3.5f;
    public float maxAggroRadius = 12f;

    public float innerRingRadius = 6f;
    public float middleRingRadius = 9f;
    public float outerRingRadius = 12f;

    public int twoRingThreshold = 6;
    public int threeRingThreshold = 12;

    public int maxAttackers = 2;
    public float frontArcDegrees = 140f;

    public float globalSpinSpeed = 1.2f;  
    public float personalSpinMin = 0.1f;   
    public float personalSpinMax = 0.35f; 

    public Transform player;

    private readonly List<EnemyStateHandler> allEnemies = new();
    private readonly List<EnemyStateHandler> lockedAttackers = new();
    private readonly Dictionary<EnemyStateHandler, float> pendingPromotions = new();

    private readonly Dictionary<EnemyStateHandler, float> baseAngle = new();

    private struct FormationData
    {
        public int ringIndex;
        public float distToPlayer;
        public float angleDeg;
        public bool isInFrontArc;
        public Vector3 desiredPosition;
    }

    private readonly Dictionary<EnemyStateHandler, FormationData> formation = new();

    public IReadOnlyList<EnemyStateHandler> LockedAttackers => lockedAttackers;

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
            if (p != null)
                player = p.transform;
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        BuildFormation();
        PromoteIfPossible();
        ProcessPendingPromotions();
    }

    public void RegisterEnemy(EnemyStateHandler e)
    {
        if (e != null && !allEnemies.Contains(e))
        {
            allEnemies.Add(e);

            baseAngle[e] = Random.Range(0f, 360f);

            e.lastAttackTime = -999f;
        }
    }

    public void UnregisterEnemy(EnemyStateHandler e)
    {
        allEnemies.Remove(e);
        lockedAttackers.Remove(e);
        pendingPromotions.Remove(e);
        formation.Remove(e);
        baseAngle.Remove(e);
    }

    private void BuildFormation()
    {
        formation.Clear();

        List<(EnemyStateHandler enemy, float dist)> active = new();

        foreach (var e in allEnemies)
        {
            if (e == null || e.Target == null)
                continue;

            Vector3 toEnemy = e.character.transform.position - player.position;
            float dist = toEnemy.magnitude;

            if (dist > maxAggroRadius || dist < 0.001f)
                continue;

            active.Add((e, dist));
        }

        if (active.Count == 0)
            return;

        active.Sort((a, b) => a.dist.CompareTo(b.dist));

        int usedRings = 1;
        if (active.Count > twoRingThreshold) usedRings = 2;
        if (active.Count > threeRingThreshold) usedRings = 3;

        int perRing = Mathf.CeilToInt(active.Count / (float)usedRings);
        float[] radii = { innerRingRadius, middleRingRadius, outerRingRadius };

        float globalSpinRad = globalSpinSpeed * Mathf.Deg2Rad * Time.time;

        for (int i = 0; i < active.Count; i++)
        {
            var enemy = active[i].enemy;
            float dist = active[i].dist;

            if (enemy == null)
                continue;

            int ringIndex = Mathf.Clamp(i / perRing, 0, usedRings - 1);
            float targetRadius = radii[ringIndex];

            if (ringIndex == 0)
                targetRadius = Mathf.Min(targetRadius, attackZoneRadius * 0.95f);

            if (!baseAngle.TryGetValue(enemy, out float baseDeg))
            {
                baseDeg = Random.Range(0f, 360f);
                baseAngle[enemy] = baseDeg;
            }

            float baseRad = baseDeg * Mathf.Deg2Rad;

            float hash = Mathf.Abs(enemy.GetHashCode() % 1000) / 1000f;
            float personalSpeed = Mathf.Lerp(personalSpinMin, personalSpinMax, hash);
            float personalRad = personalSpeed * Mathf.Deg2Rad * Time.time;

            float finalRad = baseRad + globalSpinRad + personalRad;

            Vector3 dir = new Vector3(Mathf.Sin(finalRad), 0f, Mathf.Cos(finalRad));

            Vector3 desiredPos = player.position + dir * targetRadius;

            float jitter = 0.25f;
            float shuffleAngle = Time.time * 0.8f + enemy.GetHashCode();
            float shuffle = Mathf.Sin(shuffleAngle) * jitter;

            Vector3 tangent = new Vector3(-dir.z, 0f, dir.x); 
            desiredPos += tangent * shuffle;

            float signedToEnemy = Vector3.SignedAngle(player.forward, dir, Vector3.up);
            bool inFrontArc = Mathf.Abs(signedToEnemy) <= (frontArcDegrees * 0.5f);

            formation[enemy] = new FormationData
            {
                ringIndex = ringIndex,
                distToPlayer = dist,
                angleDeg = baseDeg,
                isInFrontArc = inFrontArc,
                desiredPosition = desiredPos
            };
        }
    }

    private void PromoteIfPossible()
    {
        if (lockedAttackers.Count + pendingPromotions.Count >= maxAttackers)
            return;

        if (formation.Count == 0)
            return;

        var candidates = formation
            .Where(kv =>
            {
                var e = kv.Key;
                var data = kv.Value;

                if (e == null || e.Target == null) return false;
                if (data.ringIndex != 0) return false;             
                if (!data.isInFrontArc) return false;          
                if (lockedAttackers.Contains(e)) return false;
                if (pendingPromotions.ContainsKey(e)) return false;
                if (Time.time < e.attackTagCooldownEndTime) return false;
                if (data.distToPlayer > attackZoneRadius * 1.4f) return false;

                return true;
            })
            .OrderBy(kv => kv.Key.lastAttackTime)   
            .ThenBy(kv => kv.Value.distToPlayer)       
            .Select(kv => kv.Key)
            .ToList();

        if (candidates.Count == 0)
            return;

        var chosen = candidates[0];
        float delay = Random.Range(0.35f, 0.7f);
        pendingPromotions[chosen] = Time.time + delay;
    }

    private void ProcessPendingPromotions()
    {
        if (pendingPromotions.Count == 0)
            return;

        float now = Time.time;

        var ready = pendingPromotions
            .Where(kv => kv.Value <= now)
            .Select(kv => kv.Key)
            .ToList();

        foreach (var e in ready)
        {
            pendingPromotions.Remove(e);

            if (e == null || !allEnemies.Contains(e) || e.Target == null)
                continue;

            if (lockedAttackers.Count >= maxAttackers)
                break;

            lockedAttackers.Add(e);
            e.SetState(e.GetAttackState());
        }
    }

    public void NotifyAttackFinished(EnemyStateHandler e)
    {
        if (e == null)
            return;

        lockedAttackers.Remove(e);
        pendingPromotions.Remove(e);

        float cooldown = Random.Range(3.5f, 5.0f);
        e.attackTagCooldownEndTime = Time.time + cooldown;

        e.lastAttackTime = Time.time;
    }

    public bool IsAttacker(EnemyStateHandler e) => lockedAttackers.Contains(e);

    public bool TryGetFormationDestination(EnemyStateHandler enemy, out Vector3 destination)
    {
        if (enemy != null && formation.TryGetValue(enemy, out var data))
        {
            destination = data.desiredPosition;
            return true;
        }

        destination = default;
        return false;
    }

    public bool IsInFrontArc(EnemyStateHandler enemy)
    {
        return enemy != null &&
               formation.TryGetValue(enemy, out var data) &&
               data.isInFrontArc;
    }

    public bool IsInRearRing(EnemyStateHandler enemy)
    {
        return enemy != null &&
               formation.TryGetValue(enemy, out var data) &&
               data.ringIndex > 0;
    }
}