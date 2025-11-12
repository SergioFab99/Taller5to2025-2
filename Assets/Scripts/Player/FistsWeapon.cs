using Sirenix.OdinInspector;
using System;
using System.Collections;
//using UnityEditor.Animations;
using UnityEngine;


public class FistsWeapon : Weapon
{
    //public AnimatorController controller;

    public CombatHand currentHand;
    
    public LayerMask hitMask;

    public event PunchSide OnAttack;
    public delegate void PunchSide(int hand);
    private bool hitDone;
    [SerializeField] PlayerAudio playerAudio;


  
    private PlayerCombat playerCombat;
    private float timeSinceLastPunch;

    public override void Initialize(PlayerCombat playerCombat)
    {
        this.playerCombat = playerCombat;

        currentHand = CombatHand.None;

    }

    public override void Attack()
    {
        Debug.Log("Attacking");
        playerCombat._state.CanAttack = false;
        switch (currentHand)
        {
            case CombatHand.None:
                currentHand = CombatHand.Right;
                OnAttack?.Invoke(1);
                var direc = new Vector3(0.15f, -0.1f, -0.05f);
                StartCoroutine(MakeShake((settings as FistsWeaponSettings).shakeForce, (settings as FistsWeaponSettings).AttackDelay, direc));

                break;

            case CombatHand.Right:
                currentHand = CombatHand.Left;
                OnAttack?.Invoke(2);
                var direc2 = new Vector3(-0.15f, -0.1f, -0.05f);
                StartCoroutine(MakeShake((settings as FistsWeaponSettings).shakeForce, (settings as FistsWeaponSettings).AttackDelay, direc2));
                break;

            case CombatHand.Left:
                currentHand = CombatHand.Right;
                OnAttack?.Invoke(1);
                var direc3 = new Vector3(0.15f, -0.1f, -0.05f);
                StartCoroutine(MakeShake((settings as FistsWeaponSettings).shakeForce, (settings as FistsWeaponSettings).AttackDelay, direc3));
                break;
        }

        timeSinceLastPunch = Time.time;
        StartCoroutine(playerCombat.ResetCanAttack((settings as FistsWeaponSettings).timeBetweenAttacks));

        /* if (Physics.Raycast(playerCombat.playerCamera._camera.transform.position, playerCombat.playerCamera._camera.transform.forward, out RaycastHit hit, (settings as FistsWeaponSettings).attackDistance, hitMask))
         {
             var hitInfo = new HitInfo(hit.collider, hit.point, hit.normal, (settings as FistsWeaponSettings).damague);
             StartCoroutine(PerfomDelay((settings as FistsWeaponSettings).AttackDelay,hitInfo));
         } */

        Collider[] cols = Physics.OverlapBox(playerCombat.hitPoint.transform.position, (settings as FistsWeaponSettings).box, playerCombat.cam.transform.rotation, hitMask);
        foreach (Collider col in cols)
        {
            Vector3 fist = ((currentHand == CombatHand.None) || (currentHand == CombatHand.Right)) ? playerCombat.rightPunchPos.transform.position : playerCombat.leftPunchPos.transform.position;
            var hitInfo = new HitInfo(col, col.ClosestPoint(fist), (col.ClosestPoint(fist) - fist), (settings as FistsWeaponSettings).damague, Wtype,playerCombat);
            StartCoroutine(PerfomDelay((settings as FistsWeaponSettings).AttackDelay, hitInfo));
        }


    }

  
    public override void CombatTickUpdate(float deltaTime)
    {
        if (Time.time - timeSinceLastPunch > (settings as FistsWeaponSettings).timeToDoublePunch)
        {
            currentHand = CombatHand.None;
        }
    }

    public override void PerformOnHit(HitInfo hitInfo)
    {

        if (hitInfo.col.TryGetComponent<TagContainer>(out TagContainer tagContainer))
        {
            HealthController health;
            /* if (tagContainer.HasTag("BodyPart"))
             {
                 Debug.Log("Find bodyPart");
                 var character = hitInfo.col.GetComponentInParent<EnemyCharacter>() ;

                 if(character.GetComponent<TagContainer>().HasTag("Damagable"))
                 {
                     Debug.Log("Find bodyPart and Damagable parent");
                     health = character.gameObject.GetComponent<HealthController>();
                     health.TakeDamague((settings as FistsWeaponSettings).damague);
                 }
             } */
            if (tagContainer.HasTag("Damagable") && !tagContainer.HasTag("Player"))
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



    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (playerCombat != null)
        {
            Gizmos.DrawCube(playerCombat.hitPoint.transform.position, (settings as FistsWeaponSettings).box);

        }
    }
}
