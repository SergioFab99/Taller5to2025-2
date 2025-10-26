using UnityEngine;
using TMPro;
public class Indications : MonoBehaviour
{
    [SerializeField] private GameObject indicationsBackground, next, back, close;
    [SerializeField] private HealthController playerLife;
    [SerializeField] private TMP_Text indicationsText, indicationsTitle;
    [SerializeField] private string[] textsForTitle;
    [SerializeField] [TextArea(4, 6)] private string[] textsForIndications;
    [SerializeField] private string[] textsForTitleLife;
    [SerializeField] [TextArea(2, 4)] private string[] textsForIndicationsLife;
    public static int changeIndications;
    public static bool indicationsLife;
    public static Indications instance;
    private void Awake()
    {
        indicationsBackground.SetActive(false);
    }
    void Start()
    {
        instance = this;       
        ChangeIndicationsOnAwake();
        ChangeIndications();
    }
    private void Update()
    {
        /*if (!indicationsLife && playerLife.health != playerLife.maxHealth)
        {
            ChangeToIndicationsOfLife();
        }*/
    }
    public void NextIndication()
    {
        changeIndications++;
        ActiveExtras();
        ChangeIndications();
    }
    public void BackIndication()
    {
        changeIndications--;
        ActiveExtras();
        ChangeIndications();
    }
    void ChangeIndicationsOnAwake()
    {
        if (!SetUpTutorial.checkPoint1 && !SetUpTutorial.checkPoint2 && !SetUpTutorial.checkPoint3)
        {
            changeIndications = 0;
            if (!SetUpTutorial.camera1)
            {
                ChangeIndications();
                ActivateIndications();
            }
        }
        if (SetUpTutorial.checkPoint1)
        {
            changeIndications = 5;

            if (!SetUpTutorial.camera2)
            {
                ChangeIndications();
                ActivateIndications();
            }
        }
        if (SetUpTutorial.checkPoint2)
        {
            changeIndications = 6;

            if (!SetUpTutorial.camera3)
            {
                ChangeIndications();
                ActivateIndications();
            }
        }
        if (SetUpTutorial.checkPoint3)
        {
            changeIndications = 6;
        }
    }
    void ChangeIndications()
    {
        changeIndications = Mathf.Clamp(changeIndications, 0, 7);
        indicationsTitle.text = textsForTitle[changeIndications];
        indicationsText.text = textsForIndicationsLife[changeIndications];
    }
    public void ChangeToIndicationsOfLife()
    {
        indicationsTitle.text = textsForTitleLife[0];
        indicationsText.text = textsForIndications[0];
        close.SetActive(true);
        indicationsBackground.SetActive(true);
        next.SetActive(false);
        back.SetActive(false);
        Time.timeScale = 0;
        indicationsLife = true;
    }
    void ActiveExtras()
    {
        if(changeIndications == 0 || changeIndications == 1 || changeIndications == 4 || changeIndications == 5 || changeIndications == 6 || changeIndications == 7)
        {
            close.SetActive(true);
            next.SetActive(false);
        }
        if(changeIndications == 3 || changeIndications == 4)
        {
            back.SetActive(true);
            next.SetActive(false);
        }
        else
        {
            back.SetActive(false);
        }
        if(changeIndications == 2 || changeIndications == 3)
        {
            next.SetActive(true);
            close.SetActive(false);
        }
    }
    public void ActivateIndications()
    {
        indicationsBackground.SetActive(true);
        ActiveExtras();
        Time.timeScale = 0;
    }
    public void CloseIndications()
    {
        indicationsBackground.SetActive(false);
        Time.timeScale = 1;
        if (indicationsLife)
        {
            ChangeIndications();
            if (SetUpTutorial.checkPoint2)
            {
                ActivateIndications();
            }
        }
    }
}
