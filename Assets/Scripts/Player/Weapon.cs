
using UnityEngine;

public enum WeaponType
{
    Fist,
    Knife,
    Bat,

}


public abstract class Weapon : MonoBehaviour
{
    
    public delegate void OnHitEvent(HitInfo info);
    public event OnHitEvent OnHit;

    public WeaponSettings settings;
    public WeaponType Wtype;

    public TagContainer tagContainer;
    public virtual void Initialize(PlayerCombat playerCombat)
    {
       
    }

    public virtual void Attack()
    {

    }

    public virtual void LinkWeapon()
    {

    }

    public virtual void UnlinkWeapon()
    {

    }

    public virtual void PerformOnHit(HitInfo hitInfo)
    {

    }
    public virtual void CombatTickUpdate(float deltaTime)
    {

    }

    public virtual void Throw(Vector3 direc)
    {
        Debug.Log("Maybe this to nothing");
    }

}
