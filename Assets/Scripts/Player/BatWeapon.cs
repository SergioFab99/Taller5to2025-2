using System.Collections;
using UnityEditor.Animations;
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

        playerCombat._state.CanAttack = false;

        StartCoroutine(playerCombat.ResetCanAttack((settings as BatWeaponSettings).timeBetweenSwings));

        StartCoroutine(SwingRoutine());
    }

    public override void PerformOnHit(HitInfo hitInfo)
    {
        if (hitInfo.col.TryGetComponent<TagContainer>(out TagContainer tags))
        {
            if (tags.HasTag("Damagable") && !tags.HasTag("Player"))
            {
                Debug.Log($"Bat hit {hitInfo.col.name}");

                if (tags.TryGetComponent(out HealthController health))
                {
                    health.TakeDamague((settings as BatWeaponSettings).damague);
                }

            }
        }
    }

    public override void CombatTickUpdate(float deltaTime)
    {
        if (!isSwinging || swingPoint == null) return;

        Vector3 currentPos = swingPoint.position;
        dir = currentPos - lastPos;
        float dist = dir.magnitude;

        if (dist > 0.0001f && !hitDone)
        {
            RaycastHit hit;
            if (Physics.SphereCast(lastPos,(settings as BatWeaponSettings).swingRadius, dir.normalized, out hit, dist, hitMask, QueryTriggerInteraction.Ignore))
            {

                HitInfo hitInfo = new HitInfo(hit.collider, hit.point, hit.normal, (int)((settings as BatWeaponSettings).damague));
                PerformOnHit(hitInfo);
                hitDone = true;
            }
        }

        lastPos = currentPos;
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
    }
}
