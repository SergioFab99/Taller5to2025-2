using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel, pauseMenu, shopMenu;
    [SerializeField] private PlayerCamera playerCamera;
    void Start()
    {
        pausePanel.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0f;
            playerCamera.SetLookLocked(true);
            pausePanel.SetActive(true);
            pauseMenu.SetActive(true);
            shopMenu.SetActive(false);
        }
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
    }
    public void ShopButton()
    {
        pauseMenu.SetActive(false);
        shopMenu.SetActive(true);
    }
}
