using UnityEngine;

public class ArmAnimation : MonoBehaviour
{
    Animator anim;
    float timer;
    bool startTime;
    public int attack;

    CombatState state;

    public float punchTimeModifier;

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
        if(Input.GetMouseButtonDown(0))
        {
            startTime = true;
            attack++;
            anim.SetInteger("Attack", attack);
            //anim.SetBool("Attacking", true);
            if (attack >= 2)
            {
                attack = 2;
                if(timer >= 1)
                {
                    attack = 1;
                    anim.SetInteger("Attack", attack);
                    timer = 0;
                }
            }
        }
        /*if (timer >= 1)
        {
            anim.SetBool("Attacking", false);
        }*/
        if (timer >= 1.5f)
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

    public void PunchAnimation(int side)
    {
        anim.SetInteger("Attack", side);
    }

}
