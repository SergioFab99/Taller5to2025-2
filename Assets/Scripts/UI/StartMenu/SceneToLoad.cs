using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneToLoad : MonoBehaviour
{
    public static string sceneSaved = "LevelTutorial";

    private void Awake()
    {
        sceneSaved = SceneManager.GetActiveScene().name;
    }
}
