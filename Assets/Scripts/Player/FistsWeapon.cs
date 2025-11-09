using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

public struct punch
{
    public Transform pos;
    
    public Vector3 lastPos;
    public Vector3 dir;
    public bool isActive;

    public punch(Transform pos, Vector3 lastPos, Vector3 dir, bool isActive)
    {
        this.pos = pos;
        this.lastPos = lastPos; 
        this.dir = dir;
        this.isActive = isActive;
    }
}

public class FistsWeapon : Weapon
{
    //public AnimatorController controller;

    private punch currentPunch = new punch();
    public CombatHand currentHand;
    

    
     
    public LayerMask hitMask;

    public event PunchSide OnAttack;
    public delegate void PunchSide(int hand);
    private bool hitDone;
    [SerializeField] PlayerAudio playerAudio;


    [ShowInInspector]private punch rightPunch = new punch();
    [ShowInInspector]private punch leftPunch = new punch();
    private PlayerCombat playerCombat;
    private float timeSinceLastPunch;

    public override void Initialize(PlayerCombat playerCombat)
    {
        this.playerCombat = playerCombat;

        currentHand = CombatHand.None;
        rightPunch.pos = this.playerCombat.rightPunchPos.transform;
        leftPunch.pos = this.playerCombat.leftPunchPos.transform;
    }

    public override void Attack()
    {
        Debug.Log("Attacking");
        switch (currentHand)
        {
            case CombatHand.None:
                currentHand = CombatHand.Right;
                playerCombat._state.CanAttack = false;
                StartCoroutine(playerCombat.ResetCanAttack((settings as FistsWeaponSettings).timeBetweenAttacks));
                timeSinceLastPunch = Time.time;
                OnAttack?.Invoke(1);
                

                ActivateOrDeactivePunch(rightPunch,true);
                StartCoroutine(DeactivePunch(rightPunch,(settings as FistsWeaponSettings).punchDuration));
                break;

            case CombatHand.Right:
                currentHand = CombatHand.Left;
                playerCombat._state.CanAttack = false;
                StartCoroutine(playerCombat.ResetCanAttack((settings as FistsWeaponSettings).timeBetweenAttacks));
                timeSinceLastPunch = Time.time;
                OnAttack?.Invoke(2);

                ActivateOrDeactivePunch(leftPunch, true);
                StartCoroutine(DeactivePunch(leftPunch, (settings as FistsWeaponSettings).punchDuration));
                break;

            case CombatHand.Left:
                currentHand = CombatHand.Right;
                playerCombat._state.CanAttack = false;
                StartCoroutine(playerCombat.ResetCanAttack((settings as FistsWeaponSettings).timeBetweenAttacks));
                timeSinceLastPunch = Time.time;
                OnAttack?.Invoke(1);

                ActivateOrDeactivePunch(rightPunch, true);
                StartCoroutine(DeactivePunch(rightPunch, (settings as FistsWeaponSettings).punchDuration));


                break;
        }
    }

    public override void LinkWeapon()
    {

    }

    public override void UnlinkWeapon()
    {

    }
    public void ActivateOrDeactivePunch(punch side,bool trigger)
    {
        side.isActive = trigger;
        if (side.isActive)
        {
            hitDone = false;
            var col = Physics.OverlapSphere(side.pos.position, (settings as FistsWeaponSettings).radius, hitMask.value, QueryTriggerInteraction.Ignore);
            if (col != null && col.Length > 0)
            {
                var distance = (col[0].ClosestPoint(side.pos.position) - side.pos.position).normalized;
                HitInfo hitInfo = new HitInfo(col[0], col[0].ClosestPoint(side.pos.position), distance, (settings as FistsWeaponSettings).damague);

                PerformOnHit(hitInfo);
            }

        }

    }
    public override void CombatTickUpdate(float deltaTime)
    {
        if (Time.time - timeSinceLastPunch > (settings as FistsWeaponSettings).timeToDoublePunch)
        {
            currentHand = CombatHand.None;
        }

        if(rightPunch.isActive || leftPunch.isActive)
        {

            currentPunch = rightPunch.isActive ? rightPunch : leftPunch;
            if (hitDone)
            {
                return;
            }
            Vector3 currentPos = currentPunch.pos.position;
            currentPunch.dir = currentPos - currentPunch.lastPos;

            float dist = currentPunch.dir.magnitude;
            if (dist > 0.0001f)
            {
                RaycastHit hit;

                if (Physics.SphereCast(transform.position, (settings as FistsWeaponSettings).radius,currentPunch.dir.normalized, out hit, dist, hitMask, QueryTriggerInteraction.Ignore))
                {
                    if (currentPunch.isActive)
                    {
                        HitInfo hitInfo = new HitInfo(hit.collider, hit.point, hit.normal, (settings as FistsWeaponSettings).damague);

                        PerformOnHit(hitInfo);

                    }
                }
            }
            currentPunch.lastPos = currentPos;

        }
    }

    public override void PerformOnHit(HitInfo hitInfo)
    {
        var enemyCharacter = hitInfo.col.GetComponentInParent<EnemyCharacter>()?.gameObject;
        if (enemyCharacter != null)
        {

            if (enemyCharacter.TryGetComponent<TagContainer>(out TagContainer tagContainer) && tagContainer.HasTag("Damagable") && !tagContainer.HasTag("Player"))
            {
                Debug.Log("Hitted");
                var enemy = enemyCharacter.GetComponentInParent<Enemy>();
                if (enemy != null)
                {
                    hitInfo.enemyState = enemy.GetEnemyState();
                    enemyCharacter.TryGetComponent<HealthController>(out HealthController lifeController);
                   
                    if (hitInfo.enemyState is BlockState)
                    {
                        lifeController.TakeDamague((settings as FistsWeaponSettings).damague / 2);
                        playerAudio.PlayAttack();
                    }
                    else
                    {

                        lifeController.TakeDamague((settings as FistsWeaponSettings).damague);
                        playerAudio.PlayAttack();
                    }
                }
                hitDone = true;
            }

        }
        else
        {
            enemyCharacter = hitInfo.col.GetComponent<EnemyCharacter>()?.gameObject;
            if (enemyCharacter != null)
            {
                if (enemyCharacter.TryGetComponent<TagContainer>(out TagContainer tagContainer) && tagContainer.HasTag("Damagable") && !tagContainer.HasTag("Player"))
                {
                    Debug.Log("Hitted");
                    var enemy = enemyCharacter.GetComponentInParent<Enemy>();
                    if (enemy != null)
                    {
                        hitInfo.enemyState = enemy.GetEnemyState();
                        enemyCharacter.TryGetComponent<HealthController>(out HealthController lifeController);
                       
                        if (hitInfo.enemyState is BlockState)
                        {
                            lifeController.TakeDamague((settings as FistsWeaponSettings).damague / 2);
                            playerAudio.PlayAttack();
                        }
                        else
                        {

                            lifeController.TakeDamague((settings as FistsWeaponSettings).damague);
                            playerAudio.PlayAttack();
                        }
                    }

                   
                    hitDone = true;
                }
            }
        }
        if(hitInfo.col.TryGetComponent<TagContainer>(out TagContainer tagContainerD) && tagContainerD.HasTag("Damagable") && !tagContainerD.HasTag("Player"))
        {
            
                var healhtC = tagContainerD.gameObject.GetComponent<HealthController>();
                healhtC.TakeDamague((settings as FistsWeaponSettings).damague);
            
        }
    }

    public IEnumerator DeactivePunch(punch punch, float duration)
    {
        yield return new WaitForSeconds(duration);
        ActivateOrDeactivePunch(punch,false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (rightPunch.pos != null && leftPunch.pos != null)
        {
            // Dibuja la esfera inicial
            Gizmos.DrawWireSphere(rightPunch.pos.position, (settings as FistsWeaponSettings).radius);
            Gizmos.DrawWireSphere(leftPunch.pos.position, (settings as FistsWeaponSettings).radius);
            // Dibuja el tubo del SphereCast usando la última dirección y distancia calculada
            Vector3 directionR = rightPunch.dir.normalized;
            float distR = rightPunch.dir.magnitude;
            Vector3 directionL = leftPunch.dir.normalized;
            float distL = leftPunch.dir.magnitude;
            int steps = 10;

            for (int i = 1; i <= steps; i++)
            {
                float t = (distR / steps) * i;
                Vector3 center = rightPunch.pos.position + directionR * t;
                Gizmos.DrawWireSphere(center, (settings as FistsWeaponSettings).radius);
            }
            for (int i = 1; i <= steps; i++)
            {
                float t = (distL / steps) * i;
                Vector3 center = leftPunch.pos.position + directionL * t;
                Gizmos.DrawWireSphere(center, (settings as FistsWeaponSettings).radius);
            }
        }
    }
}
