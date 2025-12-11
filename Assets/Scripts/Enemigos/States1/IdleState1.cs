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

        ai.StopAllMovement();
        ai.character.SetMovementMode(MovementMode.NavMesh);

        if (DisplayInteractHUD.thisIsLevel1 || DisplayInteractHUD.thisIsLevel2)
        {
            ai.Target = ai.TargetPatrol;

            if (!spawnEnemiesLvl1)
            {
                GameObject spawner = GameObject.Find("SpawnEnemies");
                if (spawner)
                    spawnEnemiesLvl1 = spawner.GetComponent<SpawnEnemiesLvl1>();
            }

            patrolFounds = spawnEnemiesLvl1 ? spawnEnemiesLvl1.GetPatrolPoints() : null;
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
            Vector3 dir = (ai.TargetPatrol.position - ai.character.transform.position).normalized;
            dir.y = 0f;

            float dist = Vector3.Distance(ai.character.transform.position, ai.TargetPatrol.position);
            float stopDist = ai.agent.stoppingDistance;

            bool shouldMove = dist > stopDist + 0.5f;

            ai.character.UpdateInputs(new EnemyInput
            {
                Direction = dir,
                Move = shouldMove ? dir : Vector3.zero
            }, ai.GetBehaviourState());

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

        PatrolPointsForEnemies nearest = patrolFounds[0];
        float nearestDist = Vector3.Distance(ai.transform.position, nearest.transform.position);

        foreach (var p in patrolFounds)
        {
            float dist = Vector3.Distance(ai.transform.position, p.transform.position);
            if (dist < nearestDist)
            {
                nearest = p;
                nearestDist = dist;
            }
        }

        ai.TargetPatrol = nearest.transform;
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
        return currentVelocity;
    }
}