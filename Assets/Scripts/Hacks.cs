using UnityEngine;
using UnityEngine.SceneManagement;
public class Hacks : MonoBehaviour
{
    public HealthController health;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene("LevelTutorial");
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            SceneManager.LoadScene("Level1B");
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            health.AddHealth(1000000000000);
        }
    }
}
