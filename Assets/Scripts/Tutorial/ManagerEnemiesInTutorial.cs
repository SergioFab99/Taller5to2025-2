using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ManagerEnemiesInTutorial : MonoBehaviour
{
    [SerializeField] public List<GameObject> enemiesActive = new List<GameObject>();
    [SerializeField] private List<GameObject> firstsEnemies = new List<GameObject>();
    [SerializeField] private List<GameObject> enemiesCheckpoint1 = new List<GameObject>();
    [SerializeField] private List<GameObject> enemiesCheckpoint2 = new List<GameObject>();
    [SerializeField] private List<GameObject> enemiesCheckpoint3 = new List<GameObject>();
    public List<HealthController> enemyLife;
    [SerializeField] private GameObject firstEnemy;
    [SerializeField] private GameObject triggerIndication;
    [SerializeField] private GameObject[] prefabType;
    [SerializeField] private GameObject[] spawnPoints;
    bool starSpawn, startCoroutine;
    [SerializeField] private int timeBetweenMove;
    public static ManagerEnemiesInTutorial instance;
    Coroutine coroutine;
    int place;
    void Awake()
    {
        triggerIndication.SetActive(false);
        instance = this;
        /*for(int i = 0; i < firstsEnemies.Count; i++)
        {
            firstsEnemies[i].SetActive(false);
        }
        for(int i = 0; i < enemiesCheckpoint1.Count; i++)
        {
            enemiesCheckpoint1[i].SetActive(false);
        }
        for(int i = 0; i < enemiesCheckpoint2.Count; i++)
        {
            enemiesCheckpoint2[i].SetActive(false);
        }
        for(int i = 0; i < enemiesCheckpoint3.Count; i++)
        {
            enemiesCheckpoint3[i].SetActive(false);
        }*/
    }
    private void Start()
    {
        if (!SetUpTutorial.checkPoint1 && !SetUpTutorial.checkPoint2 && !SetUpTutorial.checkPoint3)
        {
            /*enemiesActive.Add(firstEnemy);
            SpawnEnemiesInTutorial();*/
            starSpawn = true;
            coroutine = StartCoroutine(SpawnEnemy(timeBetweenMove, 1, 0));
        }
        if (SetUpTutorial.checkPoint1)
        {
            EnemiesCheckPoint1();
        }
        if (SetUpTutorial.checkPoint2)
        {
            EnemiesCheckPoint2();
        }
        if (SetUpTutorial.checkPoint3)
        {
            EnemiesCheckPoint3();
        }
    }
    void Update()
    {
        /*if (!SetUpTutorial.checkPoint1 && !SetUpTutorial.checkPoint2 && !SetUpTutorial.checkPoint3 && SetUpTutorial.unlockMoreEnemies)
        {
            Invoke(nameof(FirstEnemies), 0.1f);
        }
        if (SetUpTutorial.checkPoint1 && SetUpTutorial.unlockMoreEnemies)
        {
            Invoke(nameof(EnemiesCheckPoint1), 0.1f);
        }
        if (SetUpTutorial.checkPoint2 && SetUpTutorial.unlockMoreEnemies)
        {
            Invoke(nameof(EnemiesCheckPoint2), 0.1f);
        }
        if (SetUpTutorial.checkPoint3 && SetUpTutorial.unlockMoreEnemies)
        {
            Invoke(nameof(EnemiesCheckPoint3), 0.2f);
        }*/
        if (!starSpawn)
        {
            StopAllCoroutines();
        }
        Debug.Log("startSpawn"+starSpawn);
        if (SetUpTutorial.checkPoint3)
        {
            if (SetUpTutorial.enemyDefeatCount >= 7)
            {
                Indications.instance.NextIndication();
                Indications.instance.ActivateIndications();
            }
        }
    }
    void FirstEnemies()
    {
        /*enemiesActive.Clear();
        enemiesActive = firstsEnemies;
        SpawnEnemiesInTutorial();*/
        starSpawn = true;
        coroutine = StartCoroutine(SpawnEnemy(timeBetweenMove, 3, 0));
        Debug.Log("Enemigos del principio");
        triggerIndication.SetActive(true);
        SetUpTutorial.unlockMoreEnemies = false;
    }
    void EnemiesCheckPoint1()
    {
        /*enemiesActive.Clear();
        enemiesActive = enemiesCheckpoint1;
        SpawnEnemiesInTutorial();*/
        starSpawn = true;
        Debug.Log("Enemigos checkpoint 1");
        coroutine = StartCoroutine(SpawnEnemy(timeBetweenMove, 7, 0));
        SetUpTutorial.unlockMoreEnemies = false;
    }
    void EnemiesCheckPoint2()
    {
        /*enemiesActive.Clear();
        enemiesActive = enemiesCheckpoint2;
        SpawnEnemiesInTutorial();*/
        starSpawn = true;
        coroutine = StartCoroutine(SpawnEnemy(timeBetweenMove, 7, 1));
        SetUpTutorial.unlockMoreEnemies = false;
    }
    void EnemiesCheckPoint3()
    {
        /*enemiesActive.Clear();
        enemiesActive = enemiesCheckpoint3;
        SpawnEnemiesInTutorial();*/
        starSpawn = true;
        coroutine = StartCoroutine(SpawnEnemy(timeBetweenMove, 1, 0));
        SetUpTutorial.unlockMoreEnemies = false;
    }
    void SpawnEnemiesInTutorial()
    {
        if (!SetUpTutorial.checkPoint3)
        {
            for (int i = 0; i < enemiesActive.Count; i++)
            {
                enemiesActive[i].SetActive(true);
                enemyLife.Add(enemiesActive[i].GetComponentInChildren<HealthController>());
            }
        }
        else
        {
            if(SetUpTutorial.enemyDefeatCount == 0)
            {
                enemiesActive[0].SetActive(true);
                enemiesActive.RemoveAt(0);
                enemiesCheckpoint3.RemoveAt(0);
                enemiesActive[0].SetActive(true);
                enemiesActive.RemoveAt(0);
                enemiesCheckpoint3.RemoveAt(0);
            }
            if(SetUpTutorial.enemyDefeatCount == 2)
            {
                enemiesActive[0].SetActive(true);
                enemiesActive.RemoveAt(0);
                enemiesCheckpoint3.RemoveAt(0);
                enemiesActive[0].SetActive(true);
                enemiesActive.RemoveAt(0);
                enemiesCheckpoint3.RemoveAt(0);
            }
            if(SetUpTutorial.enemyDefeatCount == 4)
            {
                enemiesActive[0].SetActive(true);
                enemiesActive.RemoveAt(0);
                enemiesCheckpoint3.RemoveAt(0);
                enemiesActive[0].SetActive(true);
                enemiesActive.RemoveAt(0);
                enemiesCheckpoint3.RemoveAt(0);
                enemiesActive[0].SetActive(true);
                enemiesActive.RemoveAt(0);
                enemiesCheckpoint3.RemoveAt(0);
            }
            if(SetUpTutorial.enemyDefeatCount >= 7)
            {
                Indications.instance.NextIndication();
                Indications.instance.ActivateIndications();
            }
        }        
    }
    
    public void ChangePlaceOnAwake(int place)
    {
        this.place = place;
    }
    public void CallSpawn()
    {
        
        Invoke(nameof(StartSpawn), 0.05f);
    }
    void StartSpawn()
    {
        Debug.Log("chekpointSpawn1" + SetUpTutorial.checkPoint1);
        Debug.Log("unlockMoreEnemies" + SetUpTutorial.unlockMoreEnemies);
        if (!SetUpTutorial.checkPoint1 && !SetUpTutorial.checkPoint2 && !SetUpTutorial.checkPoint3 && SetUpTutorial.unlockMoreEnemies)
        {
            Invoke(nameof(FirstEnemies), 0.1f);
        }
        if (SetUpTutorial.checkPoint1 && SetUpTutorial.unlockMoreEnemies)
        {
            Debug.Log("Empieza coroutine enemigos chekcpoint 1");
            Invoke(nameof(EnemiesCheckPoint1), 0.1f);
        }
        if (SetUpTutorial.checkPoint2 && SetUpTutorial.unlockMoreEnemies)
        {
            Invoke(nameof(EnemiesCheckPoint2), 0.1f);
        }
        if (SetUpTutorial.checkPoint3 && SetUpTutorial.unlockMoreEnemies)
        {
            Invoke(nameof(EnemiesCheckPoint3), 0.2f);
        }        
    }
    IEnumerator SpawnEnemy(float timeBetweenMove, int lenght, int type)
    {

        while (!startCoroutine)
        {
            if (starSpawn)
            {
                SpawnPoints(lenght, type);
                starSpawn = false;
                yield return new WaitForSeconds(timeBetweenMove);
            }
            yield return new WaitForSeconds(0f);
        }

    }
    void SpawnPoints(int lenght, int type)
    {
        if (!SetUpTutorial.checkPoint3)
        {
            for (int i = 0; i < lenght; i++)
            {
                Instantiate(prefabType[type], spawnPoints[place].gameObject.transform.position, spawnPoints[place].gameObject.transform.rotation);
                Debug.Log("Place" + place);
                place++;
            }
        }
        else
        {
            if (SetUpTutorial.enemyDefeatCount == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Instantiate(prefabType[0], spawnPoints[place].transform);
                    place++;
                }
            }
            if (SetUpTutorial.enemyDefeatCount == 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    Instantiate(prefabType[1], spawnPoints[place].transform);
                    place++;
                }
            }
            if (SetUpTutorial.enemyDefeatCount == 4)
            {
                for (int i = 0; i < 2; i++)
                {
                    Instantiate(prefabType[1], spawnPoints[place].transform);
                    place++;
                }
                Instantiate(prefabType[0], spawnPoints[place].transform.position, spawnPoints[place].transform.rotation);
            }
        }
    }
}
