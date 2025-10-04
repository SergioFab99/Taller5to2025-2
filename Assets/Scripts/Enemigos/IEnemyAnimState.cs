using UnityEngine;

public interface IEnemyAnimState
{
    void OnExit();
    void OnEnter();

    void AnimStateUpdate();
}
