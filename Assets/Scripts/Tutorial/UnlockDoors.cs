using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UnlockDoors : MonoBehaviour
{
    [SerializeField] private Object[] enemyLife;
    [SerializeField] private List<HealthController> enemiesLife;
    [SerializeField] private HealthController enemiesLifes;
    [SerializeField] private ManagerEnemiesInTutorial enemies;
    [SerializeField] private BoxCollider doorCollider1, doorCollider2, doorCollider3, doorCollider4;
    [SerializeField] private GameObject door4;
    public static UnlockDoors instance;
    private void Awake()
    {
        SetUpTutorial.enemyDefeatCount = 0;
    }
    private void Start()
    {
        instance = this;
        enemies = GetComponent<ManagerEnemiesInTutorial>();
        doorCollider1.enabled = false;
        doorCollider2.enabled = false;
        doorCollider3.enabled = false;
        door4.SetActive(true);
        doorCollider4.enabled = true;
        SetUpTutorial.unlockMoreEnemies = false;
        /*try
        {
            Debug.Log("Enemigos");
            enemyLife = FindObjectsByType(typeof(HealthController), FindObjectsSortMode.None);
        }
        catch
        {
            Debug.Log("No hay enemigos");
        }*/
        
        //enemyLife = enemies.enemyLife;
        CountEnemiesDefeated();
    }
    private void Update()
    {
       /* Debug.Log($"Checkpoint1 {SetUpTutorial.checkPoint1}");
        Debug.Log($"Checkpoint2 {SetUpTutorial.checkPoint2}");
        Debug.Log($"Checkpoint3 {SetUpTutorial.checkPoint3}");
            /*var enemyLifes = enemyLife;
            if (enemyLifes.TryGetComponent<HealthController>(out HealthController enemies))
            {
                enemyLife = enemies;
            }*/
        
        
        DoorsEnabled();
    }
    public void CountEnemiesDefeated()
    {
        /*for(int i = 0; i < enemyLife.Length; i++)
        {
            enemiesLife.Add(enemyLife[i]);
            enemyLife[i].GetType(typeof(HealthController)).OnDead += EnemiesDefated;
        }*/

        /*try
        {
            Debug.Log("Sí hay enemigos");
            enemyLife = FindObjectsByType(typeof(HealthController), FindObjectsSortMode.None);
        }
        catch
        {
            Debug.Log("No hay enemigos");
        }
        foreach (HealthController enemiesLifes in enemyLife)
        {
            if (enemiesLifes.gameObject.CompareTag("Enemy"))
            {
                enemiesLife.Add(enemiesLifes);
            }
                    
           
        }
        for (int i = 0; i < enemiesLife.Count; i++)
        {
            enemiesLife[i].OnDead += EnemiesDefated;
        }*/
        
    }

    public void EnemiesDefated()
    {
        /*for (int i = 0; i < enemiesLife.Count; i++)
        {
            if (enemiesLife[i] == null)
            {
                enemiesLife[i].OnDead -= EnemiesDefated;
                enemiesLife.Remove(enemiesLife[i]);
            }
                
        }
        SetUpTutorial.enemyDefeatCount++;
        Debug.Log(SetUpTutorial.enemyDefeatCount);

       /* for (int i = 0; i < enemies.enemiesActive.Count; i++)
        {
            if (enemies.enemyLife[i] == null)
            {
                enemies.enemiesActive.Remove(enemies.enemiesActive[i]);
            }
        }

        for (int i = 0; i < enemies.enemyLife.Count; i++)
        {
            if (enemies.enemyLife[i] == null)
            {
                enemies.enemyLife.Remove(enemies.enemyLife[i]);
            }
        }
        for (int i = 0; i < enemyLife.Length; i++)
        {
            if(enemyLife[i] == null)
            {
                enemyLife.Remove(enemyLife[i]);
            }
        }*/
        if (SetUpTutorial.enemyDefeatCount == 1 && !SetUpTutorial.checkPoint1 && !SetUpTutorial.checkPoint2 && !SetUpTutorial.checkPoint3)
        {
            CanUnlocMoreEnemies();
            ManagerEnemiesInTutorial.instance.CallSpawn();
        }
        if (SetUpTutorial.enemyDefeatCount == 4 && !SetUpTutorial.checkPoint1 && !SetUpTutorial.checkPoint2 && !SetUpTutorial.checkPoint3)
        {
            CanUnlockDoor1();
            CanUnlocMoreEnemies();
        }
        if (SetUpTutorial.enemyDefeatCount == 7 && SetUpTutorial.checkPoint1)
        {
            CanUnlockDoor2();
            CanUnlocMoreEnemies();
        }
        if (SetUpTutorial.enemyDefeatCount == 1 && SetUpTutorial.checkPoint2)
        {
            CanUnlockDoor3();
            CanUnlocMoreEnemies();
            //ManagerEnemiesInTutorial.instance.CallSpawn();
        }
        if (SetUpTutorial.enemyDefeatCount == 2 && SetUpTutorial.checkPoint3)
        {
            CanUnlocMoreEnemies();
            ManagerEnemiesInTutorial.instance.CallSpawn();
        }
        if (SetUpTutorial.enemyDefeatCount == 4 && SetUpTutorial.checkPoint3)
        {
            CanUnlocMoreEnemies();
            ManagerEnemiesInTutorial.instance.CallSpawn();
        }
    }
    void CanUnlocMoreEnemies()
    {
        SetUpTutorial.unlockMoreEnemies = true;
        Debug.Log($"unlockEnemies {SetUpTutorial.unlockMoreEnemies}");

        //Invoke(nameof(CountEnemiesDefeated), 0.15f);

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
        /*for (int i = 0; i < enemiesLife.Count; i++)
        {
            enemiesLife[i].OnDead -= EnemiesDefated;
        }*/
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
            door4.SetActive(false);
            doorCollider4.enabled = false;
        }
    }
}
