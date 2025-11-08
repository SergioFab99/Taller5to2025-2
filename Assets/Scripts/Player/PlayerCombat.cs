using System;
using System.Collections;
using UnityEngine;

public enum CombatHand
{
    None,
    Left,
    Right

}
public enum ObjectInteractionState
{
    HoldingObject,
    NotHoldingObject,
}

public enum  PlayerBlockState
{
    Normal,
    Blocking,
    Dodging,
    CounterAttack,    
}

[System.Serializable]
public struct CombatState
{
    public PlayerActionState playerActionState;
    public ObjectInteractionState objectInteractionState;
    public PlayerBlockState BlockState;
    public bool CanAttack;
    public bool CanGrabOrThrow;
    public bool isBlocking;
}


public struct CombatInput
{
    public bool BaseAttack;
    public bool Interact;
    
    public bool Dodge;

    public bool Blocking;
}

public class PlayerCombat : MonoBehaviour
{
    [FoldoutGroup("Positions")]
    public GameObject weaponPos;

    [FoldoutGroup("Positions")]
    public GameObject rightPunchPos;

    [FoldoutGroup("Positions")]
    public GameObject leftPunchPos;

    public GameObject hitPoint;



    public CombatState _state;

    public GameObject fistWeapon;

    public Weapon currentWeapon;


    public LayerMask hitMask;
    public event WeaponPickUp OnWeaponPickUp;
    public delegate void WeaponPickUp(GameObject Prefab);


    #region
    private float timeSinceLastPunch;
    private bool requestAttack;
    private bool requestGrab;
    private bool requestThrow;
    private bool requestInteract;
    private bool requestBlocking;
    private bool requestDodge;
    #endregion

    public event PunchSide OnAttack;
    public delegate void PunchSide(int hand);
    
    

    [SerializeReference]
    [FoldoutGroup("DefaultGrab&ThrowSettings")]
    public DefaultGrabThrowSettings DefaultGrabThrowSettings;
    public Transform cam;
    public Transform HoldPoint;

    public Transform holdpoint2;//solucion xd revisar luego
    [SerializeField] public GameObject _heldObject;

        
    public DefaultBlockDodgeCounterSettings DefaultBlockDodgeCounterSettings;


    public void Initialize()
    {
        _state.CanAttack = true;
        _state.CanGrabOrThrow = true;   
        _state.objectInteractionState = ObjectInteractionState.NotHoldingObject;
        if (currentWeapon != null) currentWeapon.Initialize(this);

       
    }
  

    
    private Vector2 _moveInput = Vector2.zero;
    public void SetMoveInput(Vector2 move) => _moveInput = move;

    // References filled by Player.Start
    [HideInInspector] public PlayerCharacter playerCharacter;
     public PlayerCamera playerCamera;
    // Dodge state
    private bool _isDodging = false;
    // Counter window
    private bool _canCounter = false;

    public void UpdateInput(CombatInput input)
    {
        requestAttack = input.BaseAttack;
        requestInteract = input.Interact;
        requestBlocking = input.Blocking;
        requestDodge = input.Dodge;

        if (requestAttack) Debug.Log("Requested Attack");
        if (requestInteract) Debug.Log("Requested Interact");
        if (requestBlocking) Debug.Log("Requested Blocking");
        if (requestDodge) Debug.Log("Requested Dodge");
    }

    
    public void Update()
    {
       
        
    }

    public bool CheckIfCanAttack()
    {
        return _state.CanAttack;
    }
    
     public bool CheckIfCanGraborThrow()
    {
        return _state.CanGrabOrThrow;
    }


    public void CombatTickUpdate(float deltaTime)
    {

        currentWeapon?.CombatTickUpdate(deltaTime);
        if (requestAttack && CheckIfCanAttack())
        {
            Attack();
        }
        if (requestInteract)
        {
            if (currentWeapon.Wtype == WeaponType.Fist)
            {
                Debug.Log("PickingUp Object");
                PickUpWeapon();
                return;
            }

           if(currentWeapon != null && currentWeapon.Wtype != WeaponType.Fist && currentWeapon.tagContainer.HasTag("Throwable"))
            {
                Debug.Log("RequestThrow");

                currentWeapon.Throw(cam.forward);
                LinkWeapon(fistWeapon);
            }

        }
        
        
        // Handle block input, but do not allow starting block while dodging
        if (requestBlocking && !_state.isBlocking && !_isDodging)
        {
            Block();
        }
        else if (!requestBlocking && _state.isBlocking)
        {
            _state.isBlocking = false;
            if (_state.playerActionState == PlayerActionState.Blocking)
                _state.playerActionState = PlayerActionState.Normal;
        }
        
        if (requestDodge && _state.isBlocking)
        {
            Dodge();
        }

    }

    
    public void ReceiveDamage(float damage)
    {
        float finalDamage = damage;
        if (_state.playerActionState == PlayerActionState.Blocking)
        {
            finalDamage = damage * 0.5f;             
        }

        var hc = GetComponent<HealthController>() ?? GetComponentInChildren<HealthController>() ?? GetComponentInParent<HealthController>();
        if (hc != null)
        {
            hc.TakeDamague(finalDamage);
        }
        else
        {
            Debug.LogWarning("ReceiveDamage: No HealthController found on player to apply damage.");
        }
    }


    void Attack()
    {
       

        
        if (_canCounter)
        {
            Debug.Log("Performing counter attack");
            
            currentWeapon.Attack();       
            _canCounter = false;
            return;
        }

        
        currentWeapon.Attack();       
    }

    void GrabNThrow()
    {
        /*Ray ray = new Ray(cam.position, cam.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, DefaultGrabThrowSettings.grabRange)&& _heldObject == null)
        {
            var grabbable = hit.collider.GetComponent<GrabbableObject>();
            if (grabbable != null)
            {
                _heldObject = grabbable.gameObject;

                // If it's a bat, parent to HoldPoint and set local position/rotation to inverse of HoldPoint offset
                var bat = _heldObject.GetComponent<Bat>();
                if (bat != null && bat.HoldPoint != null)
                {
                    Transform batTransform = bat.transform;
                    Transform batHoldPoint = bat.HoldPoint;
                    batTransform.SetParent(HoldPoint);
                    // Set local position/rotation so batHoldPoint aligns with HoldPoint origin
                    batTransform.localPosition = -batHoldPoint.localPosition;
                    // Alinear el bate: su eje Z apunta en la dirección del eje X del HoldPoint (hacia adelante)
                    batTransform.rotation = Quaternion.LookRotation(HoldPoint.right, HoldPoint.up);
                }
                else
                {
                    _heldObject.transform.SetParent(HoldPoint);
                    _heldObject.transform.localPosition = Vector3.zero;
                    _heldObject.transform.localRotation = Quaternion.identity;
                }

                var rb = _heldObject.GetComponent<Rigidbody>();
                if (rb != null) { rb.isKinematic = true; rb.linearVelocity = Vector3.zero; }
                _state.objectInteractionState = ObjectInteractionState.HoldingObject;

                // Llama evento de agarre
                grabbable.OnGrabbed();
            }
        }
        else if (_heldObject != null && _state.objectInteractionState == ObjectInteractionState.HoldingObject)
        {            
            var grabbable = _heldObject.GetComponent<GrabbableObject>();
            var rb = _heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                _heldObject.transform.SetParent(null);
                rb.AddForce(cam.forward * DefaultGrabThrowSettings.throwForce, ForceMode.Impulse);
            }
            // Llama evento de soltar
            if (grabbable != null) grabbable.OnReleased();
            _heldObject = null;
            _state.objectInteractionState = ObjectInteractionState.NotHoldingObject;
        }*/
    }

    void Block()
    {
        if (_isDodging)
        {
            Debug.Log("Block attempt ignored while dodging");
            return;
        }
        _state.isBlocking = true;
        _state.playerActionState = PlayerActionState.Blocking;
        Debug.Log("Blocking");
    }

    public void LinkWeapon(GameObject obj)
    {
        Debug.Log("LinkWeapon");
        var weapon = obj.GetComponent<Weapon>();
        if(weapon.Wtype == WeaponType.Fist)
        {
            weapon.gameObject.SetActive(true);
            var Oldweapon = currentWeapon.gameObject;
            UnLinkWeapon();
            currentWeapon = weapon;
            if(Oldweapon!= null)
            {
                Destroy(Oldweapon);
            }
            currentWeapon = weapon;
            
            currentWeapon.Initialize(this);
        }
        else
        {
            UnLinkWeapon();
            var weaponObj = Instantiate(obj,weaponPos.transform);
            Debug.Log("InstantieWeapon");
            var weaponScript = weaponObj.GetComponent<Weapon>();
            currentWeapon = weaponScript;

            currentWeapon.Initialize(this);
        }

    }
    void Dodge()
    {
        Debug.Log("Dodging");
       
        if (playerCharacter == null)
        {
            Debug.LogWarning("Dodge: no PlayerCharacter assigned to PlayerCombat.");
            return;
        }

        
        _state.isBlocking = false;
        if (_state.playerActionState == PlayerActionState.Blocking)
            _state.playerActionState = PlayerActionState.Normal;

        
        if (_moveInput.sqrMagnitude <= 0.01f)
        {
            Debug.Log("Dodge cancelled: no input direction.");
            return;
        }

        
        Vector3 dirWorld = Vector3.zero;
        if (_moveInput.sqrMagnitude > 0.001f)
        {            
            if (playerCamera != null && playerCamera._camera != null)
            {
                var camRot = playerCamera._camera.transform.rotation;
                dirWorld = (camRot * new Vector3(_moveInput.x, 0f, _moveInput.y));
            }
            else
            {
                dirWorld = (transform.right * _moveInput.x + transform.forward * _moveInput.y);
            }
        }
        else
        {            
            dirWorld = transform.right;
        }
        dirWorld.y = 0f;
        if (dirWorld.sqrMagnitude < 0.001f) dirWorld = transform.right;
    dirWorld.Normalize();

    
    _isDodging = true;

        
        float distance = DefaultBlockDodgeCounterSettings.dodgeDistance;
        float duration = DefaultBlockDodgeCounterSettings.dodgeDuration;
        float speed = distance / duration;

        Vector3 impulse = dirWorld * speed;

        
        playerCharacter.AddExternalForce(impulse);

        
        if (playerCamera != null)
        {
            playerCamera.SetLookLocked(true);
            StartCoroutine(EndDodgeAfter(duration));
        }
        else
        {
            StartCoroutine(EndDodgeAfter(duration));
        }
        
        float counterWindow = DefaultBlockDodgeCounterSettings != null ? DefaultBlockDodgeCounterSettings.counterWindow : 0.5f;
        StartCoroutine(OpenCounterWindow(counterWindow));
    }

    private IEnumerator EndDodgeAfter(float duration)
    {
        _state.playerActionState = PlayerActionState.Normal;
        yield return new WaitForSeconds(duration);
        _isDodging = false;
        if (playerCamera != null) playerCamera.SetLookLocked(false);
    }

    private IEnumerator OpenCounterWindow(float window)
    {
        _canCounter = true;
        yield return new WaitForSeconds(window);
        _canCounter = false;
    }

    


    public void UnLinkWeapon()
    {
        if (currentWeapon.Wtype == WeaponType.Fist)
        {
            currentWeapon.gameObject.SetActive(false);
            currentWeapon = null;
        }
        else
        {
            currentWeapon = null;      
        }
    }

    public IEnumerator ResetCanAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        _state.CanAttack = true;
    }

    public IEnumerator DeactivePunch(Punch punch,float duration)
{
    yield return new WaitForSeconds(duration);
    punch.ActivateOrDeactivePunch(false);
}


    public void PickUpWeapon()
    {
        var col = Physics.OverlapSphere(weaponPos.transform.position, 2f, hitMask.value, QueryTriggerInteraction.Ignore);
        if (col != null && col.Length > 0)
        {
            Debug.Log("EnterPickUp");
            foreach (Collider coll in col)
            {
                if (coll.gameObject.TryGetComponent<TagContainer>(out TagContainer TagC) && TagC.HasTag("PickUpWeapon"))
                {

                    Debug.Log("HasPickUpTag");
                    LinkWeapon(coll.gameObject.GetComponent<PickUpWeapon>().Prefab);
                    Destroy(coll.gameObject);

                }
            }
        }
    }
}
