using UnityEngine;

public class CheckPointsTutorial : MonoBehaviour
{

    [SerializeField] private GameObject player, spawnPoint, spawnPoint1, spawnPoint2, spawnPointFinal;
    [SerializeField] private SetUpTutorial setUpTutorial;
    private void Awake()
    {
        setUpTutorial = GetComponent<SetUpTutorial>();
        SpawnPoint3();
        player.transform.position = setUpTutorial.spawnPoint;
    }


    void SpawnPoint3()
    {
        if (!SetUpTutorial.checkPoint1 && !SetUpTutorial.checkPoint2 && !SetUpTutorial.checkPoint3)
        {
            setUpTutorial.spawnPoint = spawnPoint.transform.position;
        }
        if (SetUpTutorial.checkPoint1)
        {
            setUpTutorial.spawnPoint = spawnPoint1.transform.position;
        }
        if (SetUpTutorial.checkPoint2)
        {
            setUpTutorial.spawnPoint = spawnPoint2.transform.position;
        }
        if (SetUpTutorial.checkPoint3)
        {
            setUpTutorial.spawnPoint = spawnPointFinal.transform.position;
        }
    }
}
