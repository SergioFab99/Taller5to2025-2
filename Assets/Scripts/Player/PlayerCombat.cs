using System;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
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

public class PlayerCombat : CombatBase
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

    public LayerMask EnemyMask;

    
    public event WeaponPickUp OnWeaponPickUp;
    public delegate void WeaponPickUp(GameObject Prefab);


    #region
   
    private bool requestAttack;
    private bool requestGrab;
    private bool requestThrow;
    private bool requestInteract;
    private bool requestBlocking;
    private bool requestDodge;
    #endregion

    public event PunchSide OnAttack;
    public delegate void PunchSide(int hand);
    
    

    
    public Transform cam;


        
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
    
    private float _lastDodgeTime = 0f;
    [SerializeField] private float dodgeCooldown = 1.0f;
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
            Debug.Log(currentWeapon.Wtype);
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
        //cambiar el metodo de recivir el componente dependiendo de su lugar UWwU
        var recv = GetComponentInChildren<CombatHitReceiver>();
            if (recv != null) recv.isBlocking = false;
        
        if (requestDodge && !_isDodging)
        {
         //   Dodge();
        }

    }

    
    public void ReceiveDamage(float damage)
    {
        
        if (_isDodging)
            return;
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
        int number =3;
        if (_canCounter)
        {
            Debug.Log("Performing counter attack");
            
            currentWeapon.Attack();       
            _canCounter = false;
        
            
        }
        if(currentWeapon.Wtype == WeaponType.Fist)
        {
            if ((currentWeapon as FistsWeapon).currentHand == CombatHand.None || (currentWeapon as FistsWeapon).currentHand ==  CombatHand.Left)
            {
                number = 0;
            OnAttack?.Invoke(number);
            }
            else if((currentWeapon as FistsWeapon).currentHand == CombatHand.Right)
            {
                number = 1;
                OnAttack?.Invoke(number);
            }
           
                
        }
        else if (currentWeapon.Wtype == WeaponType.Bat)
        {
            number = 2;
            OnAttack?.Invoke(number);
        }
        currentWeapon.Attack();       

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

        // LO MISMO que en el otro. cambiuar dependiendo de su lugar UwU
        var recv = GetComponentInChildren<CombatHitReceiver>();
        if (recv != null) recv.isBlocking = true;
    }

    public void LinkWeapon(GameObject obj)
    {
        Debug.Log("LinkWeapon");
        var weapon = obj.GetComponent<Weapon>();
        if (weapon.Wtype == WeaponType.Fist)
        {
            weapon.gameObject.SetActive(true);
            var Oldweapon = currentWeapon.gameObject;
            UnLinkWeapon();
            currentWeapon = weapon;
            if (Oldweapon != null)
            {
                Destroy(Oldweapon);
            }
            currentWeapon = weapon;

            currentWeapon.Initialize(this);
        }
        else
        {
            UnLinkWeapon();
            var weaponObj = Instantiate(obj, weaponPos.transform);
            Debug.Log("InstantieWeapon");
            var weaponScript = weaponObj.GetComponent<Weapon>();
            currentWeapon = weaponScript;

            currentWeapon.Initialize(this);
        }

    }
    private Transform FindNearestTargetInFOV(float maxDist, float fovDegrees, LayerMask mask)
    {
        
        int layerMask = (mask.value == 0) ? ~0 : mask.value;

        Collider[] cols = Physics.OverlapSphere(transform.position, maxDist, layerMask, QueryTriggerInteraction.Ignore);

        if (cols == null || cols.Length == 0)
        {
            return null;
        }

        
        Vector3 forwardRef = cam != null ? cam.forward : transform.forward;

        Transform best = null;
        float bestDist = float.MaxValue;

        for (int i = 0; i < cols.Length; i++)
        {
            var c = cols[i];
            Vector3 dir = c.transform.position - transform.position;
            dir.y = 0f;
            float ang = dir.sqrMagnitude > 0.0001f ? Vector3.Angle(forwardRef, dir.normalized) : 0f;

            if (dir.sqrMagnitude < 0.01f) continue;
            if (ang <= fovDegrees * 0.5f)
            {
                float d = dir.sqrMagnitude;
                if (d < bestDist)
                {
                    bestDist = d;
                    best = c.transform;

                    
                }
            }
        }
        return best;
    }
    


    void Dodge()
    {

        if (playerCharacter == null)
        {
            Debug.LogWarning("Dodge: no PlayerCharacter assigned to PlayerCombat.");
            return;
        }
        if (Time.time - _lastDodgeTime < dodgeCooldown)
        {
            return;
        }


        float dodgeRange = DefaultBlockDodgeCounterSettings != null ? DefaultBlockDodgeCounterSettings.dodgeDistance : 3f;
        float fov = (DefaultBlockDodgeCounterSettings as DefaultBlockDodgeCounterSettings) != null && false ? 90f : 120f;
        Transform target = FindNearestTargetInFOV(dodgeRange, fov, EnemyMask);

        if (target == null)
        {
            return;
        }

        
    float durationDbg = DefaultBlockDodgeCounterSettings != null ? DefaultBlockDodgeCounterSettings.dodgeDuration : 0.2f;
    float dodgeDistanceDbg = DefaultBlockDodgeCounterSettings != null ? DefaultBlockDodgeCounterSettings.dodgeDistance : 3f;
        Vector3 tgtPos = target.position;
        float tgtDist = Vector3.Distance(playerCharacter.transform.position, tgtPos);
        Vector3 dirToTgt = (tgtPos - playerCharacter.transform.position);
        dirToTgt.y = 0f;
        Vector3 forwardRef = cam != null ? cam.forward : transform.forward;
        float tgtAngle = dirToTgt.sqrMagnitude > 0.0001f ? Vector3.Angle(forwardRef, dirToTgt.normalized) : 0f;
        float tgtSigned = dirToTgt.sqrMagnitude > 0.0001f ? Vector3.SignedAngle(forwardRef, dirToTgt.normalized, Vector3.up) : 0f;
    

        _state.isBlocking = false;
        if (_state.playerActionState == PlayerActionState.Blocking)
            _state.playerActionState = PlayerActionState.Normal;


        float duration = DefaultBlockDodgeCounterSettings != null ? DefaultBlockDodgeCounterSettings.dodgeDuration : 0.2f;
        float range = dodgeRange;
        _isDodging = true;
        _lastDodgeTime = Time.time;
        if (playerCamera != null)
        {
            playerCamera.SetLookLocked(true);
            playerCamera.SetLookLockTarget(target);
        }

        Vector3 lookDir = target.position - playerCharacter.transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            playerCharacter.transform.rotation = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
        }

        
        int directionOverride = 0;
        if (Mathf.Abs(_moveInput.x) > 0.15f)
        {
            directionOverride = _moveInput.x > 0f ? 1 : -1;
        }

        playerCharacter.StartCoroutine(playerCharacter.PerformArcMoveCoroutine(target, range, duration, playerCamera, () =>
        {
            if (playerCamera != null)
            {
                playerCamera.SetLookLocked(false);
                playerCamera.SetLookLockTarget(null);
            }
            _isDodging = false;
        }, directionOverride));
        
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
