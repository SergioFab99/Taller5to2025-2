using UnityEngine;
using Unity.Cinemachine;
public class PlayerAttack : MonoBehaviour
{
    Animator anim;
    float timer;
    bool startTime;
    public int attack;
    [SerializeField] float shakeForce;
    [SerializeField] Vector3 velocity;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Attack();
        if (startTime)
        {
            Timer();
        }
    }

    void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {

            if (timer >= 1.5)
            {
                attack = 1;
                anim.SetInteger("Attack", attack);
                timer = 0;
            }
            else
            {
                startTime = true;
                attack++;
                anim.SetInteger("Attack", attack);
            }
            
            
            //anim.SetBool("Attacking", true);

            if (attack >= 2)
            {
                attack = 0;
                timer = 0;
                startTime = false;
            }
        }
        /*if (timer >= 1)
        {
            anim.SetBool("Attacking", false);
        }*/
        if (timer >= 2)
        {
            startTime = false;
            attack = 0;
            anim.SetInteger("Attack", attack);
            timer = 0;
        }
    }

    void Timer()
    {
        timer += Time.deltaTime;
    }
    void Shake()
    {
        CameraShake.cameraShakeInstance.Shake(shakeForce, velocity);

    }
}
