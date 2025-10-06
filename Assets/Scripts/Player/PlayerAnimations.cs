using UnityEngine;
using Unity.Cinemachine;
public class PlayerAnimation : MonoBehaviour
{
     PlayerCombat _playerCombat;


    public Animator anim;
    float timer;
    bool startTime;
    public int attack;
    [SerializeField] float shakeForceAttack;
    [SerializeField] Vector3 velocityAttack;

    public bool grab;



    public void Initialize(PlayerCombat playerCombat)
    {
        _playerCombat = playerCombat;
        _playerCombat.OnAttack += Attack;
        
    }


    private void OnDisable()
    {
        _playerCombat.OnAttack -= Attack;
    }

    void Attack(int side)
    {
       
        if (_playerCombat._heldObject != null && _playerCombat._heldObject.GetComponent<Bat>() != null)
        {
            anim.SetTrigger("AttackBat");
        }
        else
        {
            if (side == 1)
            {
                anim.Play("armRight");
            }
            else
            {
                anim.Play("armLeft");
            }
        }
           
    }
    void Update()
    {
        if (_playerCombat._heldObject != null)
        {
            grab = true;
        }
        else
        {
            grab = false;
        }
            
        anim.SetBool("GrabBat", grab);    
        
    }

    void Shake()
    {
        CameraShake.cameraShakeInstance.Shake(shakeForceAttack, velocityAttack);

    }
}
