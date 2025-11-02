using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SpawnEnemiesLvl1 : MonoBehaviour
{
    [SerializeField] private float timer, maxTimer;
    [SerializeField] private GameObject player;
    [SerializeField] private SpawnPointsForEnemies[] spawnfounds;
    [SerializeField] private GameObject[][] enemiesPatron;
    [SerializeField] private GameObject[] patron1, patron2, patron3;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        spawnfounds = transform.GetComponentsInChildren<SpawnPointsForEnemies>();
    }
    void Start()
    {
        timer = maxTimer;
        enemiesPatron = new GameObject[3][];

        enemiesPatron[0] = patron1;
        enemiesPatron[1] = patron2;
        enemiesPatron[2] = patron3;
    }

    void FixedUpdate()
    {
        timer -= Time.fixedDeltaTime;
        if(timer <= 0)
        {
            FindSpawnPoint();
            timer = maxTimer;
        }
    }

    void FindSpawnPoint()
    {
        if(spawnfounds.Length > 0)
        {
            SpawnPointsForEnemies spawnNear1 = spawnfounds[0];
            SpawnPointsForEnemies spawnNear2 = spawnfounds[1];
            SpawnPointsForEnemies spawnNear3 = spawnfounds[2];
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
            Spawn(spawnNear2.transform, spawnNear3.transform);
        }        
    }
    void Spawn(Transform spawn1, Transform spawn2)
    {
        for(int i = 0; i < 2; i++)
        {
            int patron = Random.Range(0, enemiesPatron.Length);
            if(i == 0)
            {
                Instantiate(enemiesPatron[patron][0], spawn1.position, spawn1.rotation);
                Instantiate(enemiesPatron[patron][1], spawn1.position + new Vector3(0.5f, 0, -0.5f), spawn1.rotation);
                Instantiate(enemiesPatron[patron][2], spawn1.position + new Vector3(-0.5f, 0, 0.5f), spawn1.rotation);
            }
            if (i == 1)
            {
                Instantiate(enemiesPatron[patron][0], spawn2.position, spawn2.rotation);
                Instantiate(enemiesPatron[patron][1], spawn2.position + new Vector3(0.5f, 0, -0.5f), spawn2.rotation);
                Instantiate(enemiesPatron[patron][2], spawn2.position + new Vector3(-0.5f, 0, 0.5f), spawn2.rotation);
            }
        }
    }
}
