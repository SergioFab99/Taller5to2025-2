using KinematicCharacterController;
using UnityEngine;

[System.Serializable]
public class IdleState1 : IEnemyState
{
    private EnemyStateHandler ai;
    PatrolPointsForEnemies[] patrolFounds;
    [SerializeField] private SpawnEnemiesLvl1 spawnEnemiesLvl1;
    public IdleState1(EnemyStateHandler main)
    {
        ai = main;
    }

    public void OnEnter()
    {
        ai.ExitCombatMode();
        if (DisplayInteractHUD.thisIsLevel1)
        {            
            ai.Target = ai.TargetPatrol;
            spawnEnemiesLvl1 = GameObject.Find("SpawnEnemies").GetComponent<SpawnEnemiesLvl1>();
            patrolFounds = spawnEnemiesLvl1.GetPatrolPoints();
        }
        
        Debug.Log($"{ai.name} is now idle.");
    }

    public void Update()
    {
        if (ai.TargetPlayer != null && ai.CheckTargetOnView())
        {
            ai.SetState(ai.GetAlertState());
        }
        if(ai.TargetPatrol != null && ai.agent != null&& DisplayInteractHUD.thisIsLevel1)
        {
            ai.MoveTowardsTarget();
            
            if (ai.agent.remainingDistance <= ai.agent.stoppingDistance)
            {
                FoundPatrolPoints();
            }
        }
    }
    void FoundPatrolPoints()
    {
        if (patrolFounds.Length > 0)
        {
            PatrolPointsForEnemies pointNear1 = patrolFounds[0];
            float distancePointNear1 = Vector3.Distance(ai.transform.position, pointNear1.transform.position);
            foreach (PatrolPointsForEnemies point in patrolFounds)
            {
                float distanceOfPoint = Vector3.Distance(ai.transform.position, point.transform.position);
                if (distancePointNear1 > distanceOfPoint)
                {
                    pointNear1 = point;
                    distancePointNear1 = distanceOfPoint;
                }
            }
            ai.TargetPatrol = pointNear1.gameObject.transform;
        }
    }
    public void OnExit()
    {
        ai.Target = ai.TargetPlayer;
        Debug.Log($"{ai.name} left idle state.");
    }

    public Quaternion UpdateRotation(Quaternion currentRotation, float deltaTime, Vector3 _requestedRotation, KinematicCharacterMotor motor)
    {
        return currentRotation;
    }

    public Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime, KinematicCharacterMotor motor, Vector3 _requestedMovement, EnemySettingsList Settings, ref float _timeSinceUngrounded)
    {
        return Vector3.zero;
    }
}
