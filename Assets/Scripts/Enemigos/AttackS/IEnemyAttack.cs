using UnityEngine;

public interface IEnemyAttack
{
    float AttackRange { get; }
    bool IsAttacking { get; }
    bool IsFinished { get; }
    public bool Missed { get; }
    public bool ForceBlocked { get; }

    void Execute();
    void ManualUpdate();
    void BeginAttack(EnemyStateHandler handler);
    void ForceCancel(bool wasInterrupted, bool wasBlocked);

    void ResetAttackCycle();
    bool WasInterrupted { get; }
    bool TryInterrupt();
}

