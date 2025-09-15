using System.Collections;
using UnityEngine;

public enum CombatHand
{
    None,
    Left,
    Right

}

[System.Serializable]
public struct CombatState
{
    public CombatHand currentHand;
    public bool CanAttack;
}

public struct CombatInput
{
    public bool BaseAttack;
}

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]private Punch RigthArm;
    [SerializeField] private Punch LeftArm;
    public CombatState _state;

    public float timeBetweenAttacks;
    public float timeToDoublePunch;
    public float punchDuration;

    private float timeSinceLastPunch;
    private bool requestAttack;

    public void Initialize()
    {
        _state.currentHand = CombatHand.None;
        _state.CanAttack = true;
    }

    public void UpdateInput(CombatInput input)
    {
        requestAttack = input.BaseAttack;
        if (requestAttack) Debug.Log("Requested Attack");
    }

    public bool CheckIfCanAttack()
    {
        return _state.CanAttack;
    }


    public void CombatTickUpdate(float deltaTime)
    {
        if(Time.time - timeSinceLastPunch > timeToDoublePunch)
        {
            _state.currentHand = CombatHand.None;
        }

        if(requestAttack && CheckIfCanAttack())
        {
            Attack();
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

                RigthArm.ActivateOrDeactivePunch(true);
                StartCoroutine(DeactivePunch(RigthArm, punchDuration));
                break;

            case CombatHand.Right:
                _state.currentHand = CombatHand.Left;
                _state.CanAttack = false;
                StartCoroutine(ResetCanAttack(timeBetweenAttacks));
                timeSinceLastPunch = Time.time;
                RigthArm.ActivateOrDeactivePunch(true);
                StartCoroutine(DeactivePunch(RigthArm, punchDuration));

                break;

            case CombatHand.Left:
                _state.CanAttack = false;
                StartCoroutine(ResetCanAttack(timeBetweenAttacks));
                timeSinceLastPunch = Time.time;
                LeftArm.ActivateOrDeactivePunch(true);
                StartCoroutine(DeactivePunch(LeftArm, punchDuration));

                break;
        }
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
