using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class SpawnEnemiesLvl1 : MonoBehaviour
{
    [SerializeField] private float timer, maxTimer;
    [SerializeField] private GameObject player;
    [SerializeField] private SpawnPointsForEnemies[] spawnfounds;
    [SerializeField] private PatrolPointsForEnemies[] patrolFounds;
    [SerializeField] private GameObject[][] enemiesPatron;
    [SerializeField] private GameObject[] patron1, patron2, patron3;
    [SerializeField] private TMP_Text timeTMP;
    [SerializeField] SpawnPointsForEnemies spawnNear1;
    [SerializeField] SpawnPointsForEnemies spawnNear2;
    [SerializeField] SpawnPointsForEnemies spawnNear3;
    public static int enemiesCapacity;
    [SerializeField] int enemiesMaxCapacity;
    private void Awake()
    {
        enemiesCapacity = 0;
        player = GameObject.FindGameObjectWithTag("Player");
        spawnfounds = transform.GetComponentsInChildren<SpawnPointsForEnemies>();
        patrolFounds = transform.GetComponentsInChildren<PatrolPointsForEnemies>();
    }
    void Start()
    {
        timer = maxTimer;
        enemiesPatron = new GameObject[3][];

        enemiesPatron[0] = patron1;
        enemiesPatron[1] = patron2;
        enemiesPatron[2] = patron3;
        //FindSpawnPoint();
    }

    void FixedUpdate()
    {
        if(enemiesCapacity < enemiesMaxCapacity)
        {
            timer -= Time.fixedDeltaTime;
            timeTMP.text = $"Enemies Spawn in {timer}";
            if (timer <= 0)
            {
                FindSpawnPoint();
                timer = maxTimer;
            }
        }        
    }
    public PatrolPointsForEnemies[] GetPatrolPoints()
    {
        return patrolFounds;
    }
    void FindSpawnPoint()
    {
        if(spawnfounds.Length > 0)
        {
            SpawnPointsForEnemies spawnNear1 = spawnfounds[0];
            SpawnPointsForEnemies spawnNear2 = spawnfounds[0];
            SpawnPointsForEnemies spawnNear3 = spawnfounds[0];
            float distanceSpawnNear1 = Vector3.Distance(player.transform.position, spawnNear1.transform.position);
            float distanceSpawnNear2 = Vector3.Distance(player.transform.position, spawnNear2.transform.position);
            float distanceSpawnNear3 = Vector3.Distance(player.transform.position, spawnNear3.transform.position);
            foreach(SpawnPointsForEnemies spawn in spawnfounds)
            {
                float distanceOfSpawn = Vector3.Distance(player.transform.position, spawn.transform.position);
                if (distanceSpawnNear1 > distanceOfSpawn)
                {
                    spawnNear1 = spawn;
                    distanceSpawnNear1 = distanceOfSpawn;
                }
                if (distanceSpawnNear2 > distanceOfSpawn && spawn != spawnNear1)
                {
                    spawnNear2 = spawn;
                    distanceSpawnNear2 = distanceOfSpawn;
                }
                if (distanceSpawnNear3 > distanceOfSpawn && spawn != spawnNear1 && spawn != spawnNear2)
                {
                    spawnNear3 = spawn;
                    distanceSpawnNear3 = distanceOfSpawn;
                }
            }
            this.spawnNear1 = spawnNear1;
            this.spawnNear2 = spawnNear2;
            this.spawnNear3 = spawnNear3;
            Spawn(spawnNear2.transform, spawnNear3.transform);
        }        
    }
    void Spawn(Transform spawn1, Transform spawn2)
    {
        for(int i = 0; i < 2; i++)
        {
            int patron = Random.Range(0, enemiesPatron.Length);
            int patrol = Random.Range(0, patrolFounds.Length);
            if(i == 0)
            {
                GameObject ag1 =  Instantiate(enemiesPatron[patron][0], spawn1.position, spawn1.rotation);
                EnemyStateHandler nav1 = ag1.GetComponent<EnemyStateHandler>();
                nav1.TargetPatrol = patrolFounds[patrol].gameObject.transform;
                /*NavMeshAgent nav1 = ag1.GetComponentInChildren<NavMeshAgent>();
                nav1.SetDestination(patrolFounds[patrol].gameObject.transform.position);*/
                GameObject ag2 = Instantiate(enemiesPatron[patron][1], spawn1.position + new Vector3(0.5f, 0, -0.5f), spawn1.rotation);
                EnemyStateHandler nav2 = ag2.GetComponent<EnemyStateHandler>();
                nav2.TargetPatrol = patrolFounds[patrol].gameObject.transform;
                GameObject ag3 = Instantiate(enemiesPatron[patron][2], spawn1.position + new Vector3(-0.5f, 0, 0.5f), spawn1.rotation);
                EnemyStateHandler nav3 = ag3.GetComponent<EnemyStateHandler>();
                nav3.TargetPatrol = patrolFounds[patrol].gameObject.transform;
                enemiesCapacity += 3;
            }
            if (i == 1)
            {
                GameObject ag1 = Instantiate(enemiesPatron[patron][0], spawn2.position, spawn2.rotation);
                EnemyStateHandler nav1 = ag1.GetComponent<EnemyStateHandler>();
                nav1.TargetPatrol = patrolFounds[patrol].gameObject.transform;
                /*NavMeshAgent nav1 = ag1.GetComponentInChildren<NavMeshAgent>();
                nav1.SetDestination(patrolFounds[patrol].gameObject.transform.position);*/
                GameObject ag2 = Instantiate(enemiesPatron[patron][1], spawn2.position + new Vector3(0.5f, 0, -0.5f), spawn2.rotation);
                EnemyStateHandler nav2 = ag2.GetComponent<EnemyStateHandler>();
                nav2.TargetPatrol = patrolFounds[patrol].gameObject.transform;
                GameObject ag3 = Instantiate(enemiesPatron[patron][2], spawn2.position + new Vector3(-0.5f, 0, 0.5f), spawn2.rotation);
                EnemyStateHandler nav3 = ag3.GetComponent<EnemyStateHandler>();
                nav3.TargetPatrol = patrolFounds[patrol].gameObject.transform;
                enemiesCapacity += 3;
            }
        }
    }
}
