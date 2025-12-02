using UnityEngine;

public class CheckPointsTutorial : MonoBehaviour
{
    Player playerTp;
    [SerializeField] private GameObject player, spawnPoint, spawnPoint1, spawnPoint2, spawnPointFinal;
    [SerializeField] private SetUpTutorial setUpTutorial;
    private void Awake()
    {
        //setUpTutorial = GetComponent<SetUpTutorial>();
        playerTp = player.GetComponent<Player>();
        //player.transform.position = setUpTutorial.spawnPoint;
    }
    private void Start()
    {
        SpawnPoints();
    }

    void SpawnPoints()
    {
        if (!SetUpTutorial.checkPoint1 && !SetUpTutorial.checkPoint2 && !SetUpTutorial.checkPoint3)
        {
            playerTp.Teleport(spawnPoint.transform.position);
        }
        if (SetUpTutorial.checkPoint1)
        {
            playerTp.Teleport(spawnPoint1.transform.position);
        }
        if (SetUpTutorial.checkPoint2)
        {
            playerTp.Teleport(spawnPoint2.transform.position);
        }
        if (SetUpTutorial.checkPoint3)
        {
            playerTp.Teleport(spawnPointFinal.transform.position);
        }
    }
}
