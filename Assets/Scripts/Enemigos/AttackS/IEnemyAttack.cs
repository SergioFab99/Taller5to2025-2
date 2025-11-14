using UnityEngine;

public interface IEnemyAttack
{
    float AttackRange { get; }
    bool IsAttacking { get; }
    bool IsFinished { get; }

    void Execute();
    void ForceCancel();
    void ResetAttackCycle();
    bool WasInterrupted { get; }
}

