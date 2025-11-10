using UnityEngine;

public class HitInfo 
{
    public float damague;
    public Collider col;
    public Vector3 hitpoint;
    public Vector3 normal;
    public WeaponType type;
    CombatBase attacker;
    

    public HitInfo(Collider col, Vector3 point, Vector3 normal, float damage, WeaponType type, CombatBase attacker) 
    {
        this.col = col;
        this.hitpoint = point;
        this.normal = normal;
        this.damague = damage;
        this.type = type;
        this.attacker = attacker;
    }





   
}
