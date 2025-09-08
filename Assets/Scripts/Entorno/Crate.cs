using UnityEngine;

public class Crate : MonoBehaviour
{

    public float stunDuration = 2f;       
    public float knockbackForce = 7f;     
    public float minImpactVelocity = 2f;

    public Transform player;             
    public float kickRange = 2.5f;     
    public float kickForce = 10f;       
    public float kickCooldown = 0.5f;

    private Rigidbody rb;
    public bool broken = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Update()
    {
        Kick();
    }

    void Kick()
    {
        if (player == null || broken) return;

        float dist = Vector3.Distance(player.position, transform.position);
        if (dist > kickRange) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 dir = (transform.position - player.position).normalized;
            dir.y = 0f; 

            rb.AddForce(dir * kickForce, ForceMode.Impulse);

            Debug.Log("hiku");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            if (broken) return;

            EnemyMain enemy = collision.collider.GetComponent<EnemyMain>();
            if (enemy != null)
            {
                Vector3 hitDir = (enemy.transform.position - transform.position).normalized;

                enemy.stunDuration = stunDuration;
                enemy.knockbackForce = knockbackForce;

                StunState stun = enemy.GetStunState() as StunState;
                stun.SetKnockback(hitDir);

                enemy.SetState(stun);

                BreakBox();
            }
        }
    }

    void BreakBox()
    {
        broken = true;
        Destroy(gameObject);
    }
}
