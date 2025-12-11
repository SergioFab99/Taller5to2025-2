using UnityEngine;
using UnityEngine.SceneManagement;
using GameJolt.UI;

public class MainScene : MonoBehaviour
{
    void Start()
    {
        GameJoltUI.Instance.ShowSignIn((success) =>
        {
            if (success)
            {
                SceneManager.LoadScene("LevelTutorial");
            }
            else
            {
                Debug.Log("Error in Sign In");
            }
        });
    } }
