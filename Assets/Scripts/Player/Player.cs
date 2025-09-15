using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerCharacter playerCharacter;
    [SerializeField] PlayerCamera playerCamera;

    PlayerInputActions _inputActions;

    [SerializeField] CharacterState _characterState;
    [SerializeField] CharacterState _lastCharacterState;
    [Header("CameraShake")]
    [SerializeField] float shakeForce;
    [SerializeField] Vector3 velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();

        playerCharacter.Initialize();
        playerCamera.Initialize(playerCharacter.GetCameraTarget());
    }

    private void OnDestroy()
    {
        _inputActions.Dispose();
    }
    // Update is called once per frame
    void Update()
    {
        var input = _inputActions.Player;
        var characterInput = new CharacterInput
        {
            Rotation = playerCamera._camera.transform.rotation,
            Move = input.Move.ReadValue<Vector2>(),
            Jump = input.Jump.WasPressedThisFrame(),
            Crouch = input.Crouch.WasPressedThisFrame() ? CrouchInput.Toggle : CrouchInput.None
        };
        playerCharacter.UpdateInput(characterInput);
        playerCharacter.UpdateBody();
        if (characterInput.Move != new Vector3(0,0,0))
        {
            CameraShake.cameraShakeInstance.Shake(shakeForce, velocity);
        }
        var cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
        playerCamera.UpdateRotation(cameraInput);

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Should Teleport");
            var ray = new Ray(playerCamera.transform.position, playerCamera._camera.transform.forward);
            if (Physics.Raycast(ray, out var hit))
            {
                Teleport(hit.point);
            }
        }
#endif


    }

    private void LateUpdate()
    {
        float deltaTime = Time.deltaTime;
        Transform cameraTarget = playerCharacter.GetCameraTarget();
        _characterState = playerCharacter.GetState();
        _lastCharacterState = playerCharacter.GetLastState();
        playerCamera.UpdatePosition(cameraTarget);

       

    }
    public void Teleport(Vector3 position)
    {
        playerCharacter.SetPosition(position);
    }

}
