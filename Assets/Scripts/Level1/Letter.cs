using UnityEngine;
using TMPro;
public class Letter : MonoBehaviour
{
    [SerializeField] [TextArea(4, 6)] private string letterText;
    [SerializeField] private GameObject letterHUD;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private TMP_Text letterTMP;
    void Awake()
    {
        letterHUD = GameObject.Find("Canvas (1)").transform.Find("LetterPanel").gameObject;
        letterTMP = letterHUD.transform.Find("LetterTMP").GetComponent<TMP_Text>();
        playerCamera = GameObject.Find("PlayerNewAnimations Variant").transform.Find("PlayerCamera").transform.Find("Camera").GetComponent<PlayerCamera>();
        gameObject.SetActive(false);
    }

    public void OpenLetter()
    {
        letterTMP.text = letterText;
        letterHUD.SetActive(true);
        Time.timeScale = 0f;
        playerCamera.SetLookLocked(true);
    }
}
