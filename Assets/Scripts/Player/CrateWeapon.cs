using UnityEngine;

public class CrateWeapon : Weapon
{
    private PlayerCombat playerCombat;
   
    public float force;
    public GameObject throwWeapon;


    private bool hitDone;
    private float swingStartTime;
    public LayerMask hitMask;    
    public event CrateAttack OnCrateAttack;
    public delegate void CrateAttack();    

    public override void Initialize(PlayerCombat playerCombat)
    {
        this.playerCombat = playerCombat;
        
        hitDone = false;      
    }

    public override void Attack()
    {
        if (!playerCombat._state.CanAttack)
            return;

        Debug.Log("Attack");
        OnCrateAttack?.Invoke();
        playerCombat._state.CanAttack = false;


        StartCoroutine(playerCombat.ResetCanAttack((settings as CrateWeaponSettings).timeBetweenSwings));
        var direc2 = new Vector3(-0.15f, -0.1f, -0.05f);
        StartCoroutine(MakeShake((settings as CrateWeaponSettings).shakeForce, (settings as CrateWeaponSettings).AttackDelay, direc2));
        /* if (Physics.Raycast(playerCombat.playerCamera._camera.transform.position, playerCombat.playerCamera._camera.transform.forward, out RaycastHit hit, (settings as FistsWeaponSettings).attackDistance, hitMask))
         {
             var hitInfo = new HitInfo(hit.collider, hit.point, hit.normal, (settings as BatWeaponSettings).damague);
             StartCoroutine(PerfomDelay((settings as BatWeaponSettings).AttackDelay, hitInfo));
         } */
        Collider[] cols = Physics.OverlapBox(playerCombat.hitPoint.transform.position, (settings as CrateWeaponSettings).box, playerCombat.cam.transform.rotation, hitMask);
        foreach (Collider col in cols)
        {
           
            var hitInfo = new HitInfo(col, col.ClosestPoint(playerCombat.currentWeapon.transform.position), (col.ClosestPoint(playerCombat.currentWeapon.transform.position) - playerCombat.currentWeapon.transform.position), (settings as CrateWeaponSettings).damague, Wtype,playerCombat);
            StartCoroutine(PerfomDelay((settings as CrateWeaponSettings).AttackDelay, hitInfo));
        }
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
        Debug.Log("This do something, Throw");
        var ThrowObject = Instantiate(throwWeapon, transform.position, transform.rotation);
        ThrowObject.GetComponent<Rigidbody>().AddForce(direc * force , ForceMode.Impulse);
        ThrowObject.GetComponent<PickUpWeapon>().hited = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (playerCombat != null)
        {
            Gizmos.DrawCube(playerCombat.hitPoint.transform.position, (settings as CrateWeaponSettings).box);

        }
    }
}

