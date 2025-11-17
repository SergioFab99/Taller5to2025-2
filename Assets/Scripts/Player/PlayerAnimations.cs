using UnityEngine;
using Unity.Cinemachine;
using Unity.VisualScripting;
using JetBrains.Annotations;
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

    private bool wasGrappling = false;

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
        switch(side)
        {
            case 0:
                anim.Play("RightArm", 0);
                break;
            case 1:
                anim.Play("LeftArm", 0);
                break;
            case 2:
                anim.Play("BatAttack", 0);
                break;
            case 3:
                break;
        }    
    }


  
    void Update()
    {
        /*if (_playerCombat._heldObject != null)
        {
            grab = true;
        }
        else
        {
            grab = false;
        }
        anim.SetBool("GrabBat", grab);*/
        

        
       

        if (_playerCombat._state.isBlocking)
        {
            anim.SetBool("isBlocking", true);
            
        }
        else
        {
            anim.SetBool("isBlocking", false);
            
        }
        if(_playerCombat.currentWeapon.Wtype == WeaponType.Bat)
        {
            anim.SetBool("GrabBat", true);
        }
        else
        {
            anim.SetBool("GrabBat", false);
        }

        
    }

    void Shake()
    {
        CameraShake.cameraShakeInstance.Shake(shakeForceAttack, velocityAttack);

    }
}
