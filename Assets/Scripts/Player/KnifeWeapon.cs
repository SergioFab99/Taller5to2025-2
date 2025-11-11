using UnityEngine;

public class KnifeWeapon : Weapon
{
    private PlayerCombat playerCombat;
    public float force;
    public GameObject throwWeapon;
    public LayerMask hitMask;
    public event BatAttack OnAttack;
    public delegate void BatAttack();
    public override void Initialize(PlayerCombat playerCombat)
    {
        this.playerCombat = playerCombat;
       
    }

    public override void Attack()
    {
        if ( !playerCombat._state.CanAttack)
            return;

        Debug.Log("Attack");
        OnAttack?.Invoke();
        playerCombat._state.CanAttack = false;


        StartCoroutine(playerCombat.ResetCanAttack((settings as KnifeWeaponSettings).timeBetweenSlash));
        var direc2 = new Vector3(-0.15f, -0.1f, -0.05f);
        StartCoroutine(MakeShake((settings as KnifeWeaponSettings).shakeForce, (settings as KnifeWeaponSettings).AttackDelay, direc2));
        
        Collider[] cols = Physics.OverlapBox(playerCombat.hitPoint.transform.position, (settings as KnifeWeaponSettings).box, playerCombat.cam.transform.rotation, hitMask);
        foreach (Collider col in cols)
        {

            var hitInfo = new HitInfo(col, col.ClosestPoint(playerCombat.currentWeapon.transform.position), (col.ClosestPoint(playerCombat.currentWeapon.transform.position) - playerCombat.currentWeapon.transform.position), (settings as BatWeaponSettings).damague);
            StartCoroutine(PerfomDelay((settings as KnifeWeaponSettings).AttackDelay, hitInfo));
        }
    }
    public override void PerformOnHit(HitInfo hitInfo)
    {

        Debug.Log("Perform");
        if (hitInfo.col.TryGetComponent<TagContainer>(out TagContainer tagContainer))
        {
            HealthController health;
            /* if (tagContainer.HasTag("BodyPart"))
            {
                Debug.Log("Find bodyPart");
                var character = hitInfo.col.GetComponentInParent<EnemyCharacter>();

                if (character.GetComponent<TagContainer>().HasTag("Damagable"))
                {
                    Debug.Log("Find bodyPart and Damagable parent");
                    health = character.gameObject.GetComponent<HealthController>();
                    health.TakeDamague((settings as FistsWeaponSettings).damague);
                }
            } */
            if (tagContainer.HasTag("Damagable") && !tagContainer.HasTag("Player"))
            {
                health = hitInfo.col.gameObject.GetComponent<HealthController>();
                health.TakeDamague((settings as BatWeaponSettings).damague);
            }
            else
            {
                Debug.Log("This doesnt have ");
            }

        }
    }
    public override void Throw(Vector3 direc)
    {
        Debug.Log("This do something, Throw");
        var ThrowObject = Instantiate(throwWeapon, transform.position, transform.rotation);
        ThrowObject.GetComponent<Rigidbody>().AddForce(direc * force, ForceMode.Impulse);
        ThrowObject.GetComponent<PickUpWeapon>().hited = false;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (playerCombat != null)
        {
            Gizmos.DrawCube(playerCombat.hitPoint.transform.position, (settings as BatWeaponSettings).box);

        }
    }

}
