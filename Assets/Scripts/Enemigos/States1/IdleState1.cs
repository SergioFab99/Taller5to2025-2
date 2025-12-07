using KinematicCharacterController;
using UnityEngine;

[System.Serializable]
public class IdleState1 : IEnemyState
{
    private EnemyStateHandler ai;
    private PatrolPointsForEnemies[] patrolFounds;
    private SpawnEnemiesLvl1 spawnEnemiesLvl1;

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
            spawnEnemiesLvl1 = GameObject.Find("SpawnEnemies")?.GetComponent<SpawnEnemiesLvl1>();
            patrolFounds = spawnEnemiesLvl1?.GetPatrolPoints();
        }

        Debug.Log($"{ai.name} entered IDLE.");
    }

    public void Update()
    {
        if (ai.TargetPlayer != null && ai.CheckTargetOnView())
        {
            ai.SetState(ai.GetAlertState());
            return;
        }

        if (DisplayInteractHUD.thisIsLevel1 && ai.TargetPatrol != null && ai.agent != null)
        {
            Vector3 direction = (ai.TargetPatrol.position - ai.character.transform.position).normalized;
            direction.y = 0f;

            float distance = Vector3.Distance(ai.character.transform.position, ai.TargetPatrol.position);
            float stopDistance = ai.agent.stoppingDistance;
            Vector3 moveInput = (distance > stopDistance + 0.5f) ? direction : Vector3.zero;

            var enemyInput = new EnemyInput
            {
                Direction = direction,
                Move = moveInput
            };
            ai.character.UpdateInputs(enemyInput, ai.GetBehaviourState());

            ai.MoveTowardsTarget();

            if (ai.agent.remainingDistance <= ai.agent.stoppingDistance)
            {
                FoundPatrolPoints();
            }
        }
    }

    private void FoundPatrolPoints()
    {
        if (patrolFounds == null || patrolFounds.Length == 0) return;

        PatrolPointsForEnemies nearestPoint = patrolFounds[0];
        float nearestDistance = Vector3.Distance(ai.transform.position, nearestPoint.transform.position);

        foreach (PatrolPointsForEnemies point in patrolFounds)
        {
            float dist = Vector3.Distance(ai.transform.position, point.transform.position);
            if (dist < nearestDistance)
            {
                nearestPoint = point;
                nearestDistance = dist;
            }
        }

        ai.TargetPatrol = nearestPoint.transform;
    }

    public void OnExit()
    {
        ai.Target = ai.TargetPlayer;
        Debug.Log($"{ai.name} exited IDLE.");
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