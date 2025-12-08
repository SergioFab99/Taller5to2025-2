using UnityEngine;
using UnityEngine.SceneManagement;
public class CinematicGoNextScene : MonoBehaviour
{
    void GoSceneCinematic2()
    {
        SceneManager.LoadScene("Cinematic2");
    }
    void GoSceneTutorial()
    {
        SceneManager.LoadScene("LevelTutorial");
    }
}
