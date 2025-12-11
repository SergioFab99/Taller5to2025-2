
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public struct EnemyInput
{
    public Vector3 Direction;
    public Vector3 Move;
    public bool Jump;
}


public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyCharacter character;
    [SerializeField] HealthController healthController;
    [SerializeField] EnemyAnimations _animations;
    [SerializeField] EnemyStateHandler _stateHandler;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] EnemySettingsList EnemySettings;

    public MeleeAttack melee;

    public BatAttack bat;

    public Transform testTarget;

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

        _animations.AnimUpdate(Time.fixedDeltaTime, character,melee,bat, _stateHandler);
    }

    public Vector3 GetTarget(EnemyBehaviourState behaviour)
    {
        if ((behaviour == EnemyBehaviourState.Default || character.CurrentMode == MovementMode.NavMesh)
            && agent != null && agent.enabled && agent.isOnNavMesh && agent.hasPath && !agent.pathPending)
        {
            var corners = agent.path.corners;
            if (corners != null && corners.Length > 1)
                return corners[1];
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
