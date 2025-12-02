using UnityEngine;
using UnityEngine.SceneManagement;
public class StartMenuManager : MonoBehaviour
{
    [SerializeField] GameObject startMenuPanel;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] string sceneToLoad;
    private void Start()
    {
        startMenuPanel = transform.Find("StartMenuPanel").gameObject;
        optionsPanel = transform.Find("OptionsPanel").gameObject;
        startMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        sceneToLoad = SceneToLoad.sceneSaved;
    }
    public void PlayButton()
    {
        SceneManager.LoadScene(sceneToLoad);
      
    }

    
    public void OptionsButton()
    {
        startMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }
   
    public void ReturnOptionsButton()
    {
        startMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }
   
    public void ExitButton()
    {
        Application.Quit();
    }
}
