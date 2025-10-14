using UnityEngine;

public class BruiserEnemy : EnemyMain
{
    public override void Movement()
    {
        MoveTowardsTarget();
    }
}