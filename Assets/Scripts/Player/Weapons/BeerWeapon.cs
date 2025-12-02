using UnityEngine;

public class BeerWeapon : Weapon
{
    private PlayerCombat playerCombat;
   
    public float force;
    public GameObject throwWeapon;


    private bool hitDone;
    private float swingStartTime;
    public LayerMask hitMask; 

    public HealthController playerHealth;   
    
    public delegate void CrateAttack();    

    public override void Initialize(PlayerCombat playerCombat)
    {
        this.playerCombat = playerCombat;
        
        hitDone = false;

         if (playerHealth == null)
        {
            playerHealth = playerCombat.GetComponent<HealthController>()
                        ?? playerCombat.GetComponentInChildren<HealthController>()
                        ?? playerCombat.GetComponentInParent<HealthController>();
        }

    }

    public override void Attack()
    {
        if (!playerCombat._state.CanAttack)
            return;

        Debug.Log("Use Beer");
        
        playerCombat._state.CanAttack = false;    

        var hitInfo = new HitInfo(playerHealth.gameObject.GetComponent<Collider>(), playerHealth.transform.position, Vector3.up, -(settings as BeerWeaponSettings).healAmount, Wtype, playerCombat);       
        if (hitInfo.col != null)
            PerformOnHit(hitInfo);
        else
            Debug.Log("No col");
              
        if (playerCombat != null)
        {
            if (playerCombat.fistWeapon != null)
                playerCombat.LinkWeapon(playerCombat.fistWeapon);

            playerCombat._state.CanAttack = true;
        }

        Destroy(this.gameObject);
        
    }

    public override void PerformOnHit(HitInfo hitInfo)
    {
        //mirar donde esta el componente
        var recv = hitInfo.col.GetComponent<CombatHitReceiver>() 
                ?? hitInfo.col.GetComponentInParent<CombatHitReceiver>() 
                ?? hitInfo.col.GetComponentInChildren<CombatHitReceiver>();

        if (recv != null)
        {
             recv.OnHit(hitInfo);
             return;
        }   

        
    }

    public override void CombatTickUpdate(float deltaTime)
    {
        
       
    }
   

    public override void Throw(Vector3 direc)
    {
        /*
        Debug.Log("This do something, Throw");
        var ThrowObject = Instantiate(throwWeapon, transform.position, transform.rotation);
        ThrowObject.GetComponent<Rigidbody>().AddForce(direc * force , ForceMode.Impulse);
        var pick = ThrowObject.GetComponent<PickUpWeapon>();
        if (pick != null)
        {
            pick.hited = false;
            pick.Wtype = this.Wtype;
            pick.attacker = playerCombat;
        }
        */
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (playerCombat != null)
        {
            //Gizmos.DrawCube(playerCombat.hitPoint.transform.position, (settings as CrateWeaponSettings).box);

        }
    }
}
