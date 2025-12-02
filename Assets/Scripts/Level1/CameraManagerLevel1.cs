using UnityEngine;
using System;
using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
public class CameraManagerLevel1 : MonoBehaviour
{
    [SerializeField] private GameObject cameraScene1, cameraScene2, cameraScene3, cameraScene4;
    [SerializeField] private CinemachineBrain cinemachineBrain;
    public bool startCam;
    public event StartCamera startCameras;
    public delegate void StartCamera();
    [SerializeField] private float timeBetweenCameras;
    void Start()
    {
        cameraScene1.SetActive(false);
        cameraScene2.SetActive(false);
        cameraScene3.SetActive(false);
        cameraScene4.SetActive(false);
        startCameras += CameraStart;
        startCam = true;
    }
    void Update()
    {
        if (startCam)
        {
            startCameras?.Invoke();
            startCam = false;
        }
    }

    void CameraStart()
    {
        Time.timeScale = 0f;
        StartCoroutine(ChangeCamera());
    }
    IEnumerator ChangeCamera()
    {
        if (startCam)
        {
            cinemachineBrain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.Cut;
            cameraScene1.SetActive(true);
            yield return new WaitForSecondsRealtime(timeBetweenCameras);
            cameraScene2.SetActive(true);
            cameraScene1.SetActive(false);
            yield return new WaitForSecondsRealtime(timeBetweenCameras);
            cameraScene3.SetActive(true);
            cameraScene2.SetActive(false);
            yield return new WaitForSecondsRealtime(timeBetweenCameras);
            cameraScene4.SetActive(true);
            cameraScene3.SetActive(false);
            yield return new WaitForSecondsRealtime(timeBetweenCameras);
            cameraScene4.SetActive(false);
            Time.timeScale = 1f;
            StopAllCoroutines();
            startCam = false;
            yield return new WaitForSeconds(0f);
        }
        yield return new WaitForSeconds(0f);
    }
    private void OnDestroy()
    {
        startCameras -= CameraStart;
    }
}
