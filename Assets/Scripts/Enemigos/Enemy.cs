
using UnityEngine;

public struct EnemyInput
{
    public Vector3 Direction;
    public Vector3 Move;
    public CrouchInput Crouch;
}


public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyCharacter character;
    [SerializeField] EnemyCharacterState _characterState;
    [SerializeField] EnemyCharacterState _lastCharacterState;
    [SerializeField] HealthController healthController;
    [SerializeField] EnemyStateHandler _stateHandler;


    [SerializeField] EnemySettingsList EnemySettings;
    //[SerializeField] CombatManager


    
    public Transform testTarget;


   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        character.Initialize(EnemySettings,_stateHandler.GetBehaviourState());


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var enemyInput = new EnemyInput();
        switch(_stateHandler.GetBehaviourState())
        {
            case EnemyBehaviourState.Default:

                float distance = Vector3.Distance(GetTarget(),character.transform.position);
                Vector3 direction = (GetTarget() - character.transform.position).normalized;
                if(distance > EnemySettings.AISettings.stopingDistance)
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
                break;
            case EnemyBehaviourState.Combat:
                break;
            case EnemyBehaviourState.Dead:
                break;
        }
         character.UpdateInputs(enemyInput,_stateHandler.GetBehaviourState());



    }

    private void LateUpdate()
    {
        _characterState = character.GetState();
        _lastCharacterState = character.GetLastState();
    }

    public Vector3 GetTarget()
    {
        Vector3 target = Vector3.zero;
        target = testTarget.position;
        return target;
    }

    
}
