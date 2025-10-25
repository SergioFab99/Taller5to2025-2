using UnityEngine;

public class UnlockDoors : MonoBehaviour
{
    EnemyLife enemyLife;
    [SerializeField] private BoxCollider doorCollider1, doorCollider2, doorCollider3, doorCollider4;
    private void Awake()
    {
        doorCollider1.enabled = false;
        doorCollider2.enabled = false;
        doorCollider3.enabled = false;
        doorCollider4.enabled = true;
        SetUpTutorial.unlockMoreEnemies = false;
        SetUpTutorial.enemyDefeatCount = 0;
        enemyLife = GameObject.FindWithTag("Enemy").GetComponent<EnemyLife>();
        CountEnemiesDefeated();
    }
    private void Update()
    {
        if(enemyLife == null)
        {
            enemyLife = GameObject.FindWithTag("Enemy").GetComponent<EnemyLife>();
        }
        DoorsEnabled();
    }
    void CountEnemiesDefeated()
    {
        enemyLife.healthController.OnDead += EnemiesDefated;
    }

    void EnemiesDefated()
    {
        SetUpTutorial.enemyDefeatCount++;
        if (SetUpTutorial.enemyDefeatCount >= 1 && !SetUpTutorial.checkPoint1 && !SetUpTutorial.checkPoint2 && !SetUpTutorial.checkPoint3)
        {
            CanUnlocMoreEnemies();
        }
        if (SetUpTutorial.enemyDefeatCount >= 4 && !SetUpTutorial.checkPoint1 && !SetUpTutorial.checkPoint2 && !SetUpTutorial.checkPoint3)
        {
            CanUnlockDoor1();
        }
        if (SetUpTutorial.enemyDefeatCount >= 10 && SetUpTutorial.checkPoint1)
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
    }
    void CanUnlockDoor1()
    {
        SetUpTutorial.canOpenDoor1 = true;
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
        enemyLife.healthController.OnDead -= EnemiesDefated;
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
