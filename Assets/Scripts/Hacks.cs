using UnityEngine;
using UnityEngine.SceneManagement;
public class Hacks : MonoBehaviour
{
    public HealthController health;
    public bool cantheal;
    public GameObject prefabBate;
    public Transform player;
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
        if (Input.GetKeyDown(KeyCode.F3) && !cantheal)
        {
            health.AddHealth(1000000000000);
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            SceneManager.LoadScene("Cinematic1");
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SceneManager.LoadScene("Cinematic2");
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            SceneManager.LoadScene("Cinematic3");
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            SceneManager.LoadScene("StartMenu");
        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            SceneManager.LoadScene("Level2B");
        }
        if(Input.GetKeyDown(KeyCode.F9) && !cantheal)
        {
            Instantiate(prefabBate, player);
        }
    }
}
