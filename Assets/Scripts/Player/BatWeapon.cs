using UnityEngine;

public class BatWeapon : Weapon
{
    private PlayerCombat playerCombat;
    public override void Initialize(PlayerCombat playerCombat)
    {
        this.playerCombat = playerCombat;
    }


}
