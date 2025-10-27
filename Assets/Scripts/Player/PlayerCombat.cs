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

    public bool Blocking;
}

public class PlayerCombat : MonoBehaviour
{
    public GameObject weaponPos;
    public GameObject rightPunchPos;
    public GameObject leftPunchPos;

    public CombatState _state;

    public Weapon currentWeapon;


    public LayerMask hitMask;
    


    #region
    private float timeSinceLastPunch;
    private bool requestAttack;
    private bool requestGrab;
    private bool requestThrow;
    private bool requestInteract;
    private bool requestBlocking;
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
    private PlayerPickUp PlayerPickUp;


    public void Initialize(PlayerPickUp playerPickUp)
    {
        _state.CanAttack = true;
        _state.CanGrabOrThrow = true;   
        _state.objectInteractionState = ObjectInteractionState.NotHoldingObject;
        if (currentWeapon != null) currentWeapon.Initialize(this);
        PlayerPickUp = playerPickUp;
        PlayerPickUp.OnWeaponPickUp += LinkWeapon;
    }
    public void OnDisable()
    {
        PlayerPickUp.OnWeaponPickUp -= LinkWeapon;
    }
    public void UpdateInput(CombatInput input)
    {
        requestAttack = input.BaseAttack;
        requestInteract = input.Interact;
        requestBlocking = input.Blocking;

        if (requestAttack) Debug.Log("Requested Attack");
        if (requestInteract) Debug.Log("Requested Interact");
        if (requestBlocking) Debug.Log("Requested Blocking");
    }

    //Eliminar cuando holdpoint este bien
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
            if (CheckIfCanGraborThrow())
            {
                GrabNThrow();
            }
            else
            {
                Debug.Log("Cantgraborthrow");
            }

        }
        while (requestBlocking&& !_state.isBlocking)
        {
            Block();
        }
        if (!requestBlocking && _state.isBlocking)
        {
            _state.isBlocking = false;
            if (_state.playerActionState == PlayerActionState.Blocking)
                _state.playerActionState = PlayerActionState.Normal;
        }


    }


    void Attack()
    {
        Debug.Log("Attacking");
        currentWeapon?.Attack();
    }

    void GrabNThrow()
    {
        Ray ray = new Ray(cam.position, cam.forward);
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
        }
    }

    void Block()
    {
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

}
