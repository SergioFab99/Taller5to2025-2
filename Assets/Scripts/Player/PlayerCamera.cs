using UnityEngine;
using UnityEngine.InputSystem.XR;
using Unity.Cinemachine;

public struct  CameraInput
{
    public Vector2 Look;
}

public class PlayerCamera : MonoBehaviour
{
    public LayerMask mask;
    private Vector3 _eulerAngles;
    private bool _lookLocked = false;
    private Transform _lockTarget = null;
    [SerializeField] private float _lockHeightOffset = 0.5f; 
    [SerializeField] private float _lockTurnSpeed = 999f; 
    [SerializeField] private float _yawDeadZoneDeg = 0.5f;
    [SerializeField] private bool _preservePitchOnLock = true;
    private float _savedPitchOnLock = 0f;

    [SerializeField] public GameObject _camera;
    private CinemachineCamera _CMCamera;

    public float[] Gain = new float[2];
    public float sensibility = 0.1f;

    private bool _requestedSideStep;
    private Transform _currentTarget;
    public void Initialize(Transform Target)
    {
        transform.position = Target.position;
        transform.rotation = Target.rotation;
        transform.eulerAngles = _eulerAngles = Target.eulerAngles;
       
       _CMCamera = _camera.GetComponent<CinemachineCamera>();

    }
   
   
    
    public void UpdateRotation(CameraInput input)
    {
        
        if (_lookLocked && _lockTarget != null)
        {
            
            Vector3 camPos = transform.position;
            Vector3 tgt = _lockTarget.position + Vector3.up * _lockHeightOffset;
            Vector3 flatDir = new Vector3(tgt.x - camPos.x, 0f, tgt.z - camPos.z);
            if (flatDir.sqrMagnitude > 0.0001f)
            {
                float currentYaw = transform.eulerAngles.y;
                float targetYaw = Quaternion.LookRotation(flatDir.normalized, Vector3.up).eulerAngles.y;
                float delta = Mathf.DeltaAngle(currentYaw, targetYaw);
                if (Mathf.Abs(delta) > _yawDeadZoneDeg)
                {
                    float t = (_lockTurnSpeed >= 999f) ? 1f : Mathf.Clamp01(Time.deltaTime * _lockTurnSpeed);
                    float newYaw = Mathf.LerpAngle(currentYaw, targetYaw, t);
                    float basePitch = _preservePitchOnLock ? _savedPitchOnLock : transform.eulerAngles.x;
                    float newPitch = Mathf.Clamp(basePitch, -90f, 90f);
                    transform.rotation = Quaternion.Euler(newPitch, newYaw, 0f);
                    _eulerAngles = transform.eulerAngles;
                }
            }
            return;
        }

     
        
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
        
        if (locked && !_lookLocked)
        {
            _savedPitchOnLock = transform.eulerAngles.x;
        }
        
        if (!locked && _lookLocked)
        {
            float yaw = transform.eulerAngles.y;
            float restoredPitch = Mathf.Clamp(_savedPitchOnLock, -90f, 90f);
            transform.rotation = Quaternion.Euler(restoredPitch, yaw, 0f);
            _eulerAngles = transform.eulerAngles;
        }
        _lookLocked = locked;
    }

    public void SetLookLockTarget(Transform target)
    {
        _lockTarget = target;
    }

    public bool CheckIsViewTarget(Transform character)
    {
        SetTargetOnView(character);
        return _currentTarget != null;
    }

    public void SetTargetOnView(Transform character)
    {
        Ray ray = new Ray(character.position, character.forward);

        if(Physics.SphereCast(ray,1f, out RaycastHit hit, 5f, mask) && hit.collider.gameObject.TryGetComponent<TagContainer>(out TagContainer tagContainer) && tagContainer.HasTag("Enemy"))
        {
            _currentTarget = hit.transform;
        }
        else
        {
            _currentTarget = null;
        }
    }

    public Transform GetTargetInView()
    {
        return _currentTarget;
    }
}
