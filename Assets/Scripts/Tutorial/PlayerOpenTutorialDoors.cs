using UnityEngine;

public class PlayerOpenTutorialDoors : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Door"))
        {

        }
    }
}
