using UnityEngine;

public class CheckPointsTutorial : MonoBehaviour
{

    [SerializeField] private GameObject player, spawnPointFinal;
    private void Awake()
    {
        SpawnPoint3();
        player.transform.position = SetUpTutorial.spawnPoint;
    }


    void SpawnPoint3()
    {
        if (SetUpTutorial.checkPoint3)
        {
            SetUpTutorial.spawnPoint = spawnPointFinal.transform.position;
        }
    }
}
