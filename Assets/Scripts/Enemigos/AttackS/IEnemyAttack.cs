using UnityEngine;

public interface IEnemyAttack 
{
    void Execute();
    bool IsAttacking { get; }
    float AttackRange { get; }
}
