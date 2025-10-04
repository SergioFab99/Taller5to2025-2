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
    public CombatHand currentHand;
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
    private bool requestBlocking;


    public event PunchSide OnAttack;
    public delegate void PunchSide(int hand); 

    [SerializeReference]
    [FoldoutGroup("DefaultGrab&ThrowSettings")]
    public DefaultGrabThrowSettings DefaultGrabThrowSettings;
    public Transform cam;
    public Transform HoldPoint;

    public Transform holdpoint2;//solucion xd revisar luego
    [SerializeField] private GameObject _heldObject;
    [Header("Hold Point Orientation Settings")] 
    [Tooltip("If true the HoldPoint will copy the camera rotation each frame.")]
    [SerializeField] private bool alignHoldPointWithCamera = true;
    [Tooltip("If true the HoldPoint will instead align with the player root (this transform) ignoring camera yaw/pitch.")]
    [SerializeField] private bool alignWithPlayerRoot = false;
    [Tooltip("Euler offset applied AFTER alignment (use to tweak weapon twist).")]
    [SerializeField] private Vector3 holdPointRotationOffset = Vector3.zero;
    [Tooltip("Slerp factor (0 = snap, <1 = smooth) for rotation alignment.")]
    [Range(0f,1f)] [SerializeField] private float holdPointRotateSmoothing = 0f;
    

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
        requestBlocking = input.Blocking;

        if (requestAttack) Debug.Log("Requested Attack");
        if (requestInteract) Debug.Log("Requested Interact");
        if (requestBlocking) Debug.Log("Requested Blocking");
    }

    //Eliminar cuando holdpoint este bien
    public void Update()
    {
        if (HoldPoint != null && holdpoint2 != null)
        {
            HoldPoint.position = holdpoint2.position;

            // Determine target rotation source
            Quaternion targetRot = HoldPoint.rotation;
            if (alignHoldPointWithCamera && cam != null)
            {
                targetRot = cam.rotation;
            }
            else if (alignWithPlayerRoot)
            {
                // Use player facing (ignore pitch) so vertical look doesn't tilt weapon
                Vector3 fwd = transform.forward; fwd.y = 0f; if (fwd.sqrMagnitude < 0.0001f) fwd = Vector3.forward; fwd.Normalize();
                targetRot = Quaternion.LookRotation(fwd, Vector3.up);
            }

            if (holdPointRotationOffset != Vector3.zero)
            {
                targetRot *= Quaternion.Euler(holdPointRotationOffset);
            }

            if (holdPointRotateSmoothing > 0f)
            {
                HoldPoint.rotation = Quaternion.Slerp(HoldPoint.rotation, targetRot, 1f - Mathf.Pow(1f - holdPointRotateSmoothing, Time.deltaTime * 60f));
            }
            else
            {
                HoldPoint.rotation = targetRot;
            }
        }
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
        // If holding an object, use bat logic or generic logic
        if (_heldObject != null)
        {
            var bat = _heldObject.GetComponent<Bat>();
            if (bat != null)
            {
                bat.Hit(cam, cam.forward, 2.5f); // Example range, adjust as needed
                // Bat handles its own durability, do not call Use() here
                if (_state.currentHand == CombatHand.None || _state.currentHand == CombatHand.Left)
                {
                    Transform reference = bat.HoldPoint != null && bat.HoldPoint.parent != null ? bat.HoldPoint.parent : transform;
                    bat.PlaySwing(reference);
                }
            }
            else
            {
                var grabbable = _heldObject.GetComponent<GrabbableObject>();
                if (grabbable != null)
                {
                    if (grabbable.IsBroken)
                    {
                        Debug.Log("The item is broken! Cannot attack.");
                        return;
                    }
                    else
                    {
                        grabbable.Use();
                    }
                }
            }
        }
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

                // If it's a bat, parent to HoldPoint and set local position/rotation to inverse of HoldPoint offset
                var bat = _heldObject.GetComponent<Bat>();
                if (bat != null && bat.HoldPoint != null)
                {
                    Transform batTransform = bat.transform;
                    Transform batHoldPoint = bat.HoldPoint;
                    batTransform.SetParent(HoldPoint);
                    // Set local position/rotation so batHoldPoint aligns with HoldPoint origin
                    batTransform.localPosition = -batHoldPoint.localPosition;
                    batTransform.localRotation = Quaternion.Inverse(batHoldPoint.localRotation);
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

    void Block()
    {
        _state.isBlocking = true;
        _state.playerActionState = PlayerActionState.Blocking;
        Debug.Log("Blocking");
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

class Attack
{

}
