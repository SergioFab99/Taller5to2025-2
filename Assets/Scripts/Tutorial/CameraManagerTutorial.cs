using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Playables;
public class CameraManagerTutorial : MonoBehaviour
{
     private CinemachineCamera cameraScene;
    [SerializeField] private CinemachineCamera cameraPlayer;
    [SerializeField] private CinemachineBrain cinemachineBrain;
     private PlayableDirector playableDirector;
    [SerializeField] private bool cam1, cam2, cam3;

    void Start()
    {
        cameraScene = gameObject.GetComponent<CinemachineCamera>();
        playableDirector = gameObject.GetComponent<PlayableDirector>();
        cameraPlayer.Priority = 1;
        cameraScene.Priority = 0;
        Debug.Log("CameraExiste");
        if (SetUpTutorial.camera1 && cam1)
        {
            StartCameraTour1();
        }
    }

    void Update()
    {
        if (SetUpTutorial.camera2 && cam2)
        {
            Invoke(nameof(StartCameraTour2), 0.3f);
        }
        if (SetUpTutorial.camera3 && cam3)
        {
            Invoke(nameof(StartCameraTour3), 0.3f);
        }
    }
    void StartCameraTour1()
    {
        cinemachineBrain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.EaseInOut;
        cameraPlayer.Priority = 0;
        cameraScene.Priority = 1;
        Time.timeScale = 0;
        playableDirector.Play();
        Debug.Log("Camera1inicia");
        
    }
    void StartCameraTour2()
    {
        playableDirector.Play();
        cinemachineBrain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.EaseInOut;
        cameraPlayer.Priority = 0;
        cameraScene.Priority = 1;
        Time.timeScale = 0;
    }
    void StartCameraTour3()
    {
        playableDirector.Play();
        cinemachineBrain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.EaseInOut;
        cameraPlayer.Priority = 0;
        cameraScene.Priority = 1;
        Time.timeScale = 0;
    }
    void ChangePriorityCamera1Tutorial()
    {
        SetUpTutorial.camera1 = false;
        cinemachineBrain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.Cut;
        cameraScene.Priority = 0;
        cameraPlayer.Priority = 1;
        Time.timeScale = 1;
        Indications.instance.ActivateIndications();
        Destroy(this);
    }
    void ChangePriorityCamera2Tutorial()
    {
        SetUpTutorial.camera2 = false;
        cinemachineBrain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.Cut;
        cameraScene.Priority = 0;
        cameraPlayer.Priority = 1;
        Time.timeScale = 1;
        /*Indications.instance.NextIndication();
        Indications.instance.ActivateIndications();*/
        Destroy(this);
    }
    void ChangePriorityCamera3Tutorial()
    {
        SetUpTutorial.camera3 = false;
        cinemachineBrain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.Cut;
        cameraScene.Priority = 0;
        cameraPlayer.Priority = 1;
        Time.timeScale = 1;
        if (Indications.indicationsLife)
        {
            Indications.instance.NextIndication();
            Indications.instance.ActivateIndications();
        }
        else
        {
            Indications.indicationsLifeWithCamera = true;
            Indications.instance.ChangeToIndicationsOfLife();
        }
        Destroy(this);
    }
}
