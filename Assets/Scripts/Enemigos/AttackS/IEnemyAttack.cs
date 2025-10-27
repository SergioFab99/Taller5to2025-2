using UnityEngine;

public interface IEnemyAttack
{
    float AttackRange { get; }
    bool IsAttacking { get; }
    bool IsFinished { get; }
    public bool Missed { get; }
    public bool ForceBlocked { get; }

    void Execute();
    void ForceCancel(bool interrupt, bool block);

    void ResetAttackCycle();
    bool WasInterrupted { get; }
    bool TryInterrupt();
}

