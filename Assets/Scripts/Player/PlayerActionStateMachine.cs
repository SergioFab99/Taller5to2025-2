using UnityEngine;

public enum PlayerActionState
{
    Normal,
    Blocking,
    Dodging,
    CounterAttack,
    
}

public class PlayerActionStateMachine : MonoBehaviour
{
    [Header("Dodge Settings")]
    public float dodgeDistance = 3f;
    public float dodgeDuration = 0.2f;
    private bool isDodging = false;
    private Vector3 dodgeDirection;
    private float dodgeTimer = 0f;

    [Header("Block Settings")]
    public float blockAngle = 120f;
    public float blockDamageMultiplier = 0.5f;

    [Header("Counter Settings")]
    public float counterWindow = 0.5f;
    private bool canCounter = false;
    private float counterTimer = 0f;

    public PlayerActionState CurrentState { get; private set; } = PlayerActionState.Normal;

    private void Update()
    {
        // Dodge logic
        if (isDodging)
        {
            dodgeTimer += Time.deltaTime;
            float t = dodgeTimer / dodgeDuration;
            if (t < 1f)
            {
                transform.position += dodgeDirection * (dodgeDistance / dodgeDuration) * Time.deltaTime;
            }
            else
            {
                isDodging = false;
                dodgeTimer = 0f;
                SetState(PlayerActionState.Blocking);
            }
        }

        
        if (canCounter)
        {
            counterTimer += Time.deltaTime;
            if (counterTimer > counterWindow)
            {
                canCounter = false;
                counterTimer = 0f;
                if (CurrentState == PlayerActionState.CounterAttack)
                    SetState(PlayerActionState.Normal);
            }
        }
    }

    public void StartDodge(Vector3 direction)
    {
        if (CurrentState == PlayerActionState.Blocking && !isDodging)
        {
            dodgeDirection = direction.normalized;
            isDodging = true;
            dodgeTimer = 0f;
            SetState(PlayerActionState.Dodging);
            Debug.Log($"Dodge started in direction: {dodgeDirection}");
        }
    }

    public void SetBlockInput(bool isBlocking)
    {
        if (isBlocking)
        {
            if (CurrentState != PlayerActionState.Blocking)
            {
                SetState(PlayerActionState.Blocking);
                Debug.Log("Blocking Input Detected");
            }
        }
        else
        {
            if (CurrentState == PlayerActionState.Blocking)
            {
                SetState(PlayerActionState.Normal);
                Debug.Log("Blocking Input STOP Detected");
            }
        }
    }

    public void SetState(PlayerActionState newState)
    {
        CurrentState = newState;
    }

    public float CalculateDamage(Vector3 attackerPosition, bool isProjectile, float baseDamage)
    {
        if (CurrentState == PlayerActionState.Blocking && !isProjectile)
        {
            Vector3 toAttacker = (attackerPosition - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, toAttacker);
            if (angle < blockAngle * 0.5f)
            {
                
                return baseDamage * blockDamageMultiplier;
            }
        }
        return baseDamage;
    }

    
    public void OnAttackDodged()
    {
        canCounter = true;
        counterTimer = 0f;
        Debug.Log("¡Ventana de contraataque abierta!");
    }

    
    public void TryCounterAttack()
    {
        if (canCounter)
        {
            SetState(PlayerActionState.CounterAttack);
            canCounter = false;
            counterTimer = 0f;
            Debug.Log("¡Contraataque ejecutado!");
            
        }
    }
}