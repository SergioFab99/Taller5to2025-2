using UnityEngine;
using TMPro;
public class Letter : MonoBehaviour
{
    [SerializeField] [TextArea(4, 6)] private string letterText;
    [SerializeField] private GameObject letterHUD;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private TMP_Text letterTMP;
    void Start()
    {
        letterHUD = GameObject.Find("LetterPanel");
        letterTMP = letterHUD.transform.Find("LetterTMP").GetComponent<TMP_Text>();
        playerCamera = GameObject.Find("PlayerCamera").transform.Find("Camera").GetComponent<PlayerCamera>();
    }

    public void OpenLetter()
    {
        letterTMP.text = letterText;
        letterHUD.SetActive(true);
        Time.timeScale = 1f;
        playerCamera.SetLookLocked(false);
    }
}
