using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UnlockDoors : MonoBehaviour
{
    [SerializeField] private List<HealthController> enemyLife;
    [SerializeField] private ManagerEnemiesInTutorial enemies;
    [SerializeField] private BoxCollider doorCollider1, doorCollider2, doorCollider3, doorCollider4;
    private void Start()
    {
        enemies = GetComponent<ManagerEnemiesInTutorial>();
        doorCollider1.enabled = false;
        doorCollider2.enabled = false;
        doorCollider3.enabled = false;
        doorCollider4.enabled = true;
        SetUpTutorial.unlockMoreEnemies = false;
        SetUpTutorial.enemyDefeatCount = 0;
        enemyLife = enemies.enemyLife;
        CountEnemiesDefeated();
    }
    private void Update()
    {
        Debug.Log($"Checkpoint1 {SetUpTutorial.checkPoint1}");
        Debug.Log($"Checkpoint2 {SetUpTutorial.checkPoint2}");
        Debug.Log($"Checkpoint3 {SetUpTutorial.checkPoint3}");
        if(enemyLife == null)
        {
            /*var enemyLifes = enemyLife;
            if (enemyLifes.TryGetComponent<HealthController>(out HealthController enemies))
            {
                enemyLife = enemies;
            }*/
            try
            {
                enemyLife = enemies.enemyLife;
            }
            catch
            {
                Debug.Log("No hay enemigos");
            }
        }
        DoorsEnabled();
    }
    void CountEnemiesDefeated()
    {
        for(int i = 0; i < enemyLife.Count; i++)
        {
            enemyLife[i].OnDead += EnemiesDefated;
        }
    }

    void EnemiesDefated()
    {
        SetUpTutorial.enemyDefeatCount++;
        Debug.Log(SetUpTutorial.enemyDefeatCount);
        for (int i = 0; i < enemyLife.Count; i++)
        {
            if(enemyLife[i] == null)
            {
                enemyLife.Remove(enemyLife[i]);
            }
        }
        for (int i = 0; i < enemies.enemyLife.Count; i++)
        {
            if(enemies.enemyLife[i] == null)
            {
                enemies.enemyLife.Remove(enemies.enemyLife[i]);
            }
        }
        for (int i = 0; i < enemies.enemiesActive.Count; i++)
        {
            if(enemies.enemyLife[i] == null)
            {
                enemies.enemiesActive.Remove(enemies.enemiesActive[i]);
            }
        }
        if (SetUpTutorial.enemyDefeatCount >= 1 && !SetUpTutorial.checkPoint1 && !SetUpTutorial.checkPoint2 && !SetUpTutorial.checkPoint3)
        {
            CanUnlocMoreEnemies();
        }
        if (SetUpTutorial.enemyDefeatCount >= 4 && !SetUpTutorial.checkPoint1 && !SetUpTutorial.checkPoint2 && !SetUpTutorial.checkPoint3)
        {
            CanUnlockDoor1();
            CanUnlocMoreEnemies();
        }
        if (SetUpTutorial.enemyDefeatCount >= 7 && SetUpTutorial.checkPoint1)
        {
            CanUnlockDoor2();
            CanUnlocMoreEnemies();
        }
        if (SetUpTutorial.enemyDefeatCount >= 1 && SetUpTutorial.checkPoint2)
        {
            CanUnlockDoor3();
            CanUnlocMoreEnemies();
        }
        if (SetUpTutorial.enemyDefeatCount >= 2 && SetUpTutorial.checkPoint3)
        {
            CanUnlocMoreEnemies();
        }
        if (SetUpTutorial.enemyDefeatCount >= 4 && SetUpTutorial.checkPoint3)
        {
            CanUnlocMoreEnemies();
        }
    }
    void CanUnlocMoreEnemies()
    {
        SetUpTutorial.unlockMoreEnemies = true;
        Invoke(nameof(CountEnemiesDefeated), 0.25f);
    }
    void CanUnlockDoor1()
    {
        SetUpTutorial.canOpenDoor1 = true;
        Debug.Log("Puerta1 Abrir");
        SetUpTutorial.enemyDefeatCount = 0;
    }
    void CanUnlockDoor2()
    {
        SetUpTutorial.canOpenDoor2 = true;
        SetUpTutorial.enemyDefeatCount = 0;
    }
    void CanUnlockDoor3()
    {
        SetUpTutorial.canOpenDoor3 = true;
        SetUpTutorial.enemyDefeatCount = 0;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < enemyLife.Count; i++)
        {
            enemyLife[i].OnDead -= EnemiesDefated;
        }
    }

    void DoorsEnabled()
    {
        if (SetUpTutorial.canOpenDoor1)
        {
            doorCollider1.enabled = true;
            doorCollider2.enabled = true;
        }
        if (SetUpTutorial.canOpenDoor2)
        {
            doorCollider3.enabled = true;
        }
        if (SetUpTutorial.canOpenDoor3)
        {
            doorCollider3.enabled = false;
        }
    }
}
