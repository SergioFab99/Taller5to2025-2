using UnityEngine;
using UnityEngine.SceneManagement;
public class CinematicGoNextScene : MonoBehaviour
{
    void GoSceneCinematic2()
    {
        SceneManager.LoadScene("Cinematic2");
    }
    void GoSceneCinematic3()
    {
        SceneManager.LoadScene("Cinematic3");
    }
    void GoSceneTutorial()
    {
        SceneManager.LoadScene("LevelTutorial");
    }
    void GoSceneStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }
}
