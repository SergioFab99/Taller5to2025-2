using UnityEngine;
using Unity.Cinemachine;
using Unity.VisualScripting;
using JetBrains.Annotations;
public class PlayerAnimation : MonoBehaviour
{
     PlayerCombat _playerCombat;


    public Animator animArms;
    public Animator animBody;
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
                animArms.Play("RightArm", 0);
                break;
            case 1:
                animArms.Play("LeftArm", 0);
                break;
            case 2:
                animArms.Play("BatAttack", 0);
                break;
            case 3:
                break;
        }    
    }


  
    void Update()
    {
        if (_playerCombat._state.isBlocking)
        {
            animArms.SetBool("isBlocking", true);
            
        }
        else
        {
            animArms.SetBool("isBlocking", false);
            
        }
        if(_playerCombat.currentWeapon.Wtype == WeaponType.Bat)
        {
            animArms.SetBool("GrabBat", true);
        }
        else
        {
            animArms.SetBool("GrabBat", false);
        }

        
    }

    void Shake()
    {
        CameraShake.cameraShakeInstance.Shake(shakeForceAttack, velocityAttack);

    }
}
