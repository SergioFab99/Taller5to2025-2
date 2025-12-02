using System;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ButtonManager : MonoBehaviour
{
    [SerializeField, Tooltip("Nombre de la escena que se cargará al presionar el botón.")]
    private string sceneToLoad;

    public void LoadConfiguredScene()
    {
        LoadSceneInternal(sceneToLoad);
    }

    public void LoadSceneByName(string sceneName)
    {
        LoadSceneInternal(sceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
    EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void LoadSceneInternal(string targetScene)
    {
        if (string.IsNullOrWhiteSpace(targetScene))
        {
            Debug.LogWarning($"No se ha asignado un nombre de escena válido en {nameof(ButtonManager)} para '{gameObject.name}'.", this);
            return;
        }

#if UNITY_EDITOR
        if (!Application.CanStreamedLevelBeLoaded(targetScene))
        {
            Debug.LogWarning($"La escena '{targetScene}' no está incluida en Build Settings o el nombre es incorrecto.", this);
            return;
        }
#endif

        SceneManager.LoadScene(targetScene);
    }
}
