using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ManagerEnemiesInTutorial : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemiesActive = new List<GameObject>();
    [SerializeField] private List<GameObject> firstsEnemies = new List<GameObject>();
    [SerializeField] private List<GameObject> enemiesCheckpoint1 = new List<GameObject>();
    [SerializeField] private List<GameObject> enemiesCheckpoint2 = new List<GameObject>();
    [SerializeField] private List<GameObject> enemiesCheckpoint3 = new List<GameObject>();
    [SerializeField] private GameObject firstEnemy;
    void Awake()
    {
        if (!SetUpTutorial.checkPoint1 && !SetUpTutorial.checkPoint2 && !SetUpTutorial.checkPoint3)
        {
            enemiesActive.Add(firstEnemy);
            SpawnEnemiesInTutorial();
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
        if (!SetUpTutorial.checkPoint1 && !SetUpTutorial.checkPoint2 && !SetUpTutorial.checkPoint3 && SetUpTutorial.unlockMoreEnemies)
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
        }
    }
    void FirstEnemies()
    {
        enemiesActive = firstsEnemies;
        SpawnEnemiesInTutorial();
        SetUpTutorial.unlockMoreEnemies = false;
    }
    void EnemiesCheckPoint1()
    {
        enemiesActive = enemiesCheckpoint1;
        SpawnEnemiesInTutorial();
        SetUpTutorial.unlockMoreEnemies = false;
    }
    void EnemiesCheckPoint2()
    {
        enemiesActive = enemiesCheckpoint2;
        SpawnEnemiesInTutorial();
        SetUpTutorial.unlockMoreEnemies = false;
    }
    void EnemiesCheckPoint3()
    {
        enemiesActive = enemiesCheckpoint3;
        SpawnEnemiesInTutorial();
        SetUpTutorial.unlockMoreEnemies = false;
    }
    void SpawnEnemiesInTutorial()
    {
        if (!SetUpTutorial.checkPoint3)
        {
            for (int i = 0; i < enemiesActive.Count; i++)
            {
                enemiesActive[i].SetActive(true);
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
        }
    }
}
