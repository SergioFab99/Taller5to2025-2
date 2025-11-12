using UnityEngine;
using Unity.Cinemachine;
using Unity.VisualScripting;
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
        if(_playerCombat.currentWeapon != null)
        {
            (_playerCombat.currentWeapon as FistsWeapon).OnAttack += Attack;

        }
        
    }


    private void OnDisable()
    {
        _playerCombat.OnAttack -= Attack;
        (_playerCombat.currentWeapon as BatWeapon).OnAttack -= BatAttack;
    }

    void Attack(int side)
    {       
                
        {
            float camPitch = UnityEngine.Camera.main.transform.eulerAngles.x;
            
            if (camPitch > 180f) camPitch -= 360f;
            
            float threshold = 45f;
            if (camPitch < threshold)
            {
               
                if (side == 1)
                {
                    anim.Play("armRightUp");
                }
                else if (side == 2)
                {
                    anim.Play("armLeftUp");
                }
            }
            else
            {
                
                if (side == 1)
                {
                    anim.Play("armRightDown");
                }
                else if (side == 2)
                {
                    anim.Play("armLeftDown");
                }
            }
        }
           
    }

    public void BatAttack()
    {
        anim.SetTrigger("AttackBat");
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
        if (_playerCombat.currentWeapon.Wtype == WeaponType.Bat)
        {
            (_playerCombat.currentWeapon as BatWeapon).OnAttack += BatAttack;

        }
       

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

        // Animación de agarre (Grapple) con Q (TEMPORAL)
        bool isGrappling = Input.GetKey(KeyCode.Q);
        anim.SetBool("Grapple", isGrappling);
        if (isGrappling && !wasGrappling)
        {
            anim.Play("Grapple");
        }
        wasGrappling = isGrappling;
    }

    void Shake()
    {
        CameraShake.cameraShakeInstance.Shake(shakeForceAttack, velocityAttack);

    }
}
