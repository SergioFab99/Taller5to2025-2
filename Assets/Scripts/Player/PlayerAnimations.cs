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
            float camPitch = _playerCombat.cam.eulerAngles.x;
            
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

        
        if (_playerCombat._state.isBlocking)
        {
            anim.SetBool("isBlocking", true);
            anim.Play("Block");
        }
        else
        {
            anim.SetBool("isBlocking", false);
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
