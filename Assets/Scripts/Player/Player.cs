using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerCharacter playerCharacter;
    [SerializeField] PlayerCamera playerCamera;
    [SerializeField] PlayerActionStateMachine actionStateMachine; 
    PlayerInputActions _inputActions;

    [SerializeField] CharacterState _characterState;
    [SerializeField] CharacterState _lastCharacterState;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();

    playerCharacter.Initialize(playerCamera._camera.transform);
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
            Crouch = input.Crouch.WasPressedThisFrame() ? CrouchInput.Toggle : CrouchInput.None,
            Grab = input.Grab.WasPressedThisFrame(),
            Throw = input.Throw.WasPressedThisFrame(),
            Dash = input.Dash.WasPressedThisFrame(),
        };
        playerCharacter.UpdateInput(characterInput);
        playerCharacter.UpdateBody();

        // --- BLOQUEO --- //
        if (actionStateMachine != null)
        {
        bool isBlocking = input.Block.IsPressed();
        actionStateMachine.SetBlockInput(isBlocking);

            // --- DODGE --- //
            if (actionStateMachine.CurrentState == PlayerActionState.Blocking && !Mathf.Approximately(characterInput.Move.sqrMagnitude, 0f))
            {
                Vector3 moveDir = new Vector3(characterInput.Move.x, 0, characterInput.Move.y);
                moveDir = playerCamera._camera.transform.TransformDirection(moveDir);
                moveDir.y = 0f;
                if (moveDir.sqrMagnitude > 0.01f)
                {
                    actionStateMachine.StartDodge(moveDir.normalized);
                }
            }
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
