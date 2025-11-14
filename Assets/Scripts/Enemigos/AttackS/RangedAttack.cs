using UnityEngine;

public class RangedAttack : MonoBehaviour, IEnemyAttack
{
    public float attackRange = 15f;
    public float meleeRange = 2f;

    public int maxAmmo = 10;
    public float reloadTime = 3f;
    public float timeBetweenShots = 0.4f;
    public int burstMin = 1;
    public int burstMax = 3;
    public GameObject bulletPrefab;
    public Transform firePoint; 
    public float bulletSpeed = 30f;
    public float bulletDamage = 25f;
    public float meleeDamage = 5f;

    private EnemyMain ai;
    public int currentAmmo;
    private bool isAttacking = false;
    private bool finished = false;
    private bool interrupted = false;
    private bool reloading = false;

    private int burstShots;
    private int shotsFired;

    public float AttackRange => attackRange;
    public bool IsAttacking => isAttacking;
    public bool IsFinished => finished;
    public bool WasInterrupted => interrupted;

    void Awake()
    {
        ai = GetComponent<EnemyMain>();
        currentAmmo = maxAmmo;
    }

    public void Execute()
    {
        if (ai.target == null) return;
        if (isAttacking || reloading) return;

        float dist = Vector3.Distance(transform.position, ai.target.position);

        finished = false;
        interrupted = false;

        if (dist <= meleeRange)
        {
            DoMelee();
        }
        else
        {
            if (currentAmmo <= 0)
            {
                StartReload();
                return;
            }

            burstShots = Random.Range(burstMin, burstMax + 1);
            shotsFired = 0;

            ai.StopMovement();
            isAttacking = true;
            Invoke(nameof(FireShot), 0.2f); 
        }
    }

    private void FireShot()
    {
        if (ai.target == null) { EndBurst(); return; }
        if (currentAmmo <= 0) { StartReload(); return; }

        currentAmmo--;
        shotsFired++;

        Debug.Log($"{ai.name} fires bullet ({currentAmmo} left)");

        Vector3 dir = (ai.target.position - firePoint.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, rot);
        EBullet bullet = bulletObj.GetComponent<EBullet>();
        bullet.Init(ai, ai.EffectiveDamage(bulletDamage));
        bullet.speed = bulletSpeed;

        if (shotsFired < burstShots && currentAmmo > 0)
        {
            Invoke(nameof(FireShot), timeBetweenShots);
        }
        else
        {
            EndBurst();
        }
    }

    private void EndBurst()
    {
        isAttacking = false;
        ai.Movement();

        if (currentAmmo <= 0)
        {
            StartReload();
        }
        else
        {
            finished = true;
        }
    }

    private void DoMelee()
    {
        Debug.Log($"{ai.name} pistol whips");

        Collider[] hits = Physics.OverlapSphere(transform.position, meleeRange);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Player hit by pistol whip");
            }
        }

        finished = true;
    }

    public void StartReload()
    {
        if (reloading) return;
        reloading = true;
        isAttacking = false;
        finished = false;

        Debug.Log($"{ai.name} is reloading...");
        Invoke(nameof(FinishReload), reloadTime);
    }

    private void FinishReload()
    {
        reloading = false;
        isAttacking = false;
        currentAmmo = maxAmmo;
        finished = true;
        interrupted = false;
        Debug.Log($"{ai.name} finished reloading");
    }

    private void FinishAttack()
    {
        isAttacking = false;

        if (reloading)
            finished = true;
        else
            finished = false; 
    }

    public void ForceCancel()
    {
        CancelInvoke();
        isAttacking = false;
        finished = true;
        interrupted = true;
    }

    public void ResetAttackCycle()
    {
        CancelInvoke();
        isAttacking = false;
        finished = false;
        interrupted = false;
    }
}
