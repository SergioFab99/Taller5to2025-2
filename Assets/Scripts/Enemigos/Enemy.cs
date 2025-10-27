
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public struct EnemyInput
{
    public Vector3 Direction;
    public Vector3 Move;
    public CrouchInput Crouch;
    public bool Jump;
}


public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyCharacter character;
    //[SerializeField] EnemyCharacterState _characterState;
    //[SerializeField] EnemyCharacterState _lastCharacterState;
    [SerializeField] HealthController healthController;
    [SerializeField] EnemyAnimations _animations;
    [SerializeField] EnemyStateHandler _stateHandler;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] EnemySettingsList EnemySettings;
    //[SerializeField] CombatManager



    public Transform testTarget;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _stateHandler.Initialize(EnemySettings, character.transform, character, agent);

        character.Initialize(EnemySettings, _stateHandler.GetBehaviourState(), _stateHandler.GetCurrentState());

        healthController.OnDead += Ondead;

        _animations.Initialize();
    }

    public void Ondead()
    {
        healthController.OnDead -= Ondead;
        Destroy(gameObject);
    }
    public void Update()
    {
        _stateHandler.CurrentStateUpdate();
    }
    void FixedUpdate()
    {
        //character.UpdateState(_stateHandler.GetCurrentState());
        //var enemyInput = new EnemyInput();

        //switch (_stateHandler.GetBehaviourState())
        //{
        //    case EnemyBehaviourState.Default:
        //        {
        //            float distance = Vector3.Distance(GetTarget(), character.transform.position);
        //            Vector3 direction = (GetTarget() - character.transform.position).normalized;

        //            if (distance > EnemySettings.AISettings.stopingDistance)
        //            {
        //                enemyInput = new EnemyInput
        //                {
        //                    Direction = direction,
        //                    Move = direction
        //                };
        //            }
        //            else
        //            {
        //                enemyInput = new EnemyInput
        //                {
        //                    Direction = direction,
        //                    Move = Vector3.zero
        //                };
        //            }

        //            character.UpdateInputs(enemyInput, _stateHandler.GetBehaviourState());
        //            break;
        //        }

        //    case EnemyBehaviourState.Combat:
        //        {
        //            float distance = Vector3.Distance(GetTarget(), character.transform.position);
        //            Vector3 direction = (GetTarget() - character.transform.position).normalized;
        //            if (distance > EnemySettings.AISettings.stopingDistance)
        //            {
        //                enemyInput = new EnemyInput
        //                {
        //                    Direction = direction,
        //                    Move = direction
        //                };
        //            }
        //            else
        //            {
        //                enemyInput = new EnemyInput
        //                {
        //                    Direction = direction,
        //                    Move = Vector3.zero
        //                };
        //            }

        //            character.UpdateInputs(enemyInput, _stateHandler.GetBehaviourState());
        //            break;
        //        }

        //    case EnemyBehaviourState.Dead:
        //        enemyInput = new EnemyInput
        //        {
        //            Direction = Vector3.zero,
        //            Move = Vector3.zero
        //        };
        //        character.UpdateInputs(enemyInput, _stateHandler.GetBehaviourState());
        //        break;
        //}

        //_animations.AnimUpdate(Time.fixedDeltaTime, character);

        character.UpdateState(_stateHandler.GetCurrentState());

        var enemyInput = new EnemyInput();
        EnemyBehaviourState behaviour = _stateHandler.GetBehaviourState();

        Vector3 destination = GetTarget(behaviour);
        Vector3 direction = (destination - character.transform.position).normalized;
        float distance = Vector3.Distance(destination, character.transform.position);

        Vector3 moveVector = (distance > EnemySettings.AISettings.stopingDistance) ? direction : Vector3.zero;

        enemyInput = new EnemyInput
        {
            Direction = direction,
            Move = moveVector
        };

        character.UpdateInputs(enemyInput, behaviour);

        _animations.AnimUpdate(Time.fixedDeltaTime, character);
    }

    public Vector3 GetTarget(EnemyBehaviourState behaviour)
    {
        if ((behaviour == EnemyBehaviourState.Default || character.CurrentMode == MovementMode.NavMesh)
            && agent != null && agent.enabled && agent.isOnNavMesh && agent.hasPath && !agent.pathPending)
        {
            var corners = agent.path.corners;
            if (corners != null && corners.Length > 1)
                return corners[1]; // next corner
            if (corners != null && corners.Length == 1)
                return corners[0];
        }

        return _stateHandler.Target != null
            ? _stateHandler.Target.position
            : character.transform.position; 
    }

    private void OnDrawGizmos()
    {
        if (character == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(GetTarget(_stateHandler != null ? _stateHandler.GetBehaviourState() : EnemyBehaviourState.Default), 0.1f);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(character.transform.position, GetTarget(_stateHandler != null ? _stateHandler.GetBehaviourState() : EnemyBehaviourState.Default));
    }

    public IEnemyState GetEnemyState()
    {
        return _stateHandler.GetCurrentState();
    }


}
