
using UnityEngine;
using UnityEngine.AI;

public struct EnemyInput
{
    public Vector3 Direction;
    public Vector3 Move;
    public CrouchInput Crouch;
}


public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyCharacter character;
    //[SerializeField] EnemyCharacterState _characterState;
    //[SerializeField] EnemyCharacterState _lastCharacterState;
    [SerializeField] HealthController healthController;
    [SerializeField] EnemyStateHandler _stateHandler;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] EnemySettingsList EnemySettings;
    //[SerializeField] CombatManager



    public Transform testTarget;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _stateHandler.Initialize(EnemySettings, character.transform);

        character.Initialize(EnemySettings, _stateHandler.GetBehaviourState(), _stateHandler.GetCurrentState());

        healthController.OnDead += Ondead;
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

        switch (_stateHandler.GetBehaviourState())
        {
            case EnemyBehaviourState.Default:
                {
                    float distance = Vector3.Distance(GetTarget(), character.transform.position);
                    Vector3 direction = (testTarget.position - character.transform.position).normalized;

                    if (distance > EnemySettings.AISettings.stopingDistance)
                    {
                        enemyInput = new EnemyInput
                        {
                            Direction = direction,
                            Move = direction
                        };
                    }
                    else
                    {
                        enemyInput = new EnemyInput
                        {
                            Direction = direction,
                            Move = Vector3.zero
                        };
                    }

                    character.UpdateInputs(enemyInput, _stateHandler.GetBehaviourState());
                    break;
                }

            case EnemyBehaviourState.Combat:
                {
                    Vector3 direction = (testTarget.position - character.transform.position).normalized;
                    enemyInput = new EnemyInput
                    {
                        Direction = direction,
                        Move = direction
                    };

                    character.UpdateInputs(enemyInput, _stateHandler.GetBehaviourState());
                    break;
                }

            case EnemyBehaviourState.Dead:
                enemyInput = new EnemyInput
                {
                    Direction = Vector3.zero,
                    Move = Vector3.zero
                };
                character.UpdateInputs(enemyInput, _stateHandler.GetBehaviourState());
                break;
        }
    }

    private void LateUpdate()
    {
        //_characterState = character.GetState();
        //_lastCharacterState = character.GetLastState();
    }

    public Vector3 GetTarget()
    {
        return agent.steeringTarget;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(GetTarget(), 0.1f);
    }

    public IEnemyState GetEnemyState()
    {
        return _stateHandler.GetCurrentState();
    }


}
