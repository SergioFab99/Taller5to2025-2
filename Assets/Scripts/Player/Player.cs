using Unity.Cinemachine;

using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerCharacter playerCharacter;
    [SerializeField] PlayerCamera playerCamera;

    [SerializeField] CharacterTarget CharacterCameraTarget;

    [SerializeField] CameraSpring cameraSpring;

    [SerializeField] PlayerCombat playerCombat;
    [SerializeField] PlayerAnimation playerAnimation;
    [SerializeField] PlayerAudio playerAudio;

    [SerializeField] HealthController healthController;



    [Header("Damage Reception")]
    [Tooltip("Damage taken when colliding/triggering with an enemy tagged 'Enemy'.")]
    [SerializeField] private float contactDamage = 1f;
     
    PlayerInputActions _inputActions;

    [SerializeField] CharacterState _characterState;
    [SerializeField] CharacterState _lastCharacterState;
    [Header("CameraShake")]
    [SerializeField] float shakeForce;
    [SerializeField] Vector3 velocity;

    [SerializeField] string thisScene;

    public void OnDead()
    {
        thisScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(thisScene);
    }

    private void OnDisable()
    {
        healthController.OnDead -= OnDead;
    }

    
    void Start()
    {
        healthController.OnDead += OnDead;
        healthController.OnLifeChangue += OnHealthChanged;

        Cursor.lockState = CursorLockMode.Locked;
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();

        playerCharacter.Initialize(playerCamera._camera.transform);
        playerCamera.Initialize(playerCharacter.GetCameraTarget());
        CharacterCameraTarget.Initialize(playerCamera._camera.transform);

        playerCombat.Initialize();
   
    
    playerCombat.playerCharacter = playerCharacter;
    playerCombat.playerCamera = playerCamera;
        playerAnimation.Initialize(playerCombat);

     
    }

    private void OnDestroy()
    {
        _inputActions.Dispose();
    }
    void Update()
        
    {
        float deltaTime = Time.deltaTime;
        var input = _inputActions.Player;
        var characterInput = new CharacterInput
        {
            Rotation = playerCamera._camera.transform.rotation,
            Move = input.Move.ReadValue<Vector2>(),
            Jump = input.Jump.WasPressedThisFrame(),      
        };
        
        playerCombat.SetMoveInput(characterInput.Move);
        playerCharacter.UpdateInput(characterInput);
        playerCharacter.UpdateBody();
 
        
        var cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
        playerCamera.UpdateRotation(cameraInput);

        
        var combatInput = new CombatInput
        {
            BaseAttack = input.Attack.WasPressedThisFrame(),
            Interact = input.Interact.WasPressedThisFrame(),
            Blocking = input.Block.IsPressed(),
            Dodge = input.Dash.WasPressedThisFrame()
        };
        playerCombat.UpdateInput(combatInput);
        playerCombat.CombatTickUpdate(Time.deltaTime);
        playerCharacter.setState(combatInput.Blocking);

        if (healthController.health <= healthController.maxHealth / 4)
            playerAudio.SetLowHP(true);
        else playerAudio.SetLowHP(false);

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

        CharacterCameraTarget.UpdateRotation(playerCamera.transform);

        cameraSpring.UpdateSpring(playerCamera.transform,deltaTime);



    }
    public void Teleport(Vector3 position)
    {
        playerCharacter.SetPosition(position);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Enemy"))
        {
            ApplyContactDamage();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            ApplyContactDamage();
        }
    }

    private void ApplyContactDamage()
    {
        if (healthController != null && contactDamage > 0f)
        {
            healthController.TakeDamague(contactDamage);
        }
    }
    private void OnHealthChanged(float delta)
    {
        if (playerAudio == null) return;

        if (delta < 0f)
        {
            playerAudio.PlayDamage();
        }
    }

}
