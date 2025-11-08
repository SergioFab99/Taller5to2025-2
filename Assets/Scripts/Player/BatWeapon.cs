using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class BatWeapon : Weapon
{
    private PlayerCombat playerCombat;
    public Transform swingPoint;
    public float force;
    public GameObject throwWeapon;

    private bool isSwinging;
    private bool hitDone;
    private Vector3 lastPos;
    private Vector3 dir;
    private float swingStartTime;
    public LayerMask hitMask;
    public event BatAttack OnAttack;
    public delegate void BatAttack();

    public override void Initialize(PlayerCombat playerCombat)
    {
        this.playerCombat = playerCombat;
        isSwinging = false;
        hitDone = false;
        if (swingPoint == null)
        {
            Debug.LogWarning("Swing point not assigned for BatWeapon!");
        }
    }

    public override void Attack()
    {
        if (isSwinging || !playerCombat._state.CanAttack)
            return;

        Debug.Log("Attack");
        OnAttack?.Invoke();
        playerCombat._state.CanAttack = false;


        StartCoroutine(playerCombat.ResetCanAttack((settings as BatWeaponSettings).timeBetweenSwings));
        var direc2 = new Vector3(-0.15f, -0.1f, -0.05f);
        StartCoroutine(MakeShake((settings as BatWeaponSettings).shakeForce, (settings as BatWeaponSettings).AttackDelay, direc2));
        if (Physics.Raycast(playerCombat.playerCamera._camera.transform.position, playerCombat.playerCamera._camera.transform.forward, out RaycastHit hit, (settings as FistsWeaponSettings).attackDistance, hitMask))
        {
            var hitInfo = new HitInfo(hit.collider, hit.point, hit.normal, (settings as BatWeaponSettings).damague);
            StartCoroutine(PerfomDelay((settings as BatWeaponSettings).AttackDelay, hitInfo));
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
            if (tagContainer.HasTag("Damagable"))
            {
                health = hitInfo.col.gameObject.GetComponent<HealthController>();
                health.TakeDamague((settings as FistsWeaponSettings).damague);
            }
            else
            {
                Debug.Log("This doesnt have ");
            }

        }
    }

    public override void CombatTickUpdate(float deltaTime)
    {
        
       
    }
    private IEnumerator SwingRoutine()
    {
        isSwinging = true;
        hitDone = false;
        swingStartTime = Time.time;


        yield return new WaitForSeconds((settings as BatWeaponSettings).swingDuration);

        isSwinging = false;
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
            Gizmos.DrawCube(playerCombat.hitPoint.transform.position, (settings as FistsWeaponSettings).box);

        }
    }
}
