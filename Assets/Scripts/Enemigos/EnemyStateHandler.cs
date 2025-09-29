using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyStateHandler : MonoBehaviour
{
    public Transform Target;
    private Transform Character;
    public EnemyBehaviourState EnemyBehaviourState = EnemyBehaviourState.Default;
    
    [SerializeField]private IEnemyState currentState;
    


    private IdleState idle;
    private PatrolState walking;
    private AlertState alert;
    private DeadState dead;

    private StunState stunned;
    private BlockState block;
    private ExposedState exposed;
    private bool isTransitioning;




    [NonSerialized] public EnemySettingsList enemySettings;

    public void Initialize(EnemySettingsList enemySettings, Transform Character)
    {
        this.Character = Character;
        this.enemySettings = enemySettings;
        idle = new IdleState(this);
        alert = new AlertState(this);

        dead = new DeadState(this);
        stunned = new StunState(this);
        block = new BlockState(this);
        exposed = new ExposedState(this);

        SetState(idle);
    }

    public void CurrentStateUpdate()
    {
        
        currentState.Update();
    }


    public void SetBehaviourState(EnemyBehaviourState enemyState)
    {
        EnemyBehaviourState = enemyState;
    }

    public EnemyBehaviourState GetBehaviourState()
    {
        return EnemyBehaviourState;
    }




  
    public void SetState(IEnemyState newState)
    {
        if (isTransitioning || newState == currentState) return;

        isTransitioning = true;

        currentState?.OnExit();
        currentState = newState;
        currentState?.OnEnter();


        isTransitioning = false;
    }

    public bool CheckTargetOnView(Transform target = null)
    {
        if (target == null) return false;
       
        return (Vector3.Distance(Character.position, target.position) <= enemySettings.AISettings.detectionDistance);
    }

    public bool CheckTargetOnAttackRange(Transform target = null)
    {
        if (target == null)
        {
            return false;
        }
        return Vector3.Distance(Character.position, target.position) <= enemySettings.AISettings.attackRange;
    }


   

    public IEnemyState GetCurrentState() => currentState;
    public IEnemyState GetIdleState() => idle;
    public IEnemyState GetAlertState() => alert;

    public IEnemyState GetDeadState() => dead;
    public IEnemyState GetStunState() => stunned;
    public IEnemyState GetBlockState() => block;
    public IEnemyState GetExposedState() => exposed;

}
