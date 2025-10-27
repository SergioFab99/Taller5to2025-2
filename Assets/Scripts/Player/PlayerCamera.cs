using UnityEngine;
using UnityEngine.InputSystem.XR;
using Unity.Cinemachine;

public struct  CameraInput
{
    public Vector2 Look;
}

public class PlayerCamera : MonoBehaviour
{
    private Vector3 _eulerAngles;
    private bool _lookLocked = false;

    [SerializeField] public GameObject _camera;
    private CinemachineCamera _CMCamera;

    public float[] Gain = new float[2];
    public float sensibility = 0.1f; 


    public void Initialize(Transform Target)
    {
        transform.position = Target.position;
        transform.rotation = Target.rotation;
        transform.eulerAngles = _eulerAngles = Target.eulerAngles;
       
       _CMCamera = _camera.GetComponent<CinemachineCamera>();

    }
   
   
    
    public void UpdateRotation(CameraInput input)
    {
        if (_lookLocked) return; 

        _eulerAngles += new Vector3(-input.Look.y * Gain[0], input.Look.x * Gain[1]) * sensibility;

        _eulerAngles.x = Mathf.Clamp(_eulerAngles.x, -90f, 90f);
        transform.eulerAngles = _eulerAngles;
    }

    public void UpdatePosition(Transform Target)
    {
        transform.position = Target.position;
    }

    public void UpdateGain(int i)
    {
       
    }

    public Vector3 GetCameraUp() => _CMCamera.transform.up;

    
    public void SetLookLocked(bool locked)
    {
        _lookLocked = locked;
    }
}
