using UnityEditor.Animations;
using UnityEngine;

public class BatWeapon : Weapon
{
    
    private PlayerCombat playerCombat;
    public override void Initialize(PlayerCombat playerCombat)
    {
        this.playerCombat = playerCombat;
    }

    public override void Attack()
    {
        base.Attack();
    }

    public override void PerformOnHit(HitInfo hitInfo)
    {
        base.PerformOnHit(hitInfo);
    }

    public override void CombatTickUpdate(float deltaTime)
    {
        base.CombatTickUpdate(deltaTime);
    }
}
