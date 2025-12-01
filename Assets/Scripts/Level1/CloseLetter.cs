using UnityEngine;

public class CloseLetter : MonoBehaviour
{
    [SerializeField] private GameObject letterHUD;
    [SerializeField] private PlayerCamera playerCamera;
    void Start()
    {
        letterHUD = GameObject.Find("LetterPanel");
        playerCamera = GameObject.Find("PlayerCamera").transform.Find("Camera").GetComponent<PlayerCamera>();
    }

    public void CloseLetters()
    {
        letterHUD.SetActive(false);
        Time.timeScale = 1f;
        playerCamera.SetLookLocked(false);
    }
}
