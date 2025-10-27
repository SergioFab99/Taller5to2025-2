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
        if (SetUpTutorial.camera1 && cam1)
        {
            cinemachineBrain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.EaseInOut;
            cameraPlayer.Priority = 0;
            cameraScene.Priority = 1;
            Time.timeScale = 0;
        }
    }

    void Update()
    {
        if (SetUpTutorial.camera2 && cam2)
        {
            playableDirector.Play();
            cinemachineBrain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.EaseInOut;
            cameraPlayer.Priority = 0;
            cameraScene.Priority = 1;
            Time.timeScale = 0;
        }
        if (SetUpTutorial.camera3 && cam3)
        {
            playableDirector.Play();
            cinemachineBrain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.EaseInOut;
            cameraPlayer.Priority = 0;
            cameraScene.Priority = 1;
            Time.timeScale = 0;
        }
    }

    void ChangePriorityCamera1Tutorial()
    {
        SetUpTutorial.camera1 = false;
        cinemachineBrain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.Cut;
        cameraScene.Priority = 0;
        cameraPlayer.Priority = 1;
        Time.timeScale = 1;
        Destroy(this);
    }
    void ChangePriorityCamera2Tutorial()
    {
        SetUpTutorial.camera2 = false;
        cinemachineBrain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.Cut;
        cameraScene.Priority = 0;
        cameraPlayer.Priority = 1;
        Time.timeScale = 1;
        Destroy(this);
    }
    void ChangePriorityCamera3Tutorial()
    {
        SetUpTutorial.camera3 = false;
        cinemachineBrain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.Cut;
        cameraScene.Priority = 0;
        cameraPlayer.Priority = 1;
        Time.timeScale = 1;
        Destroy(this);
    }
}
