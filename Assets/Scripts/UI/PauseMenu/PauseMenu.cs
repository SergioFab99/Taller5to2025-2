using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel, pauseMenu, shopMenu, optionsMenu;
    [SerializeField] private PlayerCamera playerCamera;
    private void Awake()
    {
        pausePanel.SetActive(false);
    }
    void Start()
    {
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale == 1f)
        {
            ActivePauseMenu();
        }
    }
    public void ActivePauseMenu()
    {
        Time.timeScale = 0f;
        playerCamera.SetLookLocked(true);
        pausePanel.SetActive(true);
        pauseMenu.SetActive(true);
        shopMenu.SetActive(false);
    }
    public void ResumeButton()
    {
        Time.timeScale = 1f;
        playerCamera.SetLookLocked(false);
        pausePanel.SetActive(false);
    }
    public void BackToMenuButton()
    {
        Time.timeScale = 1f;
        playerCamera.SetLookLocked(false);
        SceneManager.LoadScene("StartMenu");
    }
    public void ReturnButton()
    {
        pauseMenu.SetActive(true);
        shopMenu.SetActive(false);
        optionsMenu.SetActive(false);
    }
    public void ShopButton()
    {
        pauseMenu.SetActive(false);
        shopMenu.SetActive(true);
    }
    public void OptionsButton()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }
}
