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

  
    private bool hitDone;
    [SerializeField] PlayerAudio playerAudio;


  
    private PlayerCombat playerCombat;
    private float timeSinceLastPunch;
    private int arm;
    private bool isAttacking;

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
                arm = 0;
                /*var direc = new Vector3(0.15f, -0.1f, -0.05f);
                StartCoroutine(MakeShake((settings as FistsWeaponSettings).shakeForce, (settings as FistsWeaponSettings).AttackDelay, direc)); */

                break;

            case CombatHand.Right:
                currentHand = CombatHand.Left;
                arm = 1;
                /* var direc2 = new Vector3(-0.15f, -0.1f, -0.05f);
                 StartCoroutine(MakeShake((settings as FistsWeaponSettings).shakeForce, (settings as FistsWeaponSettings).AttackDelay, direc2)); */
                break;

            case CombatHand.Left:
                currentHand = CombatHand.Right;
                arm = 0;
               /* var direc3 = new Vector3(0.15f, -0.1f, -0.05f);
                StartCoroutine(MakeShake((settings as FistsWeaponSettings).shakeForce, (settings as FistsWeaponSettings).AttackDelay, direc3)); */
                break;
        }

        timeSinceLastPunch = Time.time;
        StartCoroutine(playerCombat.ResetCanAttack((settings as FistsWeaponSettings).timeBetweenAttacks));
        isAttacking = true;
        StartCoroutine(ResetIsAttacking((settings as FistsWeaponSettings).punchDuration));
        //StartCoroutine(MakeAttack((settings as FistsWeaponSettings).AttackDelay));
        /* if (Physics.Raycast(playerCombat.playerCamera._camera.transform.position, playerCombat.playerCamera._camera.transform.forward, out RaycastHit hit, (settings as FistsWeaponSettings).attackDistance, hitMask))
         {
             var hitInfo = new HitInfo(hit.collider, hit.point, hit.normal, (settings as FistsWeaponSettings).damague);
             StartCoroutine(PerfomDelay((settings as FistsWeaponSettings).AttackDelay,hitInfo));
         } */

        


    }

    public IEnumerator MakeAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        DetectAttackandPerform();

    }

    public void DetectAttackandPerform()
    {
        Collider[] cols = Physics.OverlapBox(playerCombat.hitPoint.transform.position, (settings as FistsWeaponSettings).box, playerCombat.cam.transform.rotation, hitMask);
        foreach (Collider col in cols)
        {
            Vector3 fist = ((currentHand == CombatHand.None) || (currentHand == CombatHand.Right)) ? playerCombat.rightPunchPos.transform.position : playerCombat.leftPunchPos.transform.position;
            var hitInfo = new HitInfo(col, col.ClosestPoint(fist), (col.ClosestPoint(fist) - fist), (settings as FistsWeaponSettings).damague, Wtype, playerCombat);
            PerformOnHit(hitInfo);
            
        }
    }

  
    public override void CombatTickUpdate(float deltaTime)
    {
        if (Time.time - timeSinceLastPunch > (settings as FistsWeaponSettings).timeToDoublePunch)
        {
            currentHand = CombatHand.None;
        }

        if(isAttacking)
        {
            
            var pos = arm == 0 ? playerCombat.rightPunchPos.transform.position : playerCombat.leftPunchPos.transform.position;
            var cols = Physics.OverlapSphere(pos, (settings as FistsWeaponSettings).radius);
            foreach (Collider col in cols)
            {
                var hitInfo = new HitInfo(col, col.ClosestPoint(pos), (col.ClosestPoint(pos) - pos), (settings as FistsWeaponSettings).damague, Wtype, playerCombat);
                PerformOnHit(hitInfo);
            }
        }
        
    }



    [SerializeField] OpenDoor openDoor;
    public override void PerformOnHit(HitInfo hitInfo)
    {

        //mirar donde esta el componente
        var recv = hitInfo.col.GetComponent<CombatHitReceiver>() 
                ?? hitInfo.col.GetComponentInParent<CombatHitReceiver>() 
                ?? hitInfo.col.GetComponentInChildren<CombatHitReceiver>();

        if (recv != null)
        {
             recv.OnHit(hitInfo);
            switch (currentHand)
            {
                case CombatHand.Right:
                    var direc = new Vector3(0.15f, -0.1f, -0.05f);
                    MakeShake((settings as FistsWeaponSettings).shakeForce, (settings as FistsWeaponSettings).AttackDelay, direc);
                    break;

                case CombatHand.Left:
                    var direc2 = new Vector3(-0.15f, -0.1f, -0.05f);
                    MakeShake((settings as FistsWeaponSettings).shakeForce, (settings as FistsWeaponSettings).AttackDelay, direc2);
                    break;
            }
             return;
        }
        else if (recv == null)
        {
            openDoor = hitInfo.col.GetComponent<OpenDoor>();
            hitInfo.col.gameObject.TryGetComponent<OpenDoor>(out OpenDoor door);
            bool isHittingDoor = door;
            if (isHittingDoor)
            {
                DoorsAttacking();
            }
        }
       
                
    }
    public void DoorsAttacking()
    {
        if (DisplayInteractHUD.thisIsTutorial)
        {
            return;
        }
        else
        {
            if (openDoor != null)
            {
                openDoor.CallStartPushOpen();
                openDoor.starMoveDoor = true;
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

    IEnumerator ResetIsAttacking(float delay)
    {
        yield return new  WaitForSeconds (delay);
        isAttacking = false;
    }
}
