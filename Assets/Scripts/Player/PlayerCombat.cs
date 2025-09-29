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

[System.Serializable]
public struct CombatState
{
    public ObjectInteractionState objectInteractionState;
    public CombatHand currentHand;
    public bool CanAttack;
    public bool CanGrabOrThrow;
}


public struct CombatInput
{
    public bool BaseAttack;
    public bool Interact;
}

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]private Punch RigthArm;
    [SerializeField]private Punch LeftArm;
    public CombatState _state;

    public float timeBetweenAttacks;
    public float timeToDoublePunch;
    public float punchDuration;

    private float timeSinceLastPunch;
    private bool requestAttack;
    private bool requestGrab;
    private bool requestThrow;
    private bool requestInteract;


    public event PunchSide OnAttack;
    public delegate void PunchSide(int hand); 

    [SerializeReference]
    [FoldoutGroup("DefaultGrab&ThrowSettings")]
    public DefaultGrabThrowSettings DefaultGrabThrowSettings;
    public Transform cam;
    public Transform HoldPoint;

    public Transform holdpoint2;
    [SerializeField] private GameObject _heldObject;
    

    public void Initialize()
    {
        _state.currentHand = CombatHand.None;
        _state.CanAttack = true;
        _state.CanGrabOrThrow = true;   
        _state.objectInteractionState = ObjectInteractionState.NotHoldingObject;
    }

    public void UpdateInput(CombatInput input)
    {
        requestAttack = input.BaseAttack;        
        requestInteract = input.Interact;

        if (requestAttack) Debug.Log("Requested Attack");
        if (requestInteract) Debug.Log("Requested Interact");
    }

    public void Update()
    {
        HoldPoint.position = holdpoint2.position;
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
        if (Time.time - timeSinceLastPunch > timeToDoublePunch)
        {
            _state.currentHand = CombatHand.None;
        }

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


    }


    void Attack()
    {
        Debug.Log("Attackig");
        switch(_state.currentHand)
        {
            case CombatHand.None:
                _state.currentHand = CombatHand.Right;
                _state.CanAttack = false;
                StartCoroutine(ResetCanAttack(timeBetweenAttacks));
                timeSinceLastPunch = Time.time;

                OnAttack?.Invoke(1);

                RigthArm.ActivateOrDeactivePunch(true);
                StartCoroutine(DeactivePunch(RigthArm, punchDuration));
                break;

            case CombatHand.Right:
                _state.currentHand = CombatHand.Left;
                _state.CanAttack = false;
                StartCoroutine(ResetCanAttack(timeBetweenAttacks));
                timeSinceLastPunch = Time.time;

                OnAttack?.Invoke(2);
                RigthArm.ActivateOrDeactivePunch(true);
                StartCoroutine(DeactivePunch(RigthArm, punchDuration));
                break;

            case CombatHand.Left:
                _state.currentHand = CombatHand.Right;
                _state.CanAttack = false;
                StartCoroutine(ResetCanAttack(timeBetweenAttacks));
                timeSinceLastPunch = Time.time;

                OnAttack?.Invoke(1);
                LeftArm.ActivateOrDeactivePunch(true);
                StartCoroutine(DeactivePunch(LeftArm, punchDuration));

                break;
        }
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

                _heldObject.transform.SetParent(HoldPoint);
                _heldObject.transform.localPosition = Vector3.zero;
                _heldObject.transform.localRotation = Quaternion.identity;

                var rb = _heldObject.GetComponent<Rigidbody>();
                if (rb != null) { rb.isKinematic = true; rb.linearVelocity = Vector3.zero; }
                _state.objectInteractionState = ObjectInteractionState.HoldingObject;

            }
        }
        else if (_heldObject != null && _state.objectInteractionState == ObjectInteractionState.HoldingObject)
        {            
            var rb = _heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                _heldObject.transform.SetParent(null);
                rb.AddForce(cam.forward * DefaultGrabThrowSettings.throwForce, ForceMode.Impulse);
            }
            _heldObject = null;
            _state.objectInteractionState = ObjectInteractionState.NotHoldingObject;
        }
    }

    void Throw()
    {
        Debug.Log("Throwing");  
    }


    IEnumerator ResetCanAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        _state.CanAttack = true;
    }

    IEnumerator DeactivePunch(Punch punch,float duration)
    {
        yield return new WaitForSeconds(duration);
        punch.ActivateOrDeactivePunch(false);
    }

}
